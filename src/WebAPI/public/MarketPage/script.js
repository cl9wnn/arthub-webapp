import {tokenStorage, showForm, createLoginForm } from '../Auth/auth.js';
import {decorateImages} from "./decorateImages.js";

let balanceText = document.getElementById('balance-text');
let backgroundContainer = document.getElementById('backgrounds-container');
let frameContainer = document.getElementById('art-frames-container');
document.addEventListener('DOMContentLoaded', async () => {
    await updateBalance();
    await loadDecorationList();
    await generateCards(decorationList, frameContainer, backgroundContainer);
});


async function updateBalance() {
    const balance = await loadBalance();
    if (balance !== null) {
        balanceText.textContent = balance;
    }
}

let decorationList = [];
async function loadDecorationList() {
    let token = tokenStorage.get();

    try {
        const response = await fetch('/api/give-decorations', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        });

        if (response.ok) {
            decorationList = await response.json();
        }
        else if (response.status === 401) {
            const success = await showForm(createLoginForm, '/auth/signin', 'Sign In');
            if (success) {
                token = tokenStorage.get();
                return await loadBalance();
            }
        }
        else {
            const data = await response.json();
            alert(data);
        }
    } catch (error) {
        alert(error.message);
    }
}

async function loadBalance() {
    let token = tokenStorage.get();

    try {
        const response = await fetch('/api/get-balance', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });

        const data = await response.json();

        if (response.ok) {
            return data; 
        } else if (response.status === 401) {
            const success = await showForm(createLoginForm, '/auth/signin', 'Sign In');
            if (success) {
                token = tokenStorage.get();
                return await loadBalance(); 
            }
        } else {
            throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
        return null; 
    }
}

async function buyDecoration(decorationId) {
    let token = tokenStorage.get();
    
    try {
        const response = await fetch('/api/buy-decoration', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body:JSON.stringify(
                parseInt(decorationId, 10),
            )
        });
        
        if (response.ok) {
            await updateBalance();
            
            await loadDecorationList();

            frameContainer.innerHTML = '';
            backgroundContainer.innerHTML = '';

            await generateCards(decorationList, frameContainer, backgroundContainer);
        } else if (response.status === 401) {
            const success = await showForm(createLoginForm, '/auth/signin', 'Sign In');
            if (success) {
                token = tokenStorage.get();
                return await loadBalance();
            }
        } else {
            const data = await response.json();
            throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
    }
}

function generateCards(cardsData, container1, container2) {
    const type1Cards = cardsData
        .filter(card => card.decoration.typeName === 'frame')
        .slice(0, 6);
    const type2Cards = cardsData
        .filter(card => card.decoration.typeName === 'background')
        .slice(0, 6);

    type1Cards.forEach(({ decoration, isBought, isSelected }) => {
        const card = createCardElement(decoration, isBought, isSelected);
        container1.appendChild(card);
    });

    type2Cards.forEach(({ decoration, isBought, isSelected }) => {
        const card = createCardElement(decoration, isBought, isSelected);
        container2.appendChild(card);
    });
}

function createCardElement(decoration, isBought, isSelected) {
    const card = document.createElement('div');
    card.className = 'card';

    if (isBought) {
        card.style.backgroundColor = '#2D2D40FF'; 
        card.style.pointerEvents = 'none'; 
    }

    const image = document.createElement('img');
    const decorationImage = decorateImages.find(img => img.decorationId === decoration.decorationId);
    image.src = decorationImage ? decorationImage.image : 'https://via.placeholder.com/150';
    image.alt = decoration.name;

    const title = document.createElement('div');
    title.className = 'title';
    title.textContent = decoration.name;

    const price = document.createElement('div');
    price.className = 'price';
    price.textContent = isBought ? 'Куплено' : `Цена: ${decoration.cost}`;

    card.appendChild(image);
    card.appendChild(title);
    card.appendChild(price);

    if (isBought) {
        const selectButton = document.createElement('button');
        selectButton.className = 'select-button';

        if (isSelected) {
            selectButton.textContent = 'Выбрано';
            selectButton.disabled = true;
            selectButton.classList.add('disabled');
        } else {
            selectButton.textContent = 'Выбрать';
            selectButton.addEventListener('click', async (event) => {
                event.stopPropagation();
                await selectDecoration(decoration.decorationId);
                selectButton.textContent = 'Выбрано';
                selectButton.disabled = true;
                selectButton.classList.add('disabled');
            });
        }

        // Enable button interaction even when card clicks are disabled
        selectButton.style.pointerEvents = 'auto';
        card.appendChild(selectButton);
    }

    card.addEventListener('click', () => {
        if (!isBought) {
            showBuyModal(decoration.decorationId);
        }
    });

    return card;
}

async function selectDecoration(decorationId) {
    let token = tokenStorage.get();

    try {
        const response = await fetch('/api/select-decoration', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body:JSON.stringify(
                parseInt(decorationId, 10),
            )
        });

        if (response.ok) {
            await loadDecorationList();

            frameContainer.innerHTML = '';
            backgroundContainer.innerHTML = '';

            await generateCards(decorationList, frameContainer, backgroundContainer);
        } else if (response.status === 401) {
            const success = await showForm(createLoginForm, '/auth/signin', 'Sign In');
            if (success) {
                await updateBalance();
                await loadDecorationList();

                frameContainer.innerHTML = '';
                backgroundContainer.innerHTML = '';

                await generateCards(decorationList, frameContainer, backgroundContainer);
            }
        } else {
            const data = await response.json();
            throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
    }
}

async function createBuyModal(cardId) {
    const modal = document.createElement('div');
    modal.className = 'modal';

    const overlay = document.createElement('div');
    overlay.className = 'overlay';
    overlay.addEventListener('click', hideModal);

    const message = document.createElement('p');
    message.className = 'message-text';
    message.textContent = 'Уверены, что хотите купить?';

    const buttonsContainer = document.createElement('div');
    buttonsContainer.className = 'modal-buttons';

    const yesButton = document.createElement('button');
    yesButton.textContent = 'Да';
    yesButton.addEventListener('click', async () => {
        await buyDecoration(cardId);  
        hideModal();  
    });
    buttonsContainer.appendChild(yesButton);

    modal.appendChild(message);
    modal.appendChild(buttonsContainer);

    document.body.appendChild(overlay);
    overlay.appendChild(modal);
}

async function showBuyModal(cardId) {
    await createBuyModal(cardId);
}

function hideModal() {
    const modal = document.querySelector('.modal');
    const overlay = document.querySelector('.overlay');
    if (modal) modal.remove();
    if (overlay) overlay.remove();
}
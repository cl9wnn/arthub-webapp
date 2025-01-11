import {tokenStorage, showForm, createLoginForm } from '../Auth/auth.js';


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
            await showInfoModal();
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

const decorateImages = [
    { decorationId: 1, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 2, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 3, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 4, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 5, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 6, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 7, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 8, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 9, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 10, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 11, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
    { decorationId: 12, image: "https://static13.tgcnt.ru/posts/_0/a7/a7bb86e198b2b690366f24d7a761c4a6.jpg" },
];

function generateCards(cardsData, container1, container2) {
    const type1Cards = cardsData
        .filter(card => card.decoration.typeName === 'frame')
        .slice(0, 6);
    const type2Cards = cardsData
        .filter(card => card.decoration.typeName === 'background')
        .slice(0, 6);

    type1Cards.forEach(({ decoration, isBought }) => {
        const card = createCardElement(decoration, isBought);
        container1.appendChild(card);
    });

    type2Cards.forEach(({ decoration, isBought }) => {
        const card = createCardElement(decoration, isBought);
        container2.appendChild(card);
    });
}

function createCardElement(decoration, isBought) {
    console.log(decoration);

    const card = document.createElement('div');
    card.className = 'card';

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

    card.addEventListener('click', () => showBuyModal(decoration.decorationId));

    return card;
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

    const noButton = document.createElement('button');
    noButton.textContent = 'Нет';
    noButton.addEventListener('click', hideModal);

    const yesButton = document.createElement('button');
    yesButton.textContent = 'Да';
    yesButton.addEventListener('click', async () => {
        await buyDecoration(cardId);  
        hideModal();  
    });
    buttonsContainer.appendChild(noButton);
    buttonsContainer.appendChild(yesButton);

    modal.appendChild(message);
    modal.appendChild(buttonsContainer);

    document.body.appendChild(overlay);
    overlay.appendChild(modal);
}
function createInfoModal() {
    const modal = document.createElement('div');
    modal.className = 'infoModal';

    const message = document.createElement('p');
    message.className = 'message-text';
    message.textContent = 'Удачная покупка!';

    const button = document.createElement('button');
    button.textContent = 'Ок';
    button.addEventListener('click', async () => {
        if (modal) modal.remove();
    });

    modal.appendChild(message);
    modal.appendChild(button);

    document.body.appendChild(modal);
}

async function showBuyModal(cardId) {
    await createBuyModal(cardId);
}

async function showInfoModal() {
    await createInfoModal();
}

function hideModal() {
    const modal = document.querySelector('.modal');
    const overlay = document.querySelector('.overlay');
    if (modal) modal.remove();
    if (overlay) overlay.remove();
}
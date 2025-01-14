const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';
import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";
import {rewardImages} from "../MarketPage/rewardImages.js";

let artworkId;
let authorId;
let balance;

document.addEventListener('DOMContentLoaded', async() => {
    const pathname = window.location.pathname;
    const pathSegments = pathname.split("/");
    artworkId = pathSegments[pathSegments.length - 1];
    await getArtworkList();
    if (artworkId) {
        await loadArtworkData(artworkId);
        await loadArtworkRewardsData(artworkId);
    }
    await loadRewardList();
});

let isLiked = false; 
let isSaved = false;
const likeBtn = document.getElementById('like-btn');
const saveBtn = document.getElementById('save-btn');
const rewardBtn = document.getElementById('reward-btn');
likeBtn.addEventListener('click', async () => {
    
    if (artworkId) {
        await likeArtwork(artworkId);
    } else {
        alert("ошибка");
    }
});

saveBtn.addEventListener('click', async () => {

    if (artworkId) {
        await saveArtwork(artworkId);
    } else {
        alert("ошибка");
    }
});

let modal;
rewardBtn.addEventListener('click', async () => {
    if (modal) {
        modal.remove();
    }
    await loadBalance();
    modal = createModal(rewardList, balance); 
    document.body.appendChild(modal);
});

document.querySelector('.author-area').addEventListener('click', function() {
    
    window.location.href = `/account/${authorId}`;
});

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
          balance = data;
        }
        else if(response.status === 401) {
            const success = await showForm(createLoginForm, '/auth/signin', 'Sign In');
            if (success) {
                token = tokenStorage.get();
                await loadBalance();
            }
        }
        else{
            throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
    }
}

async function loadArtworkData(artworkId) {
    let token = tokenStorage.get();

    try {
        const response = await fetch(`/api/artwork/${artworkId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });

        const data = await response.json();
        
        if (response.ok) {
            isLiked = data.isLiked;
            isSaved = data.isSaved;
            authorId = data.authorId;
            await updateButton(isLiked, 'Liked', 'Like', likeBtn);
            await updateButton(isSaved, 'Saved', 'Save', saveBtn);
            await renderArtInfo(data);
        } 
        else if(response.status === 401) {
                const success = await showForm(createLoginForm, '/auth/signin', 'Sign In');
                if (success) {
                    token = tokenStorage.get(); 
                    await loadArtworkData(artworkId);
                }           
        }
        else{
                throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
    }
}

async function loadArtworkRewardsData(artworkId) {
    let token = tokenStorage.get();

    try {
        const response = await fetch(`/api/artwork-rewards/${artworkId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });

        const artworkRewards = await response.json();

        if (response.ok) {
            const container = document.querySelector('.rewards-container');
            await renderRewards(rewardImages, artworkRewards, container);
        }
        else if(response.status === 401) {
            const success = await showForm(createLoginForm, '/auth/signin', 'Sign In');
            if (success) {
            }
        }
        else{
            throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
    }
}

async function likeArtwork(artworkId) {
    const token = tokenStorage.get();
    try {
        const response = await fetch(`/api/like-artwork/${artworkId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });
        const data = await response.json();

        if (response.ok) {
            isLiked = !isLiked; 
            await updateButton(isLiked, 'Liked', 'Like', likeBtn);
            await updateLikeCount(data);
        }
        else if (response.status === 401) {
            await showForm(createLoginForm, '/auth/signin', 'Sign In');
        }
        else {
                throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
    }
}

async function saveArtwork(artworkId) {
    const token = tokenStorage.get();
    try {
        const response = await fetch(`/api/save-artwork/${artworkId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });
        const data = await response.json();

        if (response.ok) {
            isSaved = !isSaved;
            await updateButton(isSaved, 'Saved', 'Save', saveBtn);
        }
        else if (response.status === 401) {
            await showForm(createLoginForm, '/auth/signin', 'Sign In');
        }
        else {
            throw new Error(data || 'Ошибка на сервере!');
        }
    } catch (error) {
        alert(error);
    }
}

function updateButton(flag, enableText, disableText, button) {
    if (flag) {
        button.innerText = enableText;
        button.style.backgroundColor = '#e0e0e0';
        button.style.color = '#333333';
    } else {
        button.innerText = disableText;
        button.style.backgroundColor = '#333333';
        button.style.color = '#e0e0e0';
    }
}

async function renderArtInfo(data){
    document.getElementById('artImg').src = `${artFolderPath}${data.artworkPath}`;
    document.getElementById('title').innerHTML = data.title;
    document.getElementById('description').innerHTML = data.description;
    document.getElementById('category').innerHTML = data.category;
    document.getElementById('profileName').innerHTML = data.profileName;
    document.getElementById('fullname').innerHTML = data.fullname;
    document.getElementById('avatarImg').src = `${avatarFolderPath}${data.avatarPath}`;
    document.getElementById('like-text').innerHTML = data.likesCount;
    document.getElementById('view-text').innerHTML = data.viewsCount;
}
async function updateLikeCount(likeCount) {
    document.getElementById('like-text').innerHTML = `${likeCount}`;
}

let currentArtIndex = 0;

async function getArtworkList() {
    try {
        const response = await fetch('/api/get-artworks', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        artList = await response.json();

        if (response.ok) {
        } else {
            alert("ЧТо то не так");
        }
    } catch (error) {
        alert(error.message);
    }
}
document.getElementById('nextImgBtn').addEventListener('click', loadNextArtwork);
document.getElementById('prevImgBtn').addEventListener('click', loadPreviousArtwork);

function findCurrentArtIndex() {
    console.log(artList);
    return artList.findIndex(art => art.artworkId == artworkId);
}

async function loadNextArtwork() {
    const currentIndex = findCurrentArtIndex();
    if (currentIndex >= 0 && currentIndex < artList.length - 1) {
        const nextArt = artList[currentIndex + 1];
        artworkId = nextArt.artworkId; 
         window.location.href = `/artwork/${artworkId}`;
    } else {
        alert("Это последний арт.");
    }
}

async function loadPreviousArtwork() {
    const currentIndex = findCurrentArtIndex();
    if (currentIndex > 0) {
        const prevArt = artList[currentIndex - 1];
        artworkId = prevArt.artworkId;
        window.location.href = `/artwork/${artworkId}`;
    } else {
        alert("Это первый арт.");
    }
}

let rewardList = [];
async function loadRewardList() {
    try {
        const response = await fetch('/api/get-rewards', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });
        
        if (response.ok) {
            rewardList = await response.json();
        } else {
            alert("ЧТо то не так");
        }
    } catch (error) {
        alert(error.message);
    }
}


async function GiveReward(rewardId, artworkId) {
    const token = tokenStorage.get();
    try {
        const response = await fetch('/api/give-reward', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                rewardId: parseInt(rewardId, 10),
                artworkId: parseInt(artworkId, 10),
            })
        });

        const responseData = await response.json();

        if (response.ok) {
            await loadArtworkRewardsData(artworkId);
        }
        else if (response.status === 401) {
            const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

            if (loginSuccessful) {
                await loadArtworkData(artworkId);
                await loadArtworkRewardsData(artworkId);
            }
        }else {
            alert(responseData);
        }
    } catch (error) {
        alert(error.message);
    }
}


//модальное окно для награды

let artList = [];

function createModal(rewardList, userBalance) {
    let modal = document.createElement("div");
    modal.id = "rewardModal";
    modal.classList.add("modal");
    modal.innerHTML = `
    <div class="modal-content">
      <span class="close">&times;</span>
      <h2>Выдать награду</h2>
      <p>Используйте свои очки, чтобы наградить создателя этого предмета!</p>
      <div class="rewards"></div>
      <div class="modal-footer">
        <p id="balance">Ваш баланс: ${userBalance} очков</p>
        <button id="confirmButton" disabled>Подтвердить</button>
      </div>
    </div>
  `;

    const rewardsContainer = modal.querySelector(".rewards");
    let selectedReward = null;

    rewardList.forEach((reward) => {
        const rewardImage = rewardImages.find(img => img.rewardId === reward.rewardId)?.img || "";

        const rewardDiv = document.createElement("div");
        rewardDiv.classList.add("reward");
        rewardDiv.innerHTML = `
          <img src="${rewardImage}" alt="${reward.name}" class="reward-icon" />
          <p>${reward.name}</p>
          <p>${reward.cost} очков</p>
        `;

        rewardDiv.addEventListener("click", () => {
            const previousSelected = rewardsContainer.querySelector(".reward.selected");
            if (previousSelected) {
                previousSelected.classList.remove("selected");
            }

            rewardDiv.classList.add("selected");
            selectedReward = reward;

            confirmButton.disabled = false;
        });

        rewardsContainer.appendChild(rewardDiv);
    });

    const closeModalBtn = modal.querySelector(".close");
    closeModalBtn.addEventListener("click", () => {
        modal.remove();
        modal = null;
    });

    window.addEventListener("click", (event) => {
        if (event.target === modal) {
            modal.remove();
            modal = null;
        }
    });

    const confirmButton = modal.querySelector("#confirmButton");
    confirmButton.addEventListener("click", async () => {
        if (selectedReward) {
            await GiveReward(selectedReward.rewardId, artworkId);
            modal.remove();
            modal = null;
        }
    });

    return modal;
}

export function renderRewards(rewardImages, rewards, container) {

    container.innerHTML = '';

    if (rewards.length === 0) {
        const noRewardsMessage = document.createElement('div');
        noRewardsMessage.textContent = 'Пока наград нет';
        noRewardsMessage.classList.add('no-rewards-message');
        container.appendChild(noRewardsMessage);
        return;
    }

    rewards.forEach(reward => {
        const rewardImage = rewardImages.find(img => img.rewardId === reward.rewardId);

        if (rewardImage) {
            const rewardElement = document.createElement('div');
            rewardElement.classList.add('reward-item');

            const imgElement = document.createElement('img');
            imgElement.src = rewardImage.img;
            imgElement.alt = `Reward ${reward.rewardId}`;
            imgElement.classList.add('reward-img');
            rewardElement.appendChild(imgElement);

            const countElement = document.createElement('span');
            countElement.textContent = `x${reward.rewardCount}`;
            countElement.classList.add('reward-count');
            rewardElement.appendChild(countElement);

            container.appendChild(rewardElement);
        } else {
            console.warn(`Изображение для rewardId ${reward.rewardId} не найдено.`);
        }
    });
}



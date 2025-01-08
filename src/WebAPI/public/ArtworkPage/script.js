const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';
import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";

let artworkId;
let authorId;

document.addEventListener('DOMContentLoaded', async() => {
    const pathname = window.location.pathname;
    const pathSegments = pathname.split("/");
    artworkId = pathSegments[pathSegments.length - 1];
    await getArtworkList();
    if (artworkId) {
        await loadArtworkData(artworkId);
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
    modal = createModal(rewardList, 1500); 
    document.body.appendChild(modal);
});

document.querySelector('.author-area').addEventListener('click', function() {
    
    window.location.href = `/account/${authorId}`;
});

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
            await renderTags(data.tags);
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
    document.getElementById('like-text').innerHTML = `likes:${data.likesCount}`;
    document.getElementById('view-text').innerHTML = `views:${data.viewsCount}`;
}
async function updateLikeCount(likeCount) {
    document.getElementById('like-text').innerHTML = `likes: ${likeCount}`;
}

function renderTags(tags) {
    const tagsContainer = document.querySelector('.tags');

    if (!tagsContainer) {
        console.error("Контейнер с классом 'tags' не найден.");
        return;
    }

    tagsContainer.innerHTML = '';

    tags.forEach(tag => {
        const button = document.createElement('button');
        button.classList.add('tag'); 
        button.textContent = tag; 
        button.disabled = true;
        tagsContainer.appendChild(button); 
    });
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


const rewards = [
    { name: "Умно", cost: 2400, img: "https://via.placeholder.com/60" },
    { name: "Тёплое одеяльце", cost: 600, img: "https://via.placeholder.com/60" },
    { name: "Пикантно", cost: 1200, img: "https://via.placeholder.com/60" },
    { name: "Медленные аплодисменты", cost: 600, img: "https://via.placeholder.com/60" },
    { name: "Возьми мои очки", cost: 4800, img: "https://via.placeholder.com/60" },
    { name: "Душевно", cost: 300, img: "https://via.placeholder.com/60" },
];


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
             console.log(responseData);
        } else {
            alert(responseData);
        }
    } catch (error) {
        alert(error.message);
    }
}


//модальное окно для награды

let artList = [];
let rewardImages = [
    { rewardId: 1, img: "/public/favicon.png" },
    { rewardId: 2, img: "https://via.placeholder.com" },
    { rewardId: 3, img: "https://via.placeholder.com" },
    { rewardId: 4, img: "https://via.placeholder.com" },
    { rewardId: 5, img: "https://via.placeholder.com" },
    { rewardId: 6, img: "https://via.placeholder.com" }
];

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
        const rewardImage = rewardImages.find(img => img.rewardId === reward.id)?.img || "";

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



const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';
import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";

let artworkId;
let authorId;
document.addEventListener('DOMContentLoaded', async() => {
    const pathname = window.location.pathname;
    const pathSegments = pathname.split("/");
    artworkId = pathSegments[pathSegments.length - 1];

    if (artworkId) {
        await loadArtworkData(artworkId);
    }
});

let isLiked = false; 
let isSaved = false;
const likeBtn = document.getElementById('like-btn');
const saveBtn = document.getElementById('save-btn');

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
        console.log(data);
        
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
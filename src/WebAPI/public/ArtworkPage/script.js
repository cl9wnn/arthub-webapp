﻿const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
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
const likeBtn = document.getElementById('like-btn');
likeBtn.addEventListener('click', async () => {
    
    if (artworkId) {
        await likeArtwork(artworkId);
    } else {
        alert("Artwork ID not found in localStorage!");
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
        
        if (response.ok) {
            isLiked = data.isLiked;
            authorId = data.authorId;
            await updateLikeButton();
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
            await updateLikeButton();
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

function updateLikeButton() {
    if (isLiked) {
        likeBtn.innerText = 'Liked';
        likeBtn.style.backgroundColor = '#e0e0e0';
        likeBtn.style.color = '#333333';
    } else {
        likeBtn.innerText = 'Like';
        likeBtn.style.backgroundColor = '#333333';
        likeBtn.style.color = '#e0e0e0';
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
    document.getElementById('like-text').innerHTML = `likes:${data.likeCount}`;
}
async function updateLikeCount(likeCount) {
    document.getElementById('like-text').innerHTML = `likes: ${likeCount}`;
}
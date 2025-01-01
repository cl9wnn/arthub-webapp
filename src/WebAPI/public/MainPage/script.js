import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm } from '../Auth/auth.js';
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';
const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';


const accountSection = document.getElementById("accountBtn");
const signupSection = document.getElementById("signupBtn");
const signinSection = document.getElementById("signinBtn");
const marketSection = document.getElementById("marketBtn");

document.addEventListener('DOMContentLoaded', async () => {
    const setupButton = (id, createFormMethod, path, buttonText) => {
        document.getElementById(id).addEventListener('click', (event) => {
            event.preventDefault();
            showForm(createFormMethod, path, buttonText);
        });
    };

    setupButton('signinBtn', createLoginForm, '/auth/signin', 'Sign in');
    toggleVisibility(tokenStorage.get());
    await loadArtworkList();
});

document.getElementById('signupBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/register-account';
});

document.getElementById('marketBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/market'; 
});

document.getElementById('accountBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/account';
});

function toggleVisibility(token) {
    if (token) {
        accountSection.style.display = "block";
        marketSection.style.display = "block";
        signupSection.style.display = "none";
        signinSection.style.display = "none";
    } else {
        marketSection.style.display = "none";
        accountSection.style.display = "none";
        signupSection.style.display = "block";
        signinSection.style.display = "block";
    }
}

async function loadArtworkList() {
    const container = document.querySelector(".artwork-container");
    try {
        const response = await fetch('/api/get-artworks', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        const artworkArray = await response.json();
        console.log(artworkArray);
        
        artworkArray.forEach(art => {
            const artworkComponent = createArtworkComponent(art);
            container.appendChild(artworkComponent);
        });
        
        if (response.ok) {
        } else {
            alert("ЧТо то не так");
        }
    } catch (error) {
        alert(error.message);
    }
}

function createArtworkComponent({ title, profileName, artworkPath, avatarPath }) {
    const artworkDiv = document.createElement("div");
    artworkDiv.classList.add("artwork");

    const img = document.createElement("img");
    img.src = `${artFolderPath}${artworkPath}`;
    img.alt = title;

    const infoDiv = document.createElement("div");
    infoDiv.classList.add("artwork-info");

    const avatar = document.createElement("img");
    avatar.src = `${avatarFolderPath}${avatarPath}`;
    avatar.classList.add("avatar");

    const textDiv = document.createElement("div");
    textDiv.classList.add("artwork-text");
    textDiv.innerHTML = `<h3>${title}</h3><p>${profileName}</p>`;

    infoDiv.appendChild(avatar);
    infoDiv.appendChild(textDiv);

    artworkDiv.appendChild(img);
    artworkDiv.appendChild(infoDiv);

    return artworkDiv;
}





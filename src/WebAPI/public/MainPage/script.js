import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm, parseJwtToSub } from '../Auth/auth.js';
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';
const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';


const accountSection = document.getElementById("accountBtn");
const marketSection = document.getElementById("marketBtn");
const savingSection = document.getElementById("savingsBtn");
const signupSection = document.getElementById("signupBtn");
const signinSection = document.getElementById("signinBtn");

document.addEventListener('DOMContentLoaded', async () => {
    const setupButton = (id, createFormMethod, path, buttonText) => {
        document.getElementById(id).addEventListener('click', async (event) => {
            event.preventDefault();
            const success = await showForm(createFormMethod, path, buttonText);
            if (success) {
                toggleVisibility(tokenStorage.get());
                await loadArtworkList();
            }
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

document.getElementById('savingsBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/savings';
});

document.getElementById('accountBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    const userId = parseJwtToSub(tokenStorage.get());
    window.location.href = `/account/${userId}`;
});

function toggleVisibility(token) {
    if (token) {
        accountSection.style.display = "block";
        savingSection.style.display = 'block';
        marketSection.style.display = "block";
        signupSection.style.display = "none";
        signinSection.style.display = "none";
    } else {
        marketSection.style.display = "none";
        savingSection.style.display = "none";
        accountSection.style.display = "none";
        signupSection.style.display = "block";
        signinSection.style.display = "block";
    }
}

async function loadArtworkList() {
    const container = document.querySelector(".artwork-container");
    container.innerHTML = '';  
    try {
        const response = await fetch('/api/get-artworks', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        const artworkArray = await response.json();
        
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

function createArtworkComponent({artworkId, title, profileName, artworkPath, avatarPath }) {
    const artworkDiv = document.createElement("div");
    artworkDiv.classList.add("artwork");

    artworkDiv.dataset.id = artworkId;

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

    artworkDiv.addEventListener("click", () => {
        window.location.href = `/artwork/${artworkId}`;
    });

    return artworkDiv;
}



import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';

document.addEventListener('DOMContentLoaded', async() => {
    await loadSavingsData();
});

async function loadSavingsData() {
    let token = tokenStorage.get();

    try {
        const response = await fetch('/api/get-savings', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        });

        const data = await response.json();

        if (response.ok) {
            await displayArtworks(data);
        }
        else if (response.status === 401) {
            const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

            if (loginSuccessful) {
                await loadSavingsData();
            }
        }
        else
        {
            alert(data);
        }
    } catch (error) {
        alert(error.message);
    }
}

async function deleteSavingArtwork(artworkId, artCard) {
    let token = tokenStorage.get();

    try {
        const response = await fetch('/api/delete-saving', {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(artworkId )
        });
        
        if (response.ok) {
            artCard.remove();
            await loadSavingsData();
        }
        else if (response.status === 401) {
            const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

            if (loginSuccessful) {
                await loadSavingsData();
            }
        }
        else
        {
            alert(data);
        }
    } catch (error) {
        alert(error.message);
    }
}

async function displayArtworks(artworksArray) {
    const artsContainer = document.querySelector('.arts-container');
    artsContainer.innerHTML = '';

    artworksArray.forEach(artwork => {
        const artCard = document.createElement('div');
        artCard.className = 'art-card';
        
        artCard.addEventListener("click", () => {
            window.location.href = `/artwork/${artwork.artworkId}`;
        });

        const closeButton = document.createElement('button');
        closeButton.className = 'closeBtn';
        closeButton.textContent = '✖';
        closeButton.addEventListener('click', async (event) => {
            event.stopPropagation();
            await deleteSavingArtwork(artwork.artworkId, artCard);
        });

        const imgElement = document.createElement('img');
        imgElement.src = `${artFolderPath}${artwork.artworkPath}`; 
        imgElement.alt = artwork.title;

        const titleElement = document.createElement('h4');
        titleElement.textContent = artwork.title;

        artCard.appendChild(closeButton);
        artCard.appendChild(imgElement);
        artCard.appendChild(titleElement);

        artsContainer.appendChild(artCard);
    });
}
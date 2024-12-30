import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";

const avatarBucketPath = 'http://localhost:9000/image-bucket/avatars/';
const upgradeBtn = document.getElementById('upgradeBtn');

let avatarImg, profileName, country;

document.addEventListener('DOMContentLoaded', async() => {
    avatarImg = document.getElementById('avatarImg');
    profileName = document.getElementById('profileName');
    country = document.getElementById('country');
    
    await loadAccountData();
});

// Обработчик кнопки Upgrade
document.getElementById('upgradeBtn').addEventListener('click', () => {
    window.location.href = '/register-artist';
});

// Обработчик кнопки добавления арта
document.getElementById('addArtBtn').addEventListener('click', async () => {
    const token = tokenStorage.get();

    try {
        const response = await fetch('/api/add-artwork', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        });

        const data = await response.json();

        if (response.ok) {
            alert(data);
        } else {
            throw new Error(data);
        }
    } catch (error) {
        alert(error.message);
    }
});

async function renderAccountData(data) {
    upgradeBtn.style.display = "flex";
    profileName.innerText = data.profileName;
    avatarImg.src = `${avatarBucketPath}${data.avatar}`;
    country.innerText = data.country;
}

function addUpgradeAccountData(data) {
    upgradeBtn.style.display = "none";

    const nameInfo = document.querySelector('.name-info');
    const contactInfoContainer = document.querySelector('.contact-info');
    const nameContainer = document.querySelector('.name-container');

    const fullnameElement = document.createElement('h1');
    fullnameElement.id = 'fullname';
    fullnameElement.innerText = data.fullname;
    nameInfo.appendChild(fullnameElement);

    const contactInfoElement = document.createElement('p');
    contactInfoElement.id = 'contact-info';
    contactInfoElement.innerText = data.contactInfo;
    contactInfoContainer.appendChild(contactInfoElement);
    
    const badgeArtistElement = document.createElement('span');
    badgeArtistElement.id = 'badge';
    badgeArtistElement.innerText = 'artist';
    nameContainer.appendChild(badgeArtistElement);
    
}
async function handleGuestView() {

    const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

    if (loginSuccessful) {
        await loadAccountData();
    }
}

async function loadAccountData() {
    const token = tokenStorage.get();

    try {
        const response = await fetch('/api/get-account', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        });

        const data = await response.json();

        if (response.ok) {
            await renderAccountData(data);

            if (data.role === 'artist') { 
                await addUpgradeAccountData(data);
            }
        } else {
            await handleGuestView();
        }
    } catch (error) {
        alert(error.message);
    }
}

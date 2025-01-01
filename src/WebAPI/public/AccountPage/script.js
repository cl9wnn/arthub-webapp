import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";

const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
// для апгрейда и для добавления работы в зависимости от роли
const addArtworkBtn = document.getElementById('addArtBtn'); 
const portfolioText = document.getElementById('portfolio-text');

let avatarImg, profileName, country;
let isArtist = false;

document.addEventListener('DOMContentLoaded', async() => {
    avatarImg = document.getElementById('avatarImg');
    profileName = document.getElementById('profileName');
    country = document.getElementById('country');
    
    await loadAccountData();
});

// Обработчик кнопки Upgrade и Add Artwork (для художников)
document.getElementById('addArtBtn').addEventListener('click', () => {
    if (isArtist) {
        window.location.href = '/new/artwork';
    }
    else{
        window.location.href = '/register-artist';
    }
});


async function renderAccountData(data) {
    addArtworkBtn.textContent = 'Upgrade';
    portfolioText.innerText = 'Improve your account by filling additional information to add own artworks';
    profileName.innerText = data.profileName;
    avatarImg.src = `${avatarFolderPath}${data.avatarPath}`;
    country.innerText = data.country;
}

function addUpgradeAccountData(data) {
    addArtworkBtn.textContent = 'Add artwork';
    portfolioText.textContent = 'Add your own artworks so that others can rate and promote you';

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

// загружаем данные аккаунта
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
                isArtist = true;
                await addUpgradeAccountData(data);
            }
        } else {
            await handleGuestView();
        }
    } catch (error) {
        alert(error.message);
    }
}

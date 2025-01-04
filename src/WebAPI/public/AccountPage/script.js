import {createLoginForm, showForm, tokenStorage, parseJwtToSub} from "../Auth/auth.js";
const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';

let avatarImg, profileName, country;
let userId;
let isArtist = false;

document.addEventListener('DOMContentLoaded', async() => {
    avatarImg = document.getElementById('avatarImg');
    profileName = document.getElementById('profileName');
    country = document.getElementById('country');

    const pathname = window.location.pathname;
    const pathSegments = pathname.split("/");
    userId = pathSegments[pathSegments.length - 1];
    
    const userTokenId = parseJwtToSub(tokenStorage.get());

    if (userId === userTokenId) {
        await createMyAccount();
    }
    
    if (userId){
        await loadAccountData(userId);
    }
});

let addArtworkBtn;  
let portfolioText;

async function createMyAccount() {
    const container = document.querySelector('.portfolio-container');

    addArtworkBtn = document.createElement('button');  
    addArtworkBtn.classList.add('profile-button');
    addArtworkBtn.id = 'addArtBtn';

    addArtworkBtn.addEventListener('click', () => {
        if (isArtist) {
            window.location.href = '/new/artwork';
        } else {
            window.location.href = '/register-artist';
        }
    });

    portfolioText = document.createElement('p');
    portfolioText.id = 'portfolio-text';

    container.appendChild(portfolioText);
    container.appendChild(addArtworkBtn);


}
async function renderAccountData(data) {
    if (addArtworkBtn){
        addArtworkBtn.textContent = 'Upgrade';
        portfolioText.innerText = 'Improve your account by filling additional information to add own artworks';
    }
    profileName.innerText = data.profileName;
    avatarImg.src = `${avatarFolderPath}${data.avatarPath}`;
    country.innerText = data.country;
}

function addUpgradeAccountData(data) {
    if (addArtworkBtn){
        addArtworkBtn.textContent = 'Add artwork';
        portfolioText.textContent = 'Add your own artworks so that others can rate and promote you';
    }

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

// загружаем данные аккаунта
async function loadAccountData(userId) {
    let token = tokenStorage.get();

    try {
        const response = await fetch(`/api/account/${userId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        });

        const data = await response.json();
        console.log(data);
        
        if (response.ok) {
            await renderAccountData(data);

            if (data.role === 'artist') { 
                isArtist = true;
                await addUpgradeAccountData(data);
            }
        } 
        else if (response.status === 401) {
            const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

            if (loginSuccessful) {
                token = tokenStorage.get();
                await loadAccountData(userId);
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

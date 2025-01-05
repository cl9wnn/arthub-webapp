import {createLoginForm, showForm, tokenStorage, parseJwtToSub} from "../Auth/auth.js";
const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';

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
    const portfolioHeader = document.querySelector('.portfolio-header');
    const portfolioTextContainer = document.querySelector('.portfolio-text-container');

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

    portfolioTextContainer.appendChild(addArtworkBtn);
    portfolioHeader.appendChild(portfolioText);
}

const portfolio = document.querySelector('.Portfolio');
async function generateArtsContainer(profileArts) {
    const portfolioContainer = document.createElement('div');
    portfolioContainer.classList.add('portfolio-container');

    const artsContainer = document.createElement('div');
    artsContainer.classList.add('arts-container');

    const prevButton = document.createElement('button');
    prevButton.classList.add('slider-btn', 'prev-btn');
    prevButton.innerText = '‹';
    artsContainer.appendChild(prevButton);

    const artSlider = document.createElement('div');
    artSlider.classList.add('art-slider');
    artsContainer.appendChild(artSlider);

    const nextButton = document.createElement('button');
    nextButton.classList.add('slider-btn', 'next-btn');
    nextButton.innerText = '›';
    artsContainer.appendChild(nextButton);

    profileArts.forEach(artwork => {
        const artItem = document.createElement('div');
        artItem.classList.add('art-item');
        artItem.addEventListener("click", () => {
            window.location.href = `/artwork/${artwork.artworkId}`;
        });

        const img = document.createElement('img');
        img.classList.add('art-img');
        img.src = `${artFolderPath}${artwork.artworkPath}`;
        artItem.appendChild(img);

        const likeCountText = document.createElement('div');
        likeCountText.classList.add('like-count');
        likeCountText.textContent = `likes: ${artwork.likeCount}`;
        artItem.appendChild(likeCountText);

        artSlider.appendChild(artItem);
    });

    let currentIndex = 0;
    const itemsToShow = 3;
    const itemWidth = 275;
    const maxIndex = Math.max(0, profileArts.length - itemsToShow);

    function updateSliderPosition() {
        const offset = -currentIndex * itemWidth;
        artSlider.style.transform = `translateX(${offset}px)`;
    }

    prevButton.addEventListener('click', () => {
        if (currentIndex > 0) {
            currentIndex--;
            updateSliderPosition();
        }
    });

    nextButton.addEventListener('click', () => {
        if (currentIndex < maxIndex) {
            currentIndex++;
            updateSliderPosition();
        }
    });

    portfolioContainer.appendChild(artsContainer);
    return portfolioContainer;
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
        addArtworkBtn.textContent = '+';
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
        
        if (response.ok) {
            await renderAccountData(data);

            if (data.role === 'artist') { 
                isArtist = true;
                if (data.profileArts.length > 0) {
                    const portfolioContainer = await generateArtsContainer(data.profileArts);
                    portfolio.appendChild(portfolioContainer);
                }
                await addUpgradeAccountData(data);
            }
        } 
        else if (response.status === 401) {
            const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

            if (loginSuccessful) {
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

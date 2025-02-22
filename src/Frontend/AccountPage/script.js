import {createLoginForm, showForm, tokenStorage, parseJwtToSub} from "../Auth/auth.js";
import {decorateImages} from "../MarketPage/decorateImages.js";
import {rewardImages} from "../MarketPage/rewardImages.js";

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
        await loadAccountDecoration(userId);
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
    const userTokenId = parseJwtToSub(tokenStorage.get());

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

        const metricsContainer = document.createElement('div');
        metricsContainer.classList.add('metrics-container');

        const likesCountText = document.createElement('p');
        likesCountText.classList.add('info-count');
        likesCountText.textContent = `likes: ${artwork.likesCount}`;

        const viewsCountText = document.createElement('p');
        viewsCountText.classList.add('info-count');
        viewsCountText.textContent = `views: ${artwork.viewsCount}`;

        metricsContainer.appendChild(likesCountText);
        metricsContainer.appendChild(viewsCountText);

        artItem.appendChild(metricsContainer);

        if (userTokenId === userId){
            const deleteButton = document.createElement('button');
            deleteButton.className = 'deleteBtn';
            deleteButton.textContent = '✖';

            deleteButton.addEventListener('click', async (event) => {
                event.stopPropagation();
                await createConfirmationModal(artwork.artworkId, artItem);
            });
            artItem.appendChild(deleteButton);
        }
        artSlider.appendChild(artItem);
    });

    let currentIndex = 0;
    const itemsToShow = 3;
    const itemWidth = 250;
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
        addArtworkBtn.textContent = 'Upgrade account';
        portfolioText.innerText = 'Upgrade your account by filling additional information to add own artworks';
    }
    profileName.innerText = data.profileName;
    avatarImg.src = `${avatarFolderPath}${data.avatarPath}`;
    country.innerText = data.country;
}

async function createArtistSummary(data) {
    const summaryDiv = document.createElement('div');
    summaryDiv.className = 'summary';

    const summaryContainer = document.createElement('div');
    summaryContainer.className = 'summary-container';

    const heading = document.createElement('h1');
    heading.id = 'summary';
    heading.textContent = 'About Artist';

    const summaryContent = document.createElement('div');
    summaryContent.className = 'summary-content';

    const createTextWithIcon = (iconSrc, textHtml) => {
        const itemContainer = document.createElement('div');
        itemContainer.className = 'item-container';

        const iconElement = document.createElement('img');
        iconElement.src = iconSrc;
        iconElement.alt = 'icon';
        iconElement.className = 'icon';

        const textElement = document.createElement('div');
        textElement.className = 'text-element';
        textElement.innerHTML = textHtml;

        itemContainer.appendChild(iconElement);
        itemContainer.appendChild(textElement);
        return itemContainer;
    };

    const fullnameElement = createTextWithIcon('/static/resources/profileIcons/name.png', ` ${data.fullname}`);
    const contactParagraph = createTextWithIcon('/static/resources/profileIcons/contact.png', `${data.contactInfo}`);
    const experienceParagraph = createTextWithIcon('/static/resources/profileIcons/experience.png', `${data.summary}`);

    const textContainer = document.createElement('div');
    textContainer.className = 'text-container';
    textContainer.appendChild(fullnameElement);
    textContainer.appendChild(contactParagraph);
    textContainer.appendChild(experienceParagraph);

    summaryContent.appendChild(textContainer);

    summaryContainer.appendChild(heading);
    summaryContainer.appendChild(summaryContent);

    summaryDiv.appendChild(summaryContainer);

    return summaryDiv;
}


async function addUpgradeAccountData(data) {
    const wrapper = document.querySelector('.wrapper');

    if (addArtworkBtn) {
        addArtworkBtn.textContent = '+';
        portfolioText.textContent = 'Add your own artworks so that others can rate and promote you';
    }

    const nameContainer = document.querySelector('.name-container');

    let badgeArtistElement = document.getElementById('badge');
    if (!badgeArtistElement) {
        badgeArtistElement = document.createElement('span');
        badgeArtistElement.id = 'badge';
        nameContainer.appendChild(badgeArtistElement);
    }
    badgeArtistElement.innerText = 'PRO';

    const portfolio = wrapper.querySelector('.Portfolio');

    const existingSummary = wrapper.querySelector('.artist-summary');
    if (existingSummary) {
        existingSummary.remove();
    }

    const existingRewardContainer = wrapper.querySelector('.rewards'); 
    if (existingRewardContainer) {
        existingRewardContainer.remove();
    }

    const summaryContainer = await createArtistSummary(data);
    summaryContainer.classList.add('artist-summary');
    wrapper.insertBefore(summaryContainer, portfolio);

    const rewardContainer = await renderRewards(rewardImages, data.rewards);
    wrapper.appendChild(rewardContainer);
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
                let portfolioContainer = portfolio.querySelector('.portfolio-container');
                if (portfolioContainer) {
                    portfolioContainer.remove();
                }

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
                await loadAccountDecoration(userId);
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

//загружаем купленный фон
async function loadAccountDecoration(userId) {
    let token = tokenStorage.get();

    try {
        const response = await fetch(`/api/get-decoration/${userId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
        });

        const decorationId = await response.json();

        if (response.ok) {
            const image = decorateImages.find(item => item.decorationId === decorationId);
            document.body.style.backgroundImage = `url('${image.image}')`;
        }
        else if (response.status === 401) {
            const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

            if (loginSuccessful) {
                await loadAccountDecoration(userId);
            }
        }
        else
        {
            alert(data);
        }
    } catch (error) {
        alert(error);
    }
}

async function deleteOwnArtwork(artworkId, art) {
    let token = tokenStorage.get();
    const userId = parseJwtToSub(tokenStorage.get());

    try {
        const response = await fetch('/api/delete-own-artwork', {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(artworkId )
        });

        if (response.ok) {
            art.remove();
            await loadAccountData(userId);
            await loadAccountDecoration(userId);
        }
        else if (response.status === 401) {
            const loginSuccessful = await showForm(createLoginForm, '/auth/signin', 'Sign In');

            if (loginSuccessful) {
                await loadAccountData(userId);
                await loadAccountDecoration(userId);
            }
        }
        else
        {
            alert("error");
        }
    } catch (error) {
        alert(error.message);
    }
}

async function createConfirmationModal(artworkId, artItem) {
    const modal = document.createElement('div');
    modal.className = 'modal';

    const modalContent = document.createElement('div');
    modalContent.className = 'modal-content';

    const message = document.createElement('p');
    message.textContent = 'Вы уверены, что хотите безвозвратно удалить это?';

    const actions = document.createElement('div');
    actions.className = 'modal-actions';

    const confirmButton = document.createElement('button');
    confirmButton.className = 'button button-confirm';
    confirmButton.textContent = 'Yes';

    const cancelButton = document.createElement('button');
    cancelButton.className = 'button button-cancel';
    cancelButton.textContent = 'Cancel';

    confirmButton.addEventListener('click', async () => {
        await deleteOwnArtwork(artworkId, artItem);
        document.body.removeChild(modal);
    });

    cancelButton.addEventListener('click', () => {
        document.body.removeChild(modal);
    });

    actions.appendChild(confirmButton);
    actions.appendChild(cancelButton);

    modalContent.appendChild(message);
    modalContent.appendChild(actions);

    modal.appendChild(modalContent);

    document.body.appendChild(modal);

    modal.style.display = 'flex';
}

function renderRewards(rewardImages, rewards) {
    const mainContainer = document.createElement('div');
    mainContainer.classList.add('rewards');

    const title = document.createElement('h1');
    title.textContent = 'Rewards';
    mainContainer.appendChild(title);

    const container = document.createElement('div');
    container.classList.add('rewards-container');
    mainContainer.appendChild(container);

    container.innerHTML = '';

    if (rewards.length === 0) {
        const noRewardsMessage = document.createElement('div');
        noRewardsMessage.textContent = 'There are no rewards yet';
        noRewardsMessage.classList.add('no-rewards-message');
        container.appendChild(noRewardsMessage);
    } else {
        rewards.forEach(reward => {
            const rewardImage = rewardImages.find(img => img.rewardId === reward.rewardId);

            if (rewardImage) {
                const rewardElement = document.createElement('div');
                rewardElement.classList.add('reward-item');

                const imgElement = document.createElement('img');
                imgElement.src = rewardImage.img;
                imgElement.alt = `Reward ${reward.rewardId}`;
                imgElement.classList.add('reward-img');
                rewardElement.appendChild(imgElement);

                const countElement = document.createElement('span');
                countElement.textContent = `x${reward.rewardCount}`;
                countElement.classList.add('reward-count');
                rewardElement.appendChild(countElement);

                container.appendChild(rewardElement);
            } else {
                console.warn(`Изображение для rewardId ${reward.rewardId} не найдено.`);
            }
        });
    }

    return mainContainer;
}
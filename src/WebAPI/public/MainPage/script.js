import {decorateImages} from "../MarketPage/decorateImages.js";

const artFolderPath = 'http://localhost:9000/image-bucket/arts/';
const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
let artworkData = [];

document.addEventListener('DOMContentLoaded', async () => {
    await loadArtworkList();
});


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

        artworkData = await response.json();
        
        artworkData.forEach(art => {
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

function createArtworkComponent({artworkId, title, profileName, artworkPath, avatarPath, decorationId }) {
    const artworkDiv = document.createElement("div");
    artworkDiv.classList.add("artwork");
    artworkDiv.dataset.id = artworkId;

    const img = document.createElement("img");
    img.src = `${artFolderPath}${artworkPath}`;
    img.alt = title;

    const frameOverlay = document.createElement("div");
    frameOverlay.classList.add("artwork-frame-overlay");
    const image = decorateImages.find(item => item.decorationId === decorationId);
    frameOverlay.style.backgroundImage = `url('${image.image}')`;
    
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
    artworkDiv.appendChild(frameOverlay);
    artworkDiv.appendChild(infoDiv);

    artworkDiv.addEventListener("click", () => {
        window.location.href = `/artwork/${artworkId}`;
    });

    return artworkDiv;
}

// обработчики поиска, фильтрации, сортировки

let searchValue = '';
let selectedCategory = '';
let selectedTag = '';
let sortValue = 'none';
let filteredArtworkData = [];

function applyFiltersAndSorting() {
    filteredArtworkData = [...artworkData];

    // Фильтрация по категории
    if (selectedCategory) {
        filteredArtworkData = filteredArtworkData.filter(art => art.category === selectedCategory);
    }

    // Фильтрация по тегу
    if (selectedTag) {
        filteredArtworkData = filteredArtworkData.filter(art => art.tags.includes(selectedTag));
    }

    // Фильтрация по поиску
    if (searchValue) {
        filteredArtworkData = filteredArtworkData.filter(art => {
            const title = art.title.toLowerCase();
            const author = art.profileName.toLowerCase();
            return title.startsWith(searchValue) || author.startsWith(searchValue);
        });
    }

    // Сортировка
    if (sortValue !== 'none') {
        const [key, order] = sortValue.split('-');
        const keyMapping = {
            likes: 'likesCount',
            views: 'viewsCount',
        };

        const dataKey = keyMapping[key];
        if (dataKey) {
            filteredArtworkData.sort((a, b) => {
                return order === 'asc' ? a[dataKey] - b[dataKey] : b[dataKey] - a[dataKey];
            });
        }
    }

    renderArtworkList();
}

function renderArtworkList() {
    const container = document.querySelector(".artwork-container");
    container.innerHTML = '';

    filteredArtworkData.forEach(art => {
        const artworkComponent = createArtworkComponent(art);
        container.appendChild(artworkComponent);
    });
}

document.getElementById('searchInput').addEventListener('input', (event) => {
    searchValue = event.target.value.toLowerCase();
    applyFiltersAndSorting();
});

document.getElementById('category').addEventListener('change', (event) => {
    selectedCategory = event.target.value;
    applyFiltersAndSorting();
});

document.getElementById('sortSelect').addEventListener('change', (event) => {
    sortValue = event.target.value;
    applyFiltersAndSorting();
});

const tagButtons = document.querySelectorAll('.tag');
tagButtons.forEach(button => {
    button.addEventListener('click', (event) => {
        const clickedTag = event.target.dataset.tag;

        if (selectedTag === clickedTag) {
            selectedTag = '';
            event.target.classList.remove('selected');
        } else {
            selectedTag = clickedTag;

            tagButtons.forEach(btn => btn.classList.remove('selected'));
            event.target.classList.add('selected');
        }

        applyFiltersAndSorting();
    });
});
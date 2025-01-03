const avatarFolderPath = 'http://localhost:9000/image-bucket/avatars/';
const artFolderPath = 'http://localhost:9000/image-bucket/arts/';


document.addEventListener('DOMContentLoaded', async() => {
    const artworkId = sessionStorage.getItem('artworkId');
    if (artworkId) {
        await loadArtworkData(artworkId);
    } else {
        alert("Artwork ID not found in localStorage!");
    }});


async function loadArtworkData(artworkId) {
    try {
        const response = await fetch(`/api/artwork/${artworkId}`, {
            method: 'GET'
        });

        const data = await response.json();
        
        if (response.ok) {
            await renderArtInfo(data);
        } 
        else {
            throw new Error(data);
        }
    } catch (error) {
        alert(error);
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
}
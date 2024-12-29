import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";
document.addEventListener('DOMContentLoaded', () => {
    loadAccountData();
});
const avatarBucketPath = 'http://localhost:9000/image-bucket/avatars/';
const avatarImg = document.getElementById('avatarImg');
const realName = document.getElementById('realName');
const profileName = document.getElementById('profileName');
const contactInfo = document.getElementById('contact-info');
const country = document.getElementById('country');

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
            realName.innerText = data.realName;
            profileName.innerText = data.profileName;
            contactInfo.innerText = data.contactInfo;
            avatarImg.src = `${avatarBucketPath}${data.avatar}`;
            country.innerText = data.country;
        } else {
            realName.innerText = 'username';
            profileName.innerText = 'about you';
            contactInfo.innerText = 'contact Info';
            avatarImg.src = `${avatarBucketPath}/default_avatar.png`;
            country.innerText = 'not found';

            showForm(createLoginForm, '/auth/signin', 'Sign In');
        }
    } catch (error) {
        alert(error.message);
    }
}
import {createLoginForm, showForm, tokenStorage} from "../Auth/auth.js";
document.addEventListener('DOMContentLoaded', () => {
    loadAccountData();
});
const avatarBucketPath = 'http://localhost:9000/image-bucket/avatars/';
const avatarImg = document.getElementById('avatarImg');
const realName = document.getElementById('realName');
const profileName = document.getElementById('profileName');


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
            avatarImg.src = `${avatarBucketPath}${data.avatar}`;
        } else {
            realName.innerText = 'Username';
            profileName.innerText = 'Real name';
            avatarImg.src = `${avatarBucketPath}/default_avatar.png`;

            showForm(createLoginForm, '/auth/signin', 'Sign In');
        }
    } catch (error) {
        alert(error.message);
    }
}
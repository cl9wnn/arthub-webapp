import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm } from '../Auth/auth.js';

const uploadBtn = document.getElementById('uploadBtn');
const avatarInput = document.getElementById('avatarInput');
avatarInput.setAttribute('accept', 'image/jpeg, image/png, image/gif, image/webp');
const avatarPreview = document.getElementById('avatarPreview');
const nextBtn = document.getElementById('nextBtn');

let avatarFile = null;

uploadBtn.addEventListener('click', () => {
    avatarInput.click();
});


avatarInput.addEventListener('change', (event) => {
    const file = event.target.files[0];

    if (file) {
        avatarFile = file;

        const reader = new FileReader();
        reader.onload = function (e) {
            avatarPreview.style.backgroundImage = `url(${e.target.result})`;
            avatarPreview.style.backgroundSize = 'cover';
            avatarPreview.style.backgroundPosition = 'center';
            avatarPreview.textContent = '';
        };
        reader.readAsDataURL(file);
    }
});

nextBtn.addEventListener('click', async () => {
    const token = tokenStorage.get();

    if (!avatarFile) {
        alert('Please select an avatar before proceeding.');
        return;
    }

    const reader = new FileReader();
    reader.onload = async function (e) {
        
        const base64String = e.target.result.split(',')[1];
        const payload = JSON.stringify({
            contentType: avatarFile.type,
            fileData: base64String
        });

        try {
            const response = await fetch('/api/save-avatar', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: payload
            });

            const data = await response.json();

            if (response.ok) {
                window.location.href = '/account';
            } else {
                showForm(createLoginForm, '/auth/signin', 'Sign In');
            }
        } catch (error) {
            alert(error.message);
        }
    };

    reader.readAsDataURL(avatarFile);
});

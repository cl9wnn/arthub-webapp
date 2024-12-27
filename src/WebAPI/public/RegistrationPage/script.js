import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm, createElement} from '../Auth/auth.js';

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
const createErrorField = () => createElement('span', { className: 'error-message' });

const validateFields = (data) => {
    const errors = {};

    if (!/^[a-zA-Z0-9_]{5,20}$/.test(data.login)) {
        errors.login = "Логин должен быть от 5 до 20 символов и содержать только латинские буквы, цифры и символы подчеркивания.";
    }

    if (!/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$/.test(data.password)) {
        errors.password = "Пароль должен быть от 8 до 20 символов, содержать минимум одну букву, одну цифру и один специальный символ.";
    }

    return errors;
};

document.getElementById('submit-button').addEventListener('click', async () => {
    const login = document.getElementById('login').value;
    const password = document.getElementById('password').value;
    const profileName = document.getElementById('profile-name').value;
    const realName = document.getElementById('real-name').value;

    if (!login || !password || !profileName || !realName) {
        alert('All fields are required!');
        return;
    }

    if (!avatarFile) {
        alert('Please select an avatar before proceeding.');
        return;
    }

    const reader = new FileReader();
    reader.onload = async function (e) {
        const base64String = e.target.result.split(',')[1];
        const payload = JSON.stringify({
            user: {
                login: login,
                password: password,
                profileName: profileName,
                realName: realName,
            },
            avatar: {
                contentType: avatarFile.type,
                fileData: base64String
            }
        });

        try {
            const response = await fetch('/auth/signup', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: payload
            });

            const data = await response.json();
            console.log(data);
            
            if (response.ok) {
                tokenStorage.save(data.token);
                window.location.href = '/account';
            } else {
                alert(data.error || 'Ошибка при выполнении запроса');
            }
        } catch (error) {
            alert(error.message);
        }
    };

    reader.readAsDataURL(avatarFile);
});
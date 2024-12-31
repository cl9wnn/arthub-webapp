import {tokenStorage, createElement} from '../Auth/auth.js';

const uploadBtn = document.getElementById('uploadBtn');
const avatarInput = document.getElementById('avatarInput');
const avatarPreview = document.getElementById('avatarPreview');
const sendBtn = document.getElementById('sendBtn');
const backBtn = document.getElementById('backBtn');
avatarInput.setAttribute('accept', 'image/jpeg, image/png, image/gif, image/webp');


backBtn.addEventListener('click', () => {
    window.location.href = '/';
});
let avatarFile = null;

// загрузка аватарки из директории
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

// валидация полей формы
const createErrorField = () => createElement('span', { className: 'error-message' });

const validateFields = (data) => {
    const errors = {};

    if (!/^[a-zA-Z0-9_]{5,20}$/.test(data.login)) {
        errors.login = "Логин должен быть от 5 до 20 символов и содержать только латинские буквы, цифры и символы подчеркивания.";
    }

    if (!/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$/.test(data.password)) {
        errors.password = "Пароль должен быть от 8 до 20 символов, содержать минимум одну букву, одну цифру и один специальный символ.";
    }
    
    if (!/^[a-zA-Z0-9_.]{3,15}$/.test(data.profileName)) {
        errors.profileName = "Имя пользователя должно быть от 3 до 15 символов и содержать только латинские буквы, цифры, точки или символы подчеркивания.";
    }
    if (!data.country) {
        errors.country = "Выберите вашу страну";
    }
    
    return errors;
};

const displayErrors = (errors) => {
    document.querySelectorAll('.error-message').forEach(span => {
        span.textContent = '';
        span.classList.remove('error-highlight');
    });

    for (const [field, message] of Object.entries(errors)) {
        const errorSpan = document.getElementById(`${field}-error`);
        if (errorSpan) {
            errorSpan.textContent = message;

            errorSpan.classList.add('error-highlight');

            setTimeout(() => errorSpan.classList.remove('error-highlight'), 300);
        }
    }
};

// отправка формы регистрации на сервер
sendBtn.addEventListener('click', async () => {
    const login = document.getElementById('login').value;
    const password = document.getElementById('password').value;
    const profileName = document.getElementById('profile-name').value;
    const country = document.getElementById('country').value;


    const formData = { login, password, profileName, country };
    const errors = validateFields(formData);
    
    if (!avatarFile) {
        errors.avatar = "Выберите ваш аватар";
    }

    if (Object.keys(errors).length > 0) {
        displayErrors(errors);
        return;
    }
    
    const reader = new FileReader();
    reader.onload = async function (e) {
        const base64String = e.target.result.split(',')[1];
        const payload = JSON.stringify({
            user: formData,
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
            
            if (response.ok) {
                tokenStorage.save(data.token);
                window.location.href = '/account';
            } else {
                alert(data || 'Ошибка при выполнении запроса');
            }
        } catch (error) {
            alert(error.message);
        }
    };
    reader.readAsDataURL(avatarFile);
});
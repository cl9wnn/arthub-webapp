import {tokenStorage, showForm, createLoginForm } from '../Auth/auth.js';


const sendBtn = document.getElementById('sendBtn');
const backBtn = document.getElementById('backBtn');


backBtn.addEventListener('click', () => {
    window.location.href = '/';
});


// валидация полей формы
const validateFields = (data) => {
    const errors = {};
    
    if (!/^[a-zA-Zа-яА-ЯёЁ '-]{3,30}$/.test(data.fullname)) {
        errors.fullname = "Ваше имя должно быть от 3 до 50 символов и содержать только буквы, пробелы, дефисы или апострофы.";
    }

    if (
        !/^(\+7|8)\d{10}$/.test(data.contactInfo) &&
        !/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(data.contactInfo) &&
        !/^(https?:\/\/)?([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}\/?.*$/.test(data.contactInfo)
    ) {
        errors.contactInfo = "Введите корректный номер телефона, почту, или ссылку";
    }

    if (!/^[a-zA-Zа-яА-ЯёЁ0-9\s.,'"-]{10,300}$/.test(data.summary)) {
        errors.summary = "Описание должно быть от 10 до 300 символов.";
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
// отправка формы регистрации художника на сервер
sendBtn.addEventListener('click', async () => {
    const fullname = document.getElementById('fullname').value;
    const contactInfo = document.getElementById('contactInfo').value;
    const summary = document.getElementById('summary').value;
    const token = tokenStorage.get();
    const formData = {fullname, contactInfo, summary };
    const errors = validateFields(formData);

    if (Object.keys(errors).length > 0) {
        displayErrors(errors);
        return;
    }
        try {
            const response = await fetch('/api/upgrade-user', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(formData)
            });

            const data = await response.json();

            if (response.ok) {
                tokenStorage.save(data.token);
                window.location.href = '/account';
            } else {
                if (data == 'Not authorized') {
                    showForm(createLoginForm, '/auth/signin', 'Sign In');
                }
                else{
                    alert (data || 'Ошибка на сервере!');
                }
            }
        } catch (error) {
            alert(error.message);
        }
});
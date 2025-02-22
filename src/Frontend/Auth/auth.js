﻿export const tokenStorage = {
    save: (token) => localStorage.setItem('jwtToken', token),
    get: () => localStorage.getItem('jwtToken'),
    remove: () => localStorage.removeItem('jwtToken')
};

export const createElement = (tag, options = {}) => {
    const element = document.createElement(tag);
    Object.entries(options).forEach(([key, value]) => {
        if (key === 'className') element.className = value;
        else if (key === 'textContent') element.textContent = value;
        else if (key.startsWith('on')) element.addEventListener(key.slice(2).toLowerCase(), value);
        else element[key] = value;
    });
    return element;
};

const createOverlay = () => createElement('div', { className: 'overlay' });

const createPopup = (onClose) => {
    const popup = createElement('div', { className: 'popup' });
    const closeButton = createElement('button', {
        textContent: '×',
        className: 'close-button',
        onClick: () => {
            window.location.href = '/';
            document.body.removeChild(document.querySelector('.overlay'));
            if (onClose) onClose();
        }
    });
    popup.appendChild(closeButton);
    return popup;
};

export const showForm = (createFormMethod, path, buttonText) => {
    return new Promise((resolve) => {
        const overlay = createOverlay();
        const popup = createPopup(() => {
            document.body.removeChild(overlay);
            resolve(false); 
        });

        const form = createFormMethod(() => {
            document.body.removeChild(overlay);
            resolve(true); 
        }, path, buttonText);

        popup.appendChild(form);
        overlay.appendChild(popup);
        document.body.appendChild(overlay);
    });
};

const createInputField = (type, name, placeholder) =>
    createElement('input', { type, name, placeholder, required: true, className: 'input-field' });


export const createLoginForm = (onClose, path) => {
    const form = createElement('form');

    const nameField = createInputField('text', 'login', 'Введите логин');
    const passwordField = createInputField('password', 'password', 'Введите пароль');
    
    const submitButton = createElement('button', {
        type: 'submit',
        textContent: 'Войти',
        className: 'submit-button'
    });

    const registerText = createElement('p', {
        textContent: 'Еще не зарегистрированы?',
        className: 'register-text'
    });

    const registerLink = createElement('a', {
        textContent: 'Зарегистрироваться',
        href: '/register-account',
        className: 'register-link'
    });

    [nameField, passwordField, submitButton, registerText, registerLink].forEach(el => form.appendChild(el));

    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        
        const formData = Object.fromEntries(new FormData(form).entries());
        await handleSubmit('/auth/signin', formData);
        onClose();
    });

    return form;
};


export function parseJwtToSub(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        const playload = JSON.parse(jsonPayload);
        return playload.Sub || null;
    } catch (error) {
        console.error('Invalid token', error);
        return null;
    }
}

export function parseJwtToRole(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );
        const payload = JSON.parse(jsonPayload);
        return payload.Role || null;  
    } catch (error) {
        console.error('Invalid token', error);
        return null;
    }
}


const handleSubmit = async (path, data) => {
    try {
        const response = await fetch(path, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error((await response.json()) || 'Ошибка при выполнении запроса');
        }

        const { token } = await response.json();
        tokenStorage.save(token);
    }
    catch (error) {
        alert(error.message);
    }
};


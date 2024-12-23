document.addEventListener('DOMContentLoaded', () => {
    const setupButton = (id, createFormMethod, path, buttonText) => {
        document.getElementById(id).addEventListener('click', (event) => {
            event.preventDefault();
            showForm(createFormMethod, path, buttonText);
        });
    };

    setupButton('signupBtn', createRegistrationForm, '/auth/signup', 'Sign up');
    setupButton('signinBtn', createLoginForm, '/auth/signin', 'Sign in');
});


const button = document.getElementById('accountBtn');
button.addEventListener('click', () => {
    window.location.href = '/account-settings';
});

export const tokenStorage = {
    save: (token) => localStorage.setItem('jwtToken', token),
    get: () => localStorage.getItem('jwtToken'),
    remove: () => localStorage.removeItem('jwtToken')
};

const createElement = (tag, options = {}) => {
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
            document.body.removeChild(document.querySelector('.overlay'));
            if (onClose) onClose();
        }
    });
    popup.appendChild(closeButton);
    return popup;
};

const showForm = (createFormMethod, path, buttonText) => {
    const overlay = createOverlay();
    const popup = createPopup(() => document.body.removeChild(overlay));

    const form = createFormMethod(() => document.body.removeChild(overlay), path, buttonText);
    popup.appendChild(form);
    overlay.appendChild(popup);

    document.body.appendChild(overlay);
};

const createInputField = (type, name, placeholder) =>
    createElement('input', { type, name, placeholder, required: true, className: 'input-field' });

const createErrorField = () => createElement('span', { className: 'error-message' });

const createForm = (path, buttonText, onSubmitHandler, validate = false) => {
    const form = createElement('form');

    const nameField = createInputField('text', 'login', 'Введите логин');
    const passwordField = createInputField('password', 'password', 'Введите пароль');

    const nameError = createErrorField();
    const passwordError = createErrorField();

    const submitButton = createElement('button', {
        type: 'submit',
        textContent: buttonText,
        className: 'submit-button'
    });

    [nameField, nameError, passwordField, passwordError, submitButton].forEach(el => form.appendChild(el));

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        nameError.textContent = '';
        passwordError.textContent = '';

        const formData = Object.fromEntries(new FormData(form).entries());

        if (validate) {
            const errors = validateFields(formData);
            if (Object.keys(errors).length > 0) {
                nameError.textContent = errors.login || '';
                passwordError.textContent = errors.password || '';
                return;
            }
        }
        await onSubmitHandler(formData, path);
    });

    return form;
};

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

const createRegistrationForm = (onClose, path) =>
    createForm(path, 'Зарегистрироваться', async (data) => {
        await handleSubmit('/auth/signup', data);
        onClose();
    }, true);

const createLoginForm = (onClose, path) =>
    createForm(path, 'Войти', async (data) => {
        await handleSubmit('/auth/signin', data);
        onClose();
    });

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

        document.getElementById('accountBtn').style.display = 'block';
        document.getElementById('signupBtn').style.display = 'none';
        document.getElementById('signinBtn').style.display = 'none';

        alert('Успешно!');
    } catch (error) {
        alert(error.message);
    }
};

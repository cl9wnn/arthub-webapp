export const someVariable = 'Hello'

document.getElementById('signup').addEventListener('click', (event) => {
    event.preventDefault();
    showForm(createRegistrationForm,"/auth/signup", "Sign up");
});

document.getElementById('signin').addEventListener('click', (event) => {
    event.preventDefault();
    showForm(createLoginForm,"/auth/signin", "Sign in");
});

function saveToken(token) {
    localStorage.setItem('jwtToken', token);
}

export function getToken() {
    return localStorage.getItem('jwtToken');
}

function removeToken() {
    localStorage.removeItem('jwtToken');
}

function createOverlay() {
    const overlay = document.createElement('div');
    overlay.className = "overlay";
    return overlay;
}

function createPopup() {
    const popup = document.createElement('div');
    popup.className = "popup";

    const closeButton = document.createElement('button');
    closeButton.textContent = "×";
    closeButton.className = "close-button";
    closeButton.addEventListener('click', () => {
        document.body.removeChild(document.querySelector('.overlay'));
    });

    popup.appendChild(closeButton);
    return popup;
}

function showForm(createFormMethod, path, bttnText) {
    const overlay = createOverlay();
    const popup = createPopup();

    const form = createFormMethod(
        () => document.body.removeChild(overlay),
        path,
        bttnText
    );
    popup.appendChild(form);

    overlay.appendChild(popup);
    document.body.appendChild(overlay);
}
function createInputField(type, name, placeholder) {
    const input = document.createElement('input');
    input.type = type;
    input.name = name;
    input.placeholder = placeholder;
    input.required = true;
    input.className = "input-field";
    return input;
}

function createErrorField() {
    const errorField = document.createElement('span');
    errorField.className = 'error-message';
    errorField.textContent = '';
    return errorField;
}

function createRegistrationForm(onClose, path) {
    return createForm(path, 'Зарегистрироваться', async (data, path) => {
        await handleSubmitSignUp(data, path);
        onClose();
    }, true);
}

function createLoginForm(onClose, path) {
    return createForm(path, 'Войти', async (data, path) => {
        await handleSubmitSignIn(data, path);
        onClose();
    });
}

function createForm(path, bttnText, onSubmitHandler, validate = false) {
    const form = document.createElement('form');

    const nameField = createInputField('text', 'login', 'Введите логин');
    const passwordField = createInputField('password', 'password', 'Введите пароль');

    const nameError = createErrorField();
    const passwordError = createErrorField();

    const submitButton = document.createElement('button');
    submitButton.type = "submit";
    submitButton.textContent = bttnText;
    submitButton.className = "submit-button";

    form.appendChild(nameField);
    form.appendChild(nameError);
    form.appendChild(passwordField);
    form.appendChild(passwordError);
    form.appendChild(submitButton);

    form.addEventListener('submit', async (event) => {
        event.preventDefault();

        nameError.textContent = '';
        passwordError.textContent = '';

        const formData = new FormData(form);
        const data = Object.fromEntries(formData.entries());

        if (validate) {
            const errors = validateFields(data);

            if (Object.keys(errors).length > 0) {
                if (errors.login) {
                    nameError.textContent = errors.login;
                }

                if (errors.password) {
                    passwordError.textContent = errors.password;
                }
                return;
            }
        }
        await onSubmitHandler(data, path);
    });

    return form;
}


function validateFields(data) {
    const errors = {};

    const loginRegex = /^[a-zA-Z0-9_]{5,20}$/;
    if (!data.login || !loginRegex.test(data.login)) {
        errors.login = "Логин должен быть от 5 до 20 символов и содержать только латинские буквы, цифры и символы подчеркивания.";
    }

    const passwordRegex = /^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$/;
    if (!data.password || !passwordRegex.test(data.password)) {
        errors.password = "Пароль должен быть от 8 до 20 символов, содержать минимум одну букву, одну цифру и один специальный символ.";
    }

    return errors;
}
async function handleSubmitSignUp(data, path) {
    try {
        const response = await fetch(path, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            const result = await response.json();
            throw new Error(result);
        }

        alert('Успешно!');
    } catch (error) {
        alert(error.message || 'Произошла ошибка');
    }
}

async function handleSubmitSignIn(data, path) {
    try {
        const response = await fetch(path, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        });

        if (!response.ok) {
            const result = await response.json();
            throw new Error(result);
        }

        const responseData = await response.json();
        saveToken(responseData.token);
        alert("Успешно!")
    } catch (error) {
        alert(error.message || 'Произошла ошибка');
    }
}
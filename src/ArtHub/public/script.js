import { tokenStorage } from './auth.js';

document.addEventListener("DOMContentLoaded", () => {
    toggleVisibility(tokenStorage.get());
});

document.getElementById('market').addEventListener('click', async (event) => {
    event.preventDefault();
    await handleMarketRequest();
});

function toggleVisibility(token) {
    const accountSection = document.getElementById("account");
    const signupSection = document.getElementById("signup");
    const signinSection = document.getElementById("signin");

    if (token) {
        accountSection.style.display = "block";
        signupSection.style.display = "none";
        signinSection.style.display = "none";
    } else {
        accountSection.style.display = "none";
        signupSection.style.display = "block";
        signinSection.style.display = "block";
    }
}

async function handleMarketRequest() {
    try {
        const result = await fetchMarketData();
        alert('Успешно!');
        console.log('Response:', result);
    } catch (error) {
        document.getElementById("signin").style.display = "block";
        alert(error.message || 'Произошла ошибка');
    }
}

async function fetchMarketData() {
    const token = tokenStorage.get();

    if (!token) {
        throw new Error('Токен отсутствует. Пожалуйста, войдите в систему.');
    }

    const response = await fetch('/test', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData || 'Ошибка сервера');
    }

    return await response.json();
}
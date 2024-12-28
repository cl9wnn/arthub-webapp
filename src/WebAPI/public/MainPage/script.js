import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm } from '../Auth/auth.js';

const accountSection = document.getElementById("accountBtn");
const signupSection = document.getElementById("signupBtn");
const signinSection = document.getElementById("signinBtn");
const marketSection = document.getElementById("marketBtn");

document.addEventListener('DOMContentLoaded', () => {
    const setupButton = (id, createFormMethod, path, buttonText) => {
        document.getElementById(id).addEventListener('click', (event) => {
            event.preventDefault();
            showForm(createFormMethod, path, buttonText);
        });
    };

    setupButton('signinBtn', createLoginForm, '/auth/signin', 'Sign in');
    toggleVisibility(tokenStorage.get());
});

document.getElementById('signupBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/register-account';
});

document.getElementById('marketBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/market'; 
});

document.getElementById('accountBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/account';
});

function toggleVisibility(token) {
    if (token) {
        accountSection.style.display = "block";
        marketSection.style.display = "block";
        signupSection.style.display = "none";
        signinSection.style.display = "none";
    } else {
        marketSection.style.display = "none";
        accountSection.style.display = "none";
        signupSection.style.display = "block";
        signinSection.style.display = "block";
    }
}

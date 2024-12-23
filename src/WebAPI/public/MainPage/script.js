import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm, createRegistrationForm } from '../Auth/auth.js';

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

    setupButton('signupBtn', createRegistrationForm, '/auth/signup', 'Sign up');
    setupButton('signinBtn', createLoginForm, '/auth/signin', 'Sign in');

    toggleVisibility(tokenStorage.get());
});


document.getElementById('marketBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/market'; 
});

document.getElementById('accountBtn').addEventListener('click', async (event) => {
    event.preventDefault();
    window.location.href = '/account-settings';
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

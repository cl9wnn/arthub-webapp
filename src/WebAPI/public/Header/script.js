import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm, parseJwtToSub, parseJwtToRole} from '../Auth/auth.js';

document.addEventListener('DOMContentLoaded', () => {
    setupHeaderButtons();
});

function setupHeaderButtons() {
    const headerButtonsContainer = document.querySelector('.header-buttons');
    
    headerButtonsContainer.innerHTML = '';
    
    if (tokenStorage.get()) {
        createLoggedInButtons(headerButtonsContainer);
    } else {
        createLoggedOutButtons(headerButtonsContainer);
    }
}

function createLoggedInButtons(container) {
    const accountButton = createButton('btnImg', 'accountBtn');
    const savingsButton = createButton('btnImg', 'savingsBtn');

    const token = tokenStorage.get();
    if (token) {
        const userRole = parseJwtToRole(token);
        console.log(userRole);
        
        if (userRole === 'artist') {
            const marketButton = createButton('btnImg', 'marketBtn');
            container.append(marketButton);
            marketButton.addEventListener('click', () => {
                window.location.href = '/market';
            });
        }
    }

    container.append(accountButton, savingsButton);

    const dropdownMenu = document.createElement('div');
    dropdownMenu.classList.add('dropdown-menu');
    const logoutLink = document.createElement('a');
    logoutLink.href = '#';
    logoutLink.textContent = 'Log Out';
    logoutLink.addEventListener('click', () => {
        tokenStorage.remove();
        setupHeaderButtons();
        window.location.href = '/';
    });

    const switchAccountLink = document.createElement('a');
    switchAccountLink.href = '#';
    switchAccountLink.textContent = 'Switch Account';
    switchAccountLink.addEventListener('click', async () => {
        tokenStorage.remove();
        await handleSignIn();
    });

    dropdownMenu.append(logoutLink, switchAccountLink);
    accountButton.append(dropdownMenu);

    accountButton.addEventListener('click', () => {
        if (token) {
            const userId = parseJwtToSub(token); 
            window.location.href = `/account/${userId}`;
        } else {
            console.error('User is not logged in.');
        }
    });

    savingsButton.addEventListener('click', () => {
        window.location.href = '/savings';
    });
}

function createLoggedOutButtons(container) {
    const signupButton = createButton('btnText', 'signupBtn', 'Sign Up');
    const signinButton = createButton('btnText', 'signinBtn', 'Sign In');

    container.append(signupButton, signinButton);

    signupButton.addEventListener('click', () => {
        window.location.href = '/register-account';
    });

    signinButton.addEventListener('click', async (event) => {
        event.preventDefault();
        await handleSignIn();
    });
}

function createButton(className, id, text = '') {
    const button = document.createElement('button');
    button.classList.add(className);
    button.id = id;
    button.textContent = text;
    return button;
}

async function handleSignIn() {
    const success = await showForm(createLoginForm, '/auth/signin', 'Sign in');
    if (success) {
        setupHeaderButtons(); 
        await loadArtworkList();
    }
}
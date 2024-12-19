import { getToken } from './auth.js';

document.getElementById('market').addEventListener('click', async (event) => {
    event.preventDefault();
    await fetchToMarket();
});

async function fetchToMarket() {
    try {
        const response = await fetch('/test', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getToken()}`
            }
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData || 'Ошибка сервера');
        }
        
        const result = await response.json(); 
        alert('Успешно!');
        console.log('Response:', result);
        
    } catch (error) {
        alert(error || 'Произошла ошибка');
    }
}




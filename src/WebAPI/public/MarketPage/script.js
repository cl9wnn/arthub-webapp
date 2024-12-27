import { tokenStorage } from '../Auth/auth.js';
import { showForm, createLoginForm } from '../Auth/auth.js';

const testBtn = document.getElementById('testBuy');


testBtn.addEventListener('click', async () => {
    const token = tokenStorage.get();
    
        try {
            const response = await fetch('/api/buy', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
            });

            const data = await response.json();

            if (response.ok) {
                console.log(data);
            } else {
                showForm(createLoginForm, '/auth/signin', 'Sign In');
            }
        } catch (error) {
            alert(error.message);
        }
});

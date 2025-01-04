import {tokenStorage, showForm, createLoginForm } from '../Auth/auth.js';

const testBtn = document.getElementById('testBuy');


testBtn.addEventListener('click', async () => {
    const token = tokenStorage.get();
    
        try {
            const response = await fetch('/api/buy', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
            });

            const data = await response.json();

            if (response.ok) {
                alert(data);
            } 
            else if (response.status === 401){
                showForm(createLoginForm, '/auth/signin', 'Sign In');
            }
            else{
                    alert (data || 'Ошибка на сервере!');
            }
        } catch (error) {
            alert(error.message);
        }
});

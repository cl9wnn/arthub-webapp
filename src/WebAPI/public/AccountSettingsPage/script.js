const uploadBtn = document.getElementById('uploadBtn');
const avatarInput = document.getElementById('avatarInput');
const avatarPreview = document.getElementById('avatarPreview');
const nextBtn = document.getElementById('nextBtn');

let avatarFile = null;

uploadBtn.addEventListener('click', () => {
    avatarInput.click();
});


// Обработка выбора файла
avatarInput.addEventListener('change', (event) => {
    const file = event.target.files[0];

    if (file) {
        const validImageTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
        if (!validImageTypes.includes(file.type)) {
            avatarPreview.textContent = 'Ошибка: Выберите файл изображения!';
            avatarPreview.style.backgroundImage = '';
            return;
        }

        avatarFile = file;

        const reader = new FileReader();
        reader.onload = function (e) {
            avatarPreview.style.backgroundImage = `url(${e.target.result})`;
            avatarPreview.style.backgroundSize = 'cover';
            avatarPreview.style.backgroundPosition = 'center';
            avatarPreview.textContent = '';
        };
        reader.readAsDataURL(file);
    }
});

// Обработка отправки файла
nextBtn.addEventListener('click', async () => {
    if (!avatarFile) {
        alert('Please select an avatar before proceeding.');
        return;
    }

    const reader = new FileReader();
    reader.onload = async function (e) {
        const base64String = e.target.result.split(',')[1];
        const payload = JSON.stringify({
            contentType: avatarFile.type,
            fileData: base64String
        });

        try {
            const response = await fetch('/api/save-avatar', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: payload
            });

            if (response.ok) {
                alert('Avatar uploaded successfully!');
            } else {
                alert('Failed to upload avatar. Please try again.');
            }
        } catch (error) {
            console.error('Error uploading avatar:', error);
            alert('An error occurred while uploading the avatar.');
        }
    };

    reader.readAsDataURL(avatarFile);
});
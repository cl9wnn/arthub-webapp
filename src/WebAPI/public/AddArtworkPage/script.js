import {createLoginForm, parseJwtToSub, showForm, tokenStorage} from "../Auth/auth.js";


const maxTags = 3;
const tags = document.querySelectorAll(".tag");
tags.forEach(tag => {
    tag.addEventListener("click", () => handleTagClick(tag, maxTags));
});
const backBtn = document.getElementById('backBtn');

backBtn.addEventListener('click', () => {
    window.history.back();
});

function handleTagClick(tag, maxTags) {
    const selectedTags = document.querySelectorAll(".tag.selected");

    if (tag.classList.contains("selected")) {
        tag.classList.remove("selected");
    } else if (selectedTags.length < maxTags) {
        tag.classList.add("selected");
    } else {
        const errorElement = document.getElementById("tags-error");
        errorElement.textContent = `You can select up to ${maxTags} tags.`;
    }
}

// валидация полей формы
const validateFields = (data) => {
    const errors = {};

    if (!/^[a-zA-Zа-яА-ЯёЁ '-]{3,60}$/.test(data.title)) {
        errors.title = "Название должно быть от 3 до 60 символов и содержать только буквы, пробелы, дефисы или апострофы.";
    }

    if (!/^[a-zA-Zа-яА-ЯёЁ0-9\s.,'"-]{10,300}$/.test(data.description)) {
        errors.description = "Описание должно быть от 10 до 300 символов.";
    }

    if (!data.category){
        errors.category = "Выберите категорию или жанр работы!";
    }
    if (data.tags.length === 0){
        errors.tags = "Выберите хотя бы 1 тег";
    }

    return errors;
};

const displayErrors = (errors) => {
    document.querySelectorAll('.error-message').forEach(span => {
        span.textContent = '';
        span.classList.remove('error-highlight');
    });

    for (const [field, message] of Object.entries(errors)) {
        const errorSpan = document.getElementById(`${field}-error`);
        if (errorSpan) {
            errorSpan.textContent = message;

            errorSpan.classList.add('error-highlight');

            setTimeout(() => errorSpan.classList.remove('error-highlight'), 300);
        }
    }
};

// загрузка арта из директории
const uploadBtn = document.getElementById('uploadBtn');
const artInput = document.getElementById('artInput');
const artPreview = document.getElementById('artPreview');
artInput.setAttribute('accept', 'image/jpeg, image/png, image/gif, image/webp');

let artFile = null;

uploadBtn.addEventListener('click', () => {
    artInput.click();
});

artInput.addEventListener('change', (event) => {
    const file = event.target.files[0];

    if (file) {
        artFile = file;

        const reader = new FileReader();
        reader.onload = function (e) {
            const img = new Image();
            img.onload = function () {
                const aspectRatio = img.height / img.width;

                artPreview.style.height = `${artPreview.offsetWidth * aspectRatio}px`;
                artPreview.style.backgroundImage = `url(${e.target.result})`;
                artPreview.style.backgroundSize = 'cover';
                artPreview.style.backgroundPosition = 'center';
                artPreview.textContent = '';
            };
            img.src = e.target.result; 
        };
        reader.readAsDataURL(file);
    }
});

// Обработчик кнопки добавления арта
document.getElementById('sendBtn').addEventListener('click', async () => {
    const token = tokenStorage.get();

    const title = document.getElementById('title').value;
    const category = document.getElementById('category').value;
    const description = document.getElementById('description').value;
    const tags = Array.from(document.querySelectorAll('.tag.selected'))
        .map(tag => tag.textContent.trim());

    const formData = { title, category, description, tags };
    const errors = validateFields(formData);

    if (!artFile) {
        errors.art = "Выберите ваш арт";
    }

    if (Object.keys(errors).length > 0) {
        displayErrors(errors);
        return;
    }

    const reader = new FileReader();
    reader.onload = async function (e) {
        const base64String = e.target.result.split(',')[1];
        const payload = JSON.stringify({
            title: formData.title,
            category: formData.category,
            description: formData.description,
            tags: formData.tags,
            artFile: {
                contentType: artFile.type,
                fileData: base64String
            }
        });

        try {
            const response = await fetch('/api/add-artwork', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: payload
            });
            
            if (response.ok) {
                const userId = parseJwtToSub(token);
                window.location.href = `/account/${userId}`;
            }
            else if (response.status === 401) {
                 await showForm(createLoginForm, '/auth/signin', 'Sign In');
                }
            else{
                    throw new Error(data || 'Ошибка на сервере!');
            }
        } catch (error) {
            alert(error);
        }
    };

    reader.readAsDataURL(artFile);
});


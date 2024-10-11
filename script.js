const apiUrl = 'https://api.github.com/repos/KeoLotso/KeosRandomStuff/contents'; 
let currentFiles = [];
let currentFolder = '';
let currentSortMode = 'A-Z';

async function fetchRepoContents() {
    try {
        const response = await fetch(apiUrl);
        const files = await response.json();
        sortFilesIntoFolders(files);
    } catch (error) {
        console.error('Error fetching repository contents:', error);
    }
}

function sortFilesIntoFolders(files) {
    const soundExtensions = ['mp3', 'wav'];
    const modelExtensions = ['fbx', 'obj', 'blend'];
    const imageExtensions = ['png', 'jpeg', 'jpg'];
    const scriptExtensions = ['cs'];

    const sounds = [];
    const models = [];
    const scripts = [];
    const images = [];

    files.forEach(file => {
        const extension = file.name.split('.').pop().toLowerCase();
        if (!['html', 'js', 'css'].includes(extension)) {
            if (soundExtensions.includes(extension)) {
                sounds.push(file);
            } else if (modelExtensions.includes(extension)) {
                models.push(file);
            } else if (imageExtensions.includes(extension)) {
                images.push(file); 
            } else if (scriptExtensions.includes(extension)) {
                scripts.push(file);
            }
        }
    });

    displayFolders(sounds, models, scripts);
    displayFiles(images);
}

function displayFolders(sounds, models, scripts) {
    const folderList = document.getElementById('folder-list');
    folderList.innerHTML = '';

    if (sounds.length > 0) {
        createFolder(folderList, 'Sounds', sounds, 'sounds-icon.png');
    }
    if (models.length > 0) {
        createFolder(folderList, '3D Models', models, 'models-icon.png');
    }
    if (scripts.length > 0) {
        createFolder(folderList, 'Scripts', scripts, 'scripts-icon.png');
    }
}

function createFolder(container, folderName, files, iconPath) {
    const folder = document.createElement('div');
    folder.classList.add('folder-item');
    folder.innerHTML = `<img src="${iconPath}" alt="${folderName} Icon" class="folder-icon" /><h2>${folderName} (${files.length})</h2>`;
    folder.addEventListener('click', () => openFolder(folderName, files));
    container.appendChild(folder);
}

function openFolder(folderName, files) {
    currentFolder = folderName;
    currentFiles = files;

    const folderList = document.getElementById('folder-list');
    const fileList = document.getElementById('file-list');
    const backButton = document.getElementById('back-button');
    const searchBar = document.getElementById('search-bar');
    const sortOptions = document.getElementById('sort-options');

    folderList.style.display = 'none';
    fileList.style.display = 'block';
    backButton.style.display = 'inline-block';
    searchBar.style.display = 'inline-block';
    sortOptions.style.display = 'none'; // Hide sort bulshit another time
    fileList.innerHTML = `<h2>${folderName}</h2>`;

    displayFiles(currentFiles);
}

function displayFiles(files) {
    const fileList = document.getElementById('file-list');
    fileList.innerHTML = `<h2>${currentFolder}</h2>`;

    files = sortFiles(files);

    files.forEach(file => {
        const fileItem = document.createElement('div');
        fileItem.classList.add('file-item');
        const extension = file.name.split('.').pop().toLowerCase();

        if (['mp3', 'wav'].includes(extension)) {
            fileItem.innerHTML = `
                <div class="file-content">
                    <p>${file.name}</p>
                    <audio controls>
                        <source src="${file.download_url}" type="audio/${extension}">
                    </audio>
                    <a href="${file.download_url}" download class="download-btn">Download</a>
                </div>
            `;
        } else {
            fileItem.innerHTML = `
                <div class="file-content">
                    <p>${file.name}</p>
                    <a href="${file.download_url}" download class="download-btn">Download</a>
                </div>
            `;
        }

        fileList.appendChild(fileItem);
    });
}

function sortFiles(files) {
    switch (currentSortMode) {
        case 'A-Z':
            return files.sort((a, b) => a.name.localeCompare(b.name));
        case 'Z-A':
            return files.sort((a, b) => b.name.localeCompare(a.name));
        default:
            return files;
    }
}

document.getElementById('sort-options').style.display = 'none'; // Hide sort Bulshity thingys yk

document.getElementById('search-bar').addEventListener('input', (event) => {
    const searchTerm = event.target.value.toLowerCase();
    const filteredFiles = currentFiles.filter(file => file.name.toLowerCase().includes(searchTerm));
    displayFiles(filteredFiles);
});

document.getElementById('back-button').addEventListener('click', () => {
    const folderList = document.getElementById('folder-list');
    const fileList = document.getElementById('file-list');
    const backButton = document.getElementById('back-button');
    const searchBar = document.getElementById('search-bar');

    folderList.style.display = 'block';
    fileList.style.display = 'none';
    backButton.style.display = 'none';
    searchBar.style.display = 'none';
});

document.getElementById('theme-toggle').addEventListener('click', () => {
    document.body.classList.toggle('light-mode');
});

fetchRepoContents();

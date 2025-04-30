const login = () => {
    const username = document.getElementById('usernameInput').value;
    const password = document.getElementById('passwordInput').value;
    const errorMessage = document.getElementById('errorMessage');
    
    errorMessage.textContent = '';
    
    if (!username || !password) {
        errorMessage.textContent = 'Username and password are required';
        return;
    }
    
    fetch('/Auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Invalid username or password');
        }
        return response.json();
    })
    .then(_ => {
        window.location.href = '/index.html';
    })
    .catch(error => {
        errorMessage.textContent = error.message;
    });
};

document.getElementById('passwordInput').addEventListener('keydown', (event) => {
    if (event.key === 'Enter') {
        login();
    }
});
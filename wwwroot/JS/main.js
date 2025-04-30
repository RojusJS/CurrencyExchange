let currencies = [];
let favorites = [];

document.addEventListener('DOMContentLoaded', () => {
    verifyAuth();
    loadCurrencies()
        .then(() => {
            populateCurrencySelects();
            getExchangeRates();
            loadFavorites();
        });
});

function verifyAuth() {
    fetch('/Auth/verify', { method: 'GET' })
        .then(response => {
            if (!response.ok) {
                window.location.href = '/login.html';
                throw new Error('User not authenticated');
            }
        })
        .catch(() => {
            window.location.href = '/login.html';
        });
}

function logout() {
    fetch('/Auth/logout', { method: 'POST' })
        .then(() => {
            window.location.href = '/login.html';
        })
        .catch(error => {
            console.error('Logout failed:', error);
            alert('Failed to logout. Please try again.');
        });
}

async function loadCurrencies() {
    try {
        const response = await fetch('/api/currencies');
        if (!response.ok) throw new Error('Failed to load currencies');
        
        currencies = await response.json();
        return currencies;
    } catch (error) {
        console.error('Error loading currencies:', error);
        alert('Failed to load currency data. Please refresh the page.');
    }
}

function populateCurrencySelects() {
    const selects = [
        document.getElementById('fromCurrency'),
        document.getElementById('toCurrency'),
        document.getElementById('newFavorite')
    ];
    
    currencies.sort((a, b) => a.code.localeCompare(b.code));
    
    selects.forEach((select, index) => {
        select.innerHTML = '';
        
        currencies.forEach(currency => {
            const option = document.createElement('option');
            option.value = currency.code;
            option.textContent = `${currency.code} - ${currency.name}`;
            select.appendChild(option);
        });
        
        if (index === 0) select.value = 'USD';
        if (index === 1) select.value = 'EUR';
    });
}

async function convertCurrency() {
    const amount = document.getElementById('amount').value;
    const from = document.getElementById('fromCurrency').value;
    const to = document.getElementById('toCurrency').value;
    const resultDiv = document.getElementById('result');
    const resultText = document.getElementById('conversionResult');
    const rateInfo = document.getElementById('exchangeRate');
    
    if (!amount || isNaN(amount) || amount <= 0) {
        alert('Please enter a valid amount');
        return;
    }
    
    try {
        const response = await fetch('/api/convert', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ from, to, amount: parseFloat(amount) })
        });
        
        if (!response.ok) throw new Error('Conversion failed');
        
        const data = await response.json();
        
        resultDiv.classList.remove('hidden');
        resultText.textContent = `${amount} ${from} = ${data.result.toFixed(4)} ${to}`;
        rateInfo.textContent = `1 ${from} = ${data.rate.toFixed(6)} ${to}`;
    } catch (error) {
        console.error('Conversion error:', error);
        alert('Failed to convert currency. Please try again.');
    }
}

function swapCurrencies() {
    const fromSelect = document.getElementById('fromCurrency');
    const toSelect = document.getElementById('toCurrency');
    
    const temp = fromSelect.value;
    fromSelect.value = toSelect.value;
    toSelect.value = temp;
}

async function getExchangeRates() {
    const ratesList = document.getElementById('ratesList');
    const loadingDiv = document.getElementById('ratesLoading');
    
    loadingDiv.style.display = 'block';
    ratesList.innerHTML = '';
    
    try {
        const response = await fetch('/api/rates');
        if (!response.ok) throw new Error('Failed to load rates');
        
        const data = await response.json();
        
        loadingDiv.style.display = 'none';
        
        Object.entries(data.rates)
            .sort((a, b) => a[0].localeCompare(b[0]))
            .forEach(([code, rate]) => {
                if (code !== 'EUR') {
                    const rateItem = document.createElement('div');
                    rateItem.className = 'rate-item';
                    rateItem.innerHTML = `
                        <span>${code}</span>
                        <span>${rate.toFixed(4)}</span>
                    `;
                    ratesList.appendChild(rateItem);
                }
            });
    } catch (error) {
        console.error('Error loading rates:', error);
        loadingDiv.style.display = 'none';
        ratesList.innerHTML = '<p class="error">Failed to load exchange rates</p>';
    }
}

async function loadFavorites() {
    const favoritesList = document.getElementById('favoritesList');
    const loadingDiv = document.getElementById('favoritesLoading');
    
    loadingDiv.style.display = 'block';
    favoritesList.innerHTML = '';
    
    try {
        const response = await fetch('/api/favorites');
        if (!response.ok) throw new Error('Failed to load favorites');
        
        const data = await response.json();
        favorites = data.items;
        
        loadingDiv.style.display = 'none';
        
        if (favorites.length === 0) {
            favoritesList.innerHTML = '<p>No favorite currencies added yet.</p>';
            return;
        }
        
        favorites.forEach(favorite => {
            const favoriteItem = document.createElement('div');
            favoriteItem.className = 'favorite-item';
            favoriteItem.innerHTML = `
                <span>${favorite.currencyCode} - ${favorite.name}</span>
                <button class="remove-btn" onclick="removeFavorite('${favorite.id}')">Remove</button>
            `;
            favoritesList.appendChild(favoriteItem);
        });
    } catch (error) {
        console.error('Error loading favorites:', error);
        loadingDiv.style.display = 'none';
        favoritesList.innerHTML = '<p class="error">Failed to load favorites</p>';
    }
}

async function addFavorite() {
    const select = document.getElementById('newFavorite');
    const currencyCode = select.value;
    
    try {
        const response = await fetch('/api/favorites', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(currencyCode)
        });
        
        if (!response.ok) throw new Error('Failed to add favorite');
        
        await loadFavorites();
    } catch (error) {
        console.error('Error adding favorite:', error);
        alert('Failed to add favorite. Please try again.');
    }
}

async function removeFavorite(id) {
    try {
        const response = await fetch(`/api/favorites/${id}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) throw new Error('Failed to remove favorite');
        
        await loadFavorites();
    } catch (error) {
        console.error('Error removing favorite:', error);
        alert('Failed to remove favorite. Please try again.');
    }
}
using CurrencyExchange.Models;

namespace CurrencyExchange.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly List<FavoriteCurrency> _favorites = new();

        public List<FavoriteCurrency> GetFavorites(string username)
        {
            return _favorites
                .Where(f => f.Username == username)
                .ToList();
        }

        public FavoriteCurrency AddFavorite(string username, string currencyCode)
        {
            var existing = _favorites.FirstOrDefault(f => 
                f.Username == username && f.CurrencyCode == currencyCode);
                
            if (existing != null)
                return existing;
                
            var favorite = new FavoriteCurrency
            {
                Username = username,
                CurrencyCode = currencyCode,
                Id = Guid.NewGuid().ToString()
            };
            
            _favorites.Add(favorite);
            return favorite;
        }

        public bool RemoveFavorite(string username, string id)
        {
            var favorite = _favorites.FirstOrDefault(f => 
                f.Username == username && f.Id == id);
                
            if (favorite == null)
                return false;
                
            return _favorites.Remove(favorite);
        }
    }
}
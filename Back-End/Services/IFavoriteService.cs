using CurrencyExchange.Models;

namespace CurrencyExchange.Services
{
    public interface IFavoriteService
    {
        List<FavoriteCurrency> GetFavorites(string username);
        FavoriteCurrency AddFavorite(string username, string currencyCode);
        bool RemoveFavorite(string username, string id);
    }
}
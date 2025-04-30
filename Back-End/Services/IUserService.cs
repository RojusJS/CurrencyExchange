using CurrencyExchange.Models;

namespace CurrencyExchange.Services
{
    public interface IUserService
    {
        bool ValidateUser(string username, string password);
        string GenerateJwtToken(string username);
    }
}
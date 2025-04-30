using CurrencyExchange.Models;

namespace CurrencyExchange.Services
{
    public interface ICurrencyService
    {
        Task<List<Currency>> GetCurrenciesAsync();
        Task<ExchangeRate> GetExchangeRatesAsync(string baseCurrency = "USD");
        Task<ConversionResult> ConvertCurrencyAsync(ConversionRequest request);
    }
}
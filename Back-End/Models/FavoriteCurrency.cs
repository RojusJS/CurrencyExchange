namespace CurrencyExchange.Models
{
    public class FavoriteCurrency
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
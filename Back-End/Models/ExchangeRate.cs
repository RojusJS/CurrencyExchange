namespace CurrencyExchange.Models
{
    public class ExchangeRate
    {
        public string Base { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
        public DateTime Date { get; set; }
    }
}
using CurrencyExchange.Models;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Controllers
{
    [ApiController]
    [Route("api")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _currencyService.GetCurrenciesAsync();
            return Ok(currencies);
        }

        [HttpGet("rates")]
        public async Task<IActionResult> GetRates()
        {
        try
        {
        var rates = await _currencyService.GetExchangeRatesAsync("EUR");
        return Ok(rates);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
        }

        [Authorize]
        [HttpPost("convert")]
        public async Task<IActionResult> Convert([FromBody] ConversionRequest request)
        {
            if (string.IsNullOrEmpty(request.From) || 
                string.IsNullOrEmpty(request.To) ||
                request.Amount <= 0)
            {
                return BadRequest(new { error = "Invalid conversion request" });
            }

            try
            {
                var result = await _currencyService.ConvertCurrencyAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
using CurrencyExchange.Models;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CurrencyExchange.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/favorites")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ICurrencyService _currencyService;

        public FavoriteController(IFavoriteService favoriteService, ICurrencyService currencyService)
        {
            _favoriteService = favoriteService;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            if (page < 1 || limit < 1 || limit > 100)
            {
                return BadRequest(new { error = "Invalid pagination parameters" });
            }

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var favorites = _favoriteService.GetFavorites(username);
            var currencies = await _currencyService.GetCurrenciesAsync();
            
            var result = favorites
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(f => new
                {
                    f.Id,
                    f.CurrencyCode,
                    Name = currencies.FirstOrDefault(c => c.Code == f.CurrencyCode)?.Name ?? f.CurrencyCode
                })
                .ToList();
            
            return Ok(new 
            {
                Items = result,
                Total = favorites.Count,
                Page = page,
                Limit = limit
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode) || currencyCode.Length != 3)
            {
                return BadRequest(new { error = "Invalid currency code" });
            }

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var currencies = await _currencyService.GetCurrenciesAsync();
            if (!currencies.Any(c => c.Code == currencyCode))
            {
                return BadRequest(new { error = "Currency code not found" });
            }

            var favorite = _favoriteService.AddFavorite(username, currencyCode);
            
            return Ok(new
            {
                favorite.Id,
                favorite.CurrencyCode,
                Name = currencies.FirstOrDefault(c => c.Code == currencyCode)?.Name ?? currencyCode
            });
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveFavorite(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { error = "Invalid favorite ID" });
            }

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = _favoriteService.RemoveFavorite(username, id);
            if (!success)
            {
                return NotFound(new { error = "Favorite not found" });
            }
            
            return Ok(new { });
        }
    }
}
using CurrencyExchange.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CurrencyExchange.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        
        private readonly Dictionary<string, string> _users = new()
        {
            { "admin", "password123" },
            { "user", "userpass" }
        };

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidateUser(string username, string password)
        {
            if (_users.TryGetValue(username, out var storedPassword))
            {
                return storedPassword == password;
            }
            return false;
        }

        public string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"] ?? "defaultDevelopmentKeyWith32Chars!!");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
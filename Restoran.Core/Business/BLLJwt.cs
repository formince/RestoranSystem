using Microsoft.IdentityModel.Tokens;
using Restoran.Core.DTOs.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restoran.Core.Business
{
    public class BLLJwt // JWT işlemleri için basit sınıf
    {
        private readonly string _secretKey = "RestaurantSystemVerySecretKeyForJWT2024RestaurantSystemVerySecretKey"; // 64 karakter
        private readonly string _issuer = "RestaurantSystem";
        private readonly string _audience = "RestaurantUsers";

        public BLLJwt()
        {
            // DI ile ilgili hiçbir şey burada olmayacak.
        }

        // JWT Token oluştur
        public string GenerateToken(UserDetailDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey); // UTF8 daha güvenli

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24), // 24 saat (1 gün) daha güvenli
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer,
                Audience = _audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // JWT Token doğrula
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey); // UTF8 daha güvenli

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5) // 5 dakika tolerance
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
            }
            catch
            {
                return null;
            }
        }

        // Token'dan kullanıcı ID'si al
        public int? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        // Token'dan kullanıcı rolünü al
        public string? GetUserRoleFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            return principal.FindFirst(ClaimTypes.Role)?.Value;
        }

        // Secret key'i dış katmanlara ver (configuration için)
        public string GetSecretKey()
        {
            return _secretKey;
        }

        public string GetIssuer()
        {
            return _issuer;
        }

        public string GetAudience()
        {
            return _audience;
        }
    }
} 
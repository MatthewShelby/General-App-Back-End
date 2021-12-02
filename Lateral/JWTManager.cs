using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Doctors
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        public static string GenerateToken(string email, string id, int expireMinutes)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.Now;
            string timeSpan = now.Ticks.ToString();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Email, email),
                            new Claim(ClaimTypes.NameIdentifier, id),
                            new Claim(ClaimTypes.DateOfBirth,timeSpan ),
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                string hh = "";
                JwtSecurityToken jwtToken = null;
                try
                {
                    jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                }
                catch (Exception ex)
                {

                    hh = ex.Message;
                }

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}
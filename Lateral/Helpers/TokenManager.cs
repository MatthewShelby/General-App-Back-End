using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Doctors
{
    public static class TokenManager
    {

        public static string generateJwtToken(IdentityUser user, IConfiguration configuration)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            // Defauli:
            //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            //Replacement:
            var key = Encoding.UTF8.GetBytes(configuration["JwtAutentication:IssuerSigningKey"]);
            //TokenExpireMinutes
            double minutes = double.Parse(configuration["JwtAutentication:TokenExpireMinutes"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                //DEF: Expires = DateTime.UtcNow.AddDays(7),
                Expires = DateTime.Now.AddMinutes(minutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

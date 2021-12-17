using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctors
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(IConfiguration configuration, RequestDelegate next)
        {
            _configuration = configuration;
            _next = next;

        }

        public async Task Invoke( HttpContext context, UserManager<IdentityUser> userManager)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last().Replace("\"", "");

            if (token != null)
                attachUserToContext(context, userManager, token);

            await _next(context);
        }
        //((Microsoft.AspNetCore.Http.DefaultHttpContext)context).Session = '((Microsoft.AspNetCore.Http.DefaultHttpContext)context).Session' threw an exception of type 'System.InvalidOperationException'
        private void attachUserToContext(HttpContext context, UserManager<IdentityUser> userManager, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                // Defauli:
                //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                //Replacement:
                var key = Encoding.UTF8.GetBytes(_configuration["JwtAutentication:IssuerSigningKey"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // default:
                //var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // Replacement:
                string userId = jwtToken.Claims.First(x => x.Type == "id").Value;
                string usrexp = jwtToken.Claims.First(x => x.Type == "exp").Value;
                string exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(usrexp)).DateTime.ToString();
                // attach user to context on successful jwt validation

                //context.Items["User"] = userManager.FindByIdAsync(userId);
                context.Items["userId"] = userId;
                context.Items["userexp"] = exp;
                
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
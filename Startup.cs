using Doctors.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Doctors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            //if (expires != null)
            //{
            //    return expires > DateTime.UtcNow;
            //}
            return false;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }
            , ServiceLifetime.Transient);

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            services.AddIdentity<IdentityUser, IdentityRole>()   //<<<<<< You have IdentityUser
              .AddDefaultTokenProviders()
              .AddEntityFrameworkStores<ApplicationDbContext>();





            services.AddAuthentication(options =>
            {
                // Identity made Cookie authentication the default.
                // However, we want JWT Bearer Auth to be the default.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(options =>
        {
            // Configure the Authority to the expected value for your authentication provider
            // This ensures the token is appropriately validated
           // options.Authority = /* TODO: Insert Authority URL here */;

            // We have to hook the OnMessageReceived event in order to
            // allow the JWT authentication handler to read the access
            // token from the query string when a WebSocket or 
            // Server-Sent Events request comes in.

            // Sending the access token in the query string is required due to
            // a limitation in Browser APIs. We restrict it to only calls to the
            // SignalR hub in this code.
            // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
            // for more information about security considerations when using
            // the query string to transmit the access token.
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/chathub")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });



            services.AddRazorPages();
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
                
            });

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCors", builder =>
                {
                    builder.WithOrigins(Configuration["ClientAddress"].Split(';'))
                    .AllowAnyMethod()                    
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .Build();
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Environment

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #endregion



            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("EnableCors");
            app.UseMiddleware<JwtMiddleware>();
            /*
            //app.UseAuthentication();
            using (var scope = app.ApplicationServices.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var userManager = (UserManager<IdentityUser>)scope.ServiceProvider.GetService(typeof(UserManager<IdentityUser>));
                // do you things here

            }*/
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/numhub");

            });
        }
    }
}



















//    public class AppSettings
//    {
//        public string Secret { get; set; }
//    }


//[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
//public class AuthorizeAttribute : Attribute, IAuthorizationFilter
//{
//    public void OnAuthorization(AuthorizationFilterContext context)
//    {
//        var user = (User)context.HttpContext.Items["User"];
//        if (user == null)
//        {
//            // not logged in
//            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
//        }
//    }
//}
//public class JwtMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly AppSettings _appSettings;

//        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
//        {
//            _next = next;
//            _appSettings = appSettings.Value;
//        }

//        public async Task Invoke(HttpContext context, IUserService userService)
//        {
//            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

//            if (token != null)
//                attachUserToContext(context, userService, token);

//            await _next(context);
//        }

//        private void attachUserToContext(HttpContext context, IUserService userService, string token)
//        {
//            try
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
//                tokenHandler.ValidateToken(token, new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(key),
//                    ValidateIssuer = false,
//                    ValidateAudience = false,
//                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
//                    ClockSkew = TimeSpan.Zero
//                }, out SecurityToken validatedToken);

//                var jwtToken = (JwtSecurityToken)validatedToken;
//                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

//                // attach user to context on successful jwt validation
//                context.Items["User"] = userService.GetById(userId);
//            }
//            catch
//            {
//                // do nothing if jwt validation fails
//                // user is not attached to context so request won't have access to secure routes
//            }
//        }
//    }


//public interface IUserService
//{
//    AuthenticateResponse Authenticate(AuthenticateRequest model);
//    IEnumerable<User> GetAll();
//    User GetById(int id);
//}
//public class User
//{
//    public int Id { get; set; }
//    public string FirstName { get; set; }
//    public string LastName { get; set; }
//    public string Username { get; set; }

//    [JsonIgnore]
//    public string Password { get; set; }
//}
//public class UserService : IUserService
//{
//    // users hardcoded for simplicity, store in a db with hashed passwords in production applications
//    private List<User> _users = new List<User>
//    {
//        new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
//    };

//    private readonly AppSettings _appSettings;

//    public UserService(IOptions<AppSettings> appSettings)
//    {
//        _appSettings = appSettings.Value;
//    }

//    public AuthenticateResponse Authenticate(AuthenticateRequest model)
//    {
//        var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

//        // return null if user not found
//        if (user == null) return null;

//        // authentication successful so generate jwt token
//        var token = generateJwtToken(user);

//        return new AuthenticateResponse(user, token);
//    }

//    public IEnumerable<User> GetAll()
//    {
//        return _users;
//    }

//    public User GetById(int id)
//    {
//        return _users.FirstOrDefault(x => x.Id == id);
//    }

//    // helper methods

//    private string generateJwtToken(User user)
//    {
//        // generate token that is valid for 7 days
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
//            Expires = DateTime.UtcNow.AddDays(7),
//            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//        };
//        var token = tokenHandler.CreateToken(tokenDescriptor);
//        return tokenHandler.WriteToken(token);
//    }
//}
//public class AuthenticationMiddleware
//{
//    private readonly RequestDelegate _next;

//    // Dependency Injection
//    public AuthenticationMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task Invoke(HttpContext context)
//    {
//        //Reading the AuthHeader which is signed with JWT
//        string authHeader = context.Request.Headers["Authorization"];

//        if (authHeader != null)
//        {
//            //Reading the JWT middle part           
//            int startPoint = authHeader.IndexOf(".") + 1;
//            int endPoint = authHeader.LastIndexOf(".");

//            var tokenString = authHeader
//                .Substring(startPoint, endPoint - startPoint).Split(".");
//            var token = tokenString[0].ToString() + "==";

//            var credentialString = Encoding.UTF8
//                .GetString(Convert.FromBase64String(token));

//            // Splitting the data from Jwt
//            var credentials = credentialString.Split(new char[] { ':', ',' });

//            // Trim this Username and UserRole.
//            var userEmail = credentials[1].Replace("\"", "");
//            var userId = credentials[3].Replace("\"", "");
//            var userExp = credentials[9].Replace("\"", "");
//            var issue = credentials[5].Replace("\"", "");

//            string valueOfUserEmail = userEmail;
//            string valueOfUserId = userId;
//            string valueOfIssue = new DateTime(long.Parse(issue)).ToString();
//            DateTime valueOfUserExp = new DateTime().AddSeconds(long.Parse(userExp));

//            //DateTime valueOfUserExp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(userExp)).DateTime;

//            if (valueOfUserExp >= DateTime.Now)
//            {
//                return;

//                // await _next(context);
//            }

//            // Identity Principal
//            Claim[] claims = null;
//            try
//            {
//                claims = new[]
//                {
//                        new Claim(ClaimTypes.Email, valueOfUserEmail),
//                        new Claim(ClaimTypes.NameIdentifier, valueOfUserId),
//                        new Claim(ClaimTypes.DateOfBirth, valueOfIssue),
//                        new Claim(ClaimTypes.Expiration,valueOfUserExp.ToString())
//                };
//            }
//            catch (Exception)
//            {
//                await _next(context);
//            }

//            var identity = new ClaimsIdentity(claims, "basic");
//            context.User = new ClaimsPrincipal(identity);
//        }
//        //Pass to the next middleware
//        await _next(context);
//    }
//}

//public class JwtIssuerOptions
//{
//    /// <summary>
//    /// 4.1.1.  "iss" (Issuer) Claim - The "iss" (issuer) claim identifies the principal that issued the JWT.
//    /// </summary>
//    public string Issuer { get; set; }

//    /// <summary>
//    /// 4.1.2.  "sub" (Subject) Claim - The "sub" (subject) claim identifies the principal that is the subject of the JWT.
//    /// </summary>
//    public string Subject { get; set; }

//    /// <summary>
//    /// 4.1.3.  "aud" (Audience) Claim - The "aud" (audience) claim identifies the recipients that the JWT is intended for.
//    /// </summary>
//    public string Audience { get; set; }

//    /// <summary>
//    /// 4.1.4.  "exp" (Expiration Time) Claim - The "exp" (expiration time) claim identifies the expiration time on or after which the JWT MUST NOT be accepted for processing.
//    /// </summary>
//    public DateTime Expiration => IssuedAt.Add(ValidFor);

//    /// <summary>
//    /// 4.1.5.  "nbf" (Not Before) Claim - The "nbf" (not before) claim identifies the time before which the JWT MUST NOT be accepted for processing.
//    /// </summary>
//    public DateTime NotBefore { get; set; } = DateTime.UtcNow;

//    /// <summary>
//    /// 4.1.6.  "iat" (Issued At) Claim - The "iat" (issued at) claim identifies the time at which the JWT was issued.
//    /// </summary>
//    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

//    /// <summary>
//    /// Set the timespan the token will be valid for (default is 120 min)
//    /// </summary>
//    public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(1080);//una semana



//    /// <summary>
//    /// "jti" (JWT ID) Claim (default ID is a GUID)
//    /// </summary>
//    public Func<Task<string>> JtiGenerator =>
//      () => Task.FromResult(Guid.NewGuid().ToString());

//    /// <summary>
//    /// The signing key to use when generating tokens.
//    /// </summary>
//    public SigningCredentials SigningCredentials { get; set; }
//}



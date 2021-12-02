using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
//using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Doctors
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {


        #region constructor
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(IConfiguration configuration,
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            string code2 = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            if (userId == null || code == null)
            {
                return await JsonH.ErrorAsync("Invalid email confirmation attempt.");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await JsonH.ErrorAsync($"Unable to load user with ID '{userId}'.");
            }
            try
            {
                var result = await _userManager.ConfirmEmailAsync(user, code2);
                if (result.Succeeded)
                    return await JsonH.SuccessAsync($"User email has been confirmed.");
                else
                    return await JsonH.ErrorAsync($"Unable to match the code.");
            }
            catch (System.Exception ex)
            {
                return await JsonH.ErrorAsync(ex.Message);
            }
        }


        [HttpGet("logout-user")]
        public async Task<IActionResult> LogOutUser()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return await JsonH.SuccessAsync("User has been logged out.");
            }
            catch (System.Exception ex)
            {
                return await JsonH.ErrorAsync(ex.Message);
            }
        }


        [HttpGet("gau")]
        public async Task<Object> GetAllUsers()
        {


            List<CurrentUser> users = new List<CurrentUser>();

            for (int i = 1; i < 7; i++)
            {
                CurrentUser newC = new CurrentUser()
                {
                    id = Guid.NewGuid().ToString(),
                    token = Guid.NewGuid().ToString(),
                    expires = DateTime.Now.AddMonths(i),
                    issue = DateTime.Now.AddYears(i),
                };
                string email = newC.id.Substring(6, 4);
                email = "mr-" + email + email + "@gmail.com";
                newC.email = email;
                users.Add(newC);
            }
            return users;
        }


        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] LoginInputModel Input)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);


                if (result.Succeeded)
                {

                    IdentityUser currentUser = await _userManager.FindByEmailAsync(Input.Email);



                    CurrentUser user = new CurrentUser()
                    {
                        email = currentUser.Email,
                        id = currentUser.Id,
                        //DEF:  token = JwtManager.GenerateToken(currentUser.Email, currentUser.Id, 2),

                        token = TokenManager.generateJwtToken(currentUser, _configuration),
                        //expires = DateTime.Now.AddMinutes(212),
                        //issue = DateTime.Now.AddMinutes(10),
                    };


                    return await JsonH.SuccessAsync(user);

                };

                if (result.RequiresTwoFactor)
                {
                    return await JsonH.ErrorAsync(new { id = "Two factor authentication is needed." });
                }
                if (result.IsLockedOut)
                {
                    return await JsonH.ErrorAsync(new { id = "User is lockedOut." });
                }
                else
                {
                    return await JsonH.ErrorAsync(new { id = "Invalid login attempt." });
                }
            }

            return await JsonH.ErrorAsync(new { id = "ModelState is not valid" });
        }


        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterNewUser([FromBody] InputModel Input)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                IdentityResult result;
                try
                {
                    result = await _userManager.CreateAsync(user, Input.Password);

                }
                catch (System.Exception ex)
                {
                    return await JsonH.ErrorAsync("Couldn't register the user. error message: " + ex.Message);
                }
                // password requires upper case characters
                //user is being register here. the confirmation email still doesn't been sent.
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                if (result.Succeeded)
                {
                    EmailSender emailSender = new EmailSender(_configuration);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //string link = $"https://localhost:44339/api/account/confirm-email?userid={user.Id}&code={code}";

                    string ClientComponentAddress = _configuration["ClientComponentAddress"];

                    string link = ClientComponentAddress + code + "/" + user.Id;


                    await emailSender.SendEmailAsync(Input.Email, "Account Activation.", "", link);
                    return await JsonH.SuccessAsync($"user id is: {user.Id}. code: {code}");
                }
                return await JsonH.ErrorAsync("Couldn't register the user. _userManager.CreateAsync +. result.error");
            }
            return await JsonH.ErrorAsync("Model state is not valid.");
        }


        [AllowAnonymous]
        [HttpGet("isConnected")]
        public bool IsConnected()
        {
            System.Threading.Thread.Sleep(2000);
            return true;
        }

        [AllowAnonymous]
        [HttpGet("num")]
        public async Task<int> GetNumberAsync()
        {
            #region File

            const string path = "C:\\TestFiles\\Num.txt";
            System.IO.StreamReader SR = new System.IO.StreamReader(path);
            string value = SR.ReadToEnd();
            SR.Close();
            SR.Dispose();
            int num = int.Parse(value);
            num++;
            System.IO.StreamWriter SW = new System.IO.StreamWriter(path);
            await SW.WriteAsync(char.Parse(num.ToString()));
            SW.Flush();
            SW.Close();
            SW.Dispose();

            #endregion


            return num;
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            string[] list = _configuration["ClientAddress"].Split(';');


            //.GetChildren().Select(x => x.Value);
            System.Threading.Thread.Sleep(1000);
            return await JsonH.SuccessAsync(list);

        }

        [AuthorizeAttribute]
        [HttpGet("who-am-i")]
        public async Task<IActionResult> Test2()
        {

            var isu = HttpContext.Items["User"];
            var ise = HttpContext.Items["userId"];
            string exp = HttpContext.Items["userexp"].ToString();

            var ssss = await _userManager.FindByIdAsync(ise.ToString());

            //var ssdd = _userManager.FindByIdAsync("426d2f22-5284-4282-a28b-13d8ae7535d3");
            CurrentUser cu = new CurrentUser()
            {
                id = ssss.Id,
                token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last().Replace("\"", ""),
                email = ssss.Email,
                expires = DateTime.Parse(exp)
            };

            return await JsonH.SuccessAsync(cu);
        }


        [HttpGet("test-user")]
        public async Task<IActionResult> UserTest()
        {
            try
            {
                var applicationUser = await _userManager.GetUserAsync(HttpContext.User);
                string userEmail = applicationUser?.Email;
                string userId = applicationUser?.Id;
                if (!string.IsNullOrEmpty(userId))
                    return await JsonH.SuccessAsync($"User email is: {userEmail}. \n user id is: {userId}");
                return await JsonH.ErrorAsync($"Couldn't find the user.");
            }
            catch (System.Exception ex)
            {
                return await JsonH.ErrorAsync(ex.Message);
            }
        }



        [HttpGet("test-auth")]
        [AuthorizeAttribute]
        public async Task<ActionResult> AuthorizationTest()
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);
            IEnumerable<System.Security.Claims.Claim> userClaims = HttpContext.User.Claims;

            string exp = userClaims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration").Value;
            string issue = userClaims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth").Value;


            DateTime tokenexpireTime = DateTime.Parse(exp);
            DateTime tokenIssueTime = DateTime.Parse(issue);

            // Error when token is expired;
            // "Input string was not in a correct format."


            CurrentUser resUser = new CurrentUser()
            {
                email = user.Email,
                id = user.Id,
                token = GetToken(),
                expires = tokenexpireTime,
                issue = tokenIssueTime,
            };
            return await JsonH.SuccessAsync(resUser);
        }


        //Gets JWT Token from request header 
        private string GetToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            authHeader = authHeader.Substring(8, authHeader.Length - 9);
            return authHeader;
        }

        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class CurrentUser
    {
        public string id { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public DateTime expires { get; set; }
        public DateTime issue { get; set; }
    }
}

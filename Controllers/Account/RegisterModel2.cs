using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Doctors.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterModel2Controller : ControllerBase
    {
        [HttpGet("test")]
        public string test()
        {
            return "Test is ok.";
        }

        [HttpPost("post")]
        public async Task<IActionResult> Post([FromBody] RegisterModel.InputModel Input, string returnUrl = null)
        {
            return await _reg.OnPostAsync(Input, returnUrl);
        }

        private readonly RegisterModel _reg;
        public RegisterModel2Controller(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, IConfiguration c)
        {
            this._reg = new RegisterModel(userManager, signInManager, logger, emailSender,c);
        }


        public class RegisterModel : PageModel
        {
            public async Task<IActionResult> OnPostAsync(InputModel Input, string returnUrl = null)
            {
                returnUrl = returnUrl ?? Url.Content("~/");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                    try
                    {
                        var result = await _userManager.CreateAsync(user, Input.Password);
                        //user is being register here. the confirmation email still doesn't been sent.
                        if (result.Succeeded)
                        {
                            //_logger.LogInformation("User created a new account with password.");

                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                            /*var callbackUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                                protocol: Request.Scheme);
                            */
                            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                                $"Please confirm your account by <a href='///'>clicking here</a>.");

                            EmailSender ES = new EmailSender(_configuration);
                            await ES.SendEmailAsync(Input.Email, "fff", "lllllll","");

                            /*$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/
                            /*
                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {
                                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                            }
                            else
                            {
                                await _signInManager.SignInAsync(user, isPersistent: false);
                                return LocalRedirect(returnUrl);
                            }
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }*/
                            return new ObjectResult("Done");

                        }
                    }
                    catch (Exception ex)
                    {
                        string g = ex.Message;
                        //throw;
                    }
                }

                // If we got this far, something failed, redisplay form
                //return Page();
                return new ObjectResult("Error.");
            }

            private readonly SignInManager<IdentityUser> _signInManager;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly ILogger<RegisterModel> _logger;
            private readonly IEmailSender _emailSender;
            private readonly IConfiguration _configuration;


            public RegisterModel(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager,
                ILogger<RegisterModel> logger,
                IEmailSender emailSender, IConfiguration c)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _logger = logger;
                _emailSender = emailSender;
                _configuration = c;
            }

            [BindProperty]
            public InputModel Input { get; set; }

            public string ReturnUrl { get; set; }

            public IList<AuthenticationScheme> ExternalLogins { get; set; }

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

            public async Task OnGetAsync(string returnUrl = null)
            {
                ReturnUrl = returnUrl;
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            }

        }
    }
}

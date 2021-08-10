using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using BugTracker.Helpers;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BugTracker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        // PROPERTIES
        private readonly SignInManager<BugTrackerUser> _signInManager;
        private readonly UserManager<BugTrackerUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;


        // CONSTRUCTORS
        public RegisterModel(
            UserManager<BugTrackerUser> userManager,
            SignInManager<BugTrackerUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // PROPERTIES (other stuff)
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // CONSTRUCTOR 
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

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


        // METHODS
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Create the model for Identity Account
                var user = new BugTrackerUser { 
                    UserName = Input.UserName, 
                    Email = Input.Email, 
                    FirstName = Input.FirstName, 
                    LastName = Input.LastName 
                };


                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Update myDB.
                    InsertNewBTUser(user.Id);

                    // Add User to default NoRole role.
                    await _userManager.AddToRoleAsync(user, "NoRole");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

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
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private void InsertNewBTUser(string stringId)
        {
            // Establish connection.
            SqlConnection db = DbHelper.GetConnection();

            // Generate Id.
            string getMaxUserIdQuery = "SELECT BTUsers.Id FROM BTUsers ORDER BY BTUsers.Id DESC;";
            // Make sure there is something in the database. Default is 0.
            int extractedMax = db.Query<int>(getMaxUserIdQuery).FirstOrDefault();
            int userId = extractedMax + 1;

            
            string query = "INSERT INTO BTUsers(Id, UserName, StringId, FirstName, LastName) VALUES( @UserId, @userName, @stringId, @firstName, @lastName );";

            // Insert the new BTUser.
            db.Execute(query, new { 
                UserId = userId, 
                userName = Input.UserName, 
                stringId = stringId,
                firstName = Input.FirstName,
                lastName = Input.LastName

            });
        }
    }
}

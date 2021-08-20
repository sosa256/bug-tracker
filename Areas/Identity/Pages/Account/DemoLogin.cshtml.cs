using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BugTracker.Areas.Identity.Pages.Account
{
    public class DemoLoginModel : PageModel
    {
        // PROPERTIES
        private readonly UserManager<BugTrackerUser> _userManager;
        private readonly SignInManager<BugTrackerUser> _signInManager;
        public enum demoRoleSelect
        {
            useAdmin = 1,
            useDeveleoper,
            useSubmitter,
        }




        // CONSTRUCTORS
        public DemoLoginModel(
            UserManager<BugTrackerUser> userManager,
            SignInManager<BugTrackerUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }




        // METHODS
        public async Task<IActionResult> OnPostAsync(IFormCollection collection, string returnUrl = null)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = null;
            int demoRole = Int32.Parse(collection["DemoRole"].ToString());
            switch (demoRole)
            {
                case (int)demoRoleSelect.useAdmin:
                    result = await _signInManager.PasswordSignInAsync(
                        "DemoAdministrator",
                        "WWSSadadqeeq1!", // password
                        false, 
                        lockoutOnFailure: false
                    );
                    break;

                case (int)demoRoleSelect.useDeveleoper:
                    result = await _signInManager.PasswordSignInAsync(
                        "DemoDeveloper",
                        "WWSSadadqeeq1!",
                        false,
                        lockoutOnFailure: false
                    );
                    break;

                case (int)demoRoleSelect.useSubmitter:
                    result = await _signInManager.PasswordSignInAsync(
                        "DemoSubmitter",
                        "WWSSadadqeeq1!",
                        false,
                        lockoutOnFailure: false
                    );
                    break;
            }

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // 
            return Page();
        }



    }
}

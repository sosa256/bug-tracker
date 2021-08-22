using BugTracker.Areas.Identity.Data;
using BugTracker.Data;
using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Administrator, DemoAdministrator")]
    public class RoleController : Controller
    {
        // PROPERTIES
        private readonly SqlHelper _sqlHelper;
        private readonly BugTrackerContext _dbIdentity;
        private readonly UserManager<BugTrackerUser> _userManager;




        // CONSTRUCTORS
        public RoleController(UserManager<BugTrackerUser> userManager, BugTrackerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _dbIdentity = context;

            var user = httpContextAccessor.HttpContext.User;
            bool isDemoAccount = user.IsInRole("DemoAdministrator")
                || user.IsInRole("DemoDeveloper")
                || user.IsInRole("DemoSubmitter");
            if (isDemoAccount)
            {
                // Use demo database connection.
                _sqlHelper = new SqlHelper(DbHelper.GetDemoConnection());
            }
            else
            {
                // Use actual database connection.
                _sqlHelper = new SqlHelper();
            }
        }




        // ACTIONS / METHODS

        // GET: Role/Assignment
        [HttpGet]
        public ActionResult Assignment()
        {
            // Get all the users.
            List<BTUser> userList = _sqlHelper.SelectAllUsersForAssignment();
            return View(userList);
        }


        // TODO: Learn how to make confirmation notification.
        // POST: Role/UserRoleUpdate
        // Change the users role to something else.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserRoleUpdateAsync(IFormCollection collection)
        {
            // Parse out normal UserId and the new role to assign.
            int currUserId = Int32.Parse( collection["item.Id"].ToString() );
            int newRoleId = Int32.Parse( collection["item.Role"].ToString() );
            
            // Update my database.
            _sqlHelper.AssignUserRole(currUserId, newRoleId);

            string currUserIdentityId = collection["item.StringId"].ToString();
            if (currUserIdentityId == "")
            {
                // The user is a dummy account
                // no need to update ASP Identity tables 
                return RedirectToAction("Assignment");
            }

            string roleName = ( (BTUserRoles) newRoleId ).ToString();

            // Update ASP Identity database (to correctly enable authorization).
            // Delete any and all entries of the currUser from the DB.
            _sqlHelper.MakeUserHaveNoRoles(currUserIdentityId);

            // Add in our new entry w/ currUserIdentityId and roleIdIdentity.
            await _userManager.AddToRoleAsync( _dbIdentity.Users.Find(currUserIdentityId), roleName );

            return RedirectToAction("Assignment");
        } // END OF: UserRoleUpdateAsync( IFormCollection )


    }
}

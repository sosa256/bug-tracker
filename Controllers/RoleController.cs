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
    [Authorize(Roles = "Administrator")]
    public class RoleController : Controller
    {
        // PROPERTIES
        private readonly SqlHelper _sqlHelper;
        private readonly BugTrackerContext _dbIdentity;
        private readonly UserManager<BugTrackerUser> _userManager;




        // CONSTRUCTORS
        public RoleController(UserManager<BugTrackerUser> userManager, BugTrackerContext context)
        {
            _userManager = userManager;
            _dbIdentity = context;
            _sqlHelper = new SqlHelper();
        }




        // ACTIONS / METHODS

        // GET: Role/Assignment
        [HttpGet]
        public ActionResult Assignment()
        {
            // Get all the users.
            List<BTUser> userList = _sqlHelper.SelectAllUsers();
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


        // GET: Role/Rename/abc-d12-3ef
        [HttpGet]
        public ActionResult Rename(string id, bool check)
        {
            IdentityRole roleToUpdate = _dbIdentity.Roles.Find( id );

            RoleViewModel model = new RoleViewModel(roleToUpdate, check);

            return View(model);
        }


// CRUD operations for role.
        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            string newRoleName = collection["currRole"].ToString();

            if (newRoleName == "")
            {
                // The new role name is not valid.
                return RedirectToAction("Manage", new { check = true });
            }

            string newRoleNameNormalized = newRoleName.ToUpper();


            // Modified code from: https://www.dotnetfunda.com/articles/show/2898/working-with-roles-in-aspnet-identity-for-mvc
            // Create new role object.
            // Add role to database.
            _dbIdentity.Roles.Add(new IdentityRole()
            {
                Name = newRoleName,
                NormalizedName = newRoleNameNormalized

            }); ;
            _dbIdentity.SaveChanges();

            return RedirectToAction("Manage");
        }// END OF: Create( IFormCollection )


        // GET: Role/Manage
        [HttpGet]
        public ActionResult Manage(bool check)
        {
            // Get a list of roles.
            List<IdentityRole> identityRoleList = _dbIdentity.Roles.ToList();

            RoleViewModel model = new RoleViewModel(identityRoleList, check);

            return View(model);
        }


        // POST: Role/Update/abc-d12-3ef
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string id, IFormCollection collection)
        {
            string newName = collection["currRole.Name"].ToString();

            if (newName == "")
            {
                // The role name is not valid.
                return RedirectToAction("Rename", new { check = true, id = id });
            }

            string newNameNormalized = newName.ToUpper();

            // Get the role.
            IdentityRole roleToUpdate = _dbIdentity.Roles.Find(id);

            // Update the property.
            roleToUpdate.Name = newName;
            roleToUpdate.NormalizedName = newNameNormalized;

            // Update database.
            _dbIdentity.Roles.Update(roleToUpdate);
            _dbIdentity.SaveChanges();

            return RedirectToAction("Manage");
        }


        // GET: Role/Delete/abc-d12-3ef
        // TODO: Learn how to make nice alerts.
        [HttpGet]
        public ActionResult Delete(string id)
        {
            // Get role and delete it.
            IdentityRole roleToDelete = _dbIdentity.Roles.Find(id);

            _dbIdentity.Roles.Remove(roleToDelete);
            _dbIdentity.SaveChanges();

            return RedirectToAction("Manage");
        }
    }
}

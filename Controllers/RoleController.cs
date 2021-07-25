using BugTracker.Data;
using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace BugTracker.Controllers
{
    public class RoleController : Controller
    {
        // PROPERTIES
        private SqlConnection _db;
        private readonly BugTrackerContext _dbIdentity;




        // CONSTRUCTORS
        public RoleController(BugTrackerContext context)
        {
            _db = DbHelper.GetConnection();
            _dbIdentity = context;
        }




        // ACTIONS / METHODS

        // Role Assignment
        // GET: Role/Assignment
        [HttpGet]
        public ActionResult Assignment()
        {
            // Get all the users.
            RoleAssignmentViewModel pizza = new RoleAssignmentViewModel()
            {
                userList = _db.Query<BTUser>("SELECT * FROM BTUsers").ToList()
            };

            return View(pizza);
        }

        [HttpPost]
        public ActionResult RoleUpdate(IFormCollection collection)
        {
            string currUser = collection["item.Id"].ToString();
            int newRoleId = Int32.Parse( collection["item.Role"].ToString() );
            
            string currUserIdentity = collection["item.StringId"].ToString();
            string roleName = ((BugTracker.Models.BTUserRoles)newRoleId).ToString();


            // Update my database.
            string query = String.Format("UPDATE BTUsers SET BTUsers.Role = {0} WHERE BTUsers.Id = {1}", newRoleId, currUser );
            _db.Execute(query);

            // Update ASP Identity database (to correctly enable authorization).
            // Delete any and all entries of the currUser from the DB.
            query = String.Format("DELETE FROM AspNetUserRoles WHERE UserId = '{0}'", currUserIdentity);
            _db.Execute(query);

            // Get RoleId.
            query = String.Format("SELECT Id FROM AspNetRoles WHERE Name = '{0}'", roleName);
            string roleIdIdentity = _db.Query<String>(query).Single();

            // Add in our new entry w/ currUserIdentity and roleIdIdentity.

            _dbIdentity.Add<Microsoft.AspNetCore.Identity.IdentityUserRole>()
            //_dbIdentity.UserRoles.Add(
            //    new Microsoft.AspNetCore.Identity.IdentityUserRole("") { RoleId = roleIdIdentity, UserId = currUserIdentity}
            //);

            // Save changes.
            _dbIdentity.SaveChanges();

            return RedirectToAction("Assignment");
        }



        // Manage Roles
        // GET: Role/Manage
        [HttpGet]
        public ActionResult Manage()
        {
            // Get a list of roles.
            RoleViewModel model = new RoleViewModel()
            {
                roleList = _dbIdentity.Roles.ToList()
            };

            return View(model);
        }


        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // Modified code from: https://www.dotnetfunda.com/articles/show/2898/working-with-roles-in-aspnet-identity-for-mvc
                // Create new role object.
                // Add role to database.
                _dbIdentity.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole()
                {
                    Name = collection["currRole"],
                    NormalizedName = collection["currRole"].ToString().ToUpper()

                }); ;
                _dbIdentity.SaveChanges();

                return RedirectToAction("Manage");
            }
            catch
            {
                return View();
            }
        }


        // GET: Role/Update/5
        [HttpGet]
        public ActionResult Update(string id)
        {
            RoleViewModel pizza = new RoleViewModel()
            {
                currRole = _dbIdentity.Roles.Find(id)
            };
            return View(pizza);
        }


        // POST: Role/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string id, IFormCollection collection)
        {
            try
            {
                // Get the role.
                Microsoft.AspNetCore.Identity.IdentityRole roleToUpdate = _dbIdentity.Roles.Find(id);

                // Update the property.
                roleToUpdate.Name = collection["currRole.Name"];
                roleToUpdate.NormalizedName = roleToUpdate.Name.ToUpper();

                // Update database.
                _dbIdentity.Roles.Update(roleToUpdate);
                _dbIdentity.SaveChanges();

                return RedirectToAction("Manage");
            }
            catch
            {
                return View();
            }
        }


        // GET: Role/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            // Get role and delete it.
            _dbIdentity.Roles.Remove(_dbIdentity.Roles.Find(id));
            _dbIdentity.SaveChanges();

            return RedirectToAction("Manage");
        }
    }
}

using BugTracker.Data;
using BugTracker.Helpers;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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


        // METHODS
        // GET: RoleController
        public ActionResult Index()
        {
            // Get a list of roles.
            RoleViewModel pizza = new RoleViewModel()
            {
                roleList = _dbIdentity.Roles.ToList()
            };


            return View(pizza);
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // Modified code from: https://www.dotnetfunda.com/articles/show/2898/working-with-roles-in-aspnet-identity-for-mvc
                // create new role obj.
                // add obj to db.
                _dbIdentity.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole()
                {
                    Name = collection["currRole"],
                    NormalizedName = collection["currRole"].ToString().ToUpper()

                }); ;
                _dbIdentity.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Role/Edit/5
        public ActionResult Update(string id)
        {
            RoleViewModel pizza = new RoleViewModel()
            {
                currRole = _dbIdentity.Roles.Find( id )
            };
            return View(pizza);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string id, IFormCollection collection)
        {
            try
            {
                // Get the role.
                // update the property.
                Microsoft.AspNetCore.Identity.IdentityRole roleToUpdate = _dbIdentity.Roles.Find(id);
                roleToUpdate.Name = collection["currRole.Name"];

                // update db.
                _dbIdentity.Roles.Update(roleToUpdate);

                _dbIdentity.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Role/Delete/5
        public ActionResult Delete(string id)
        {
            // Get role and delete it.
            _dbIdentity.Roles.Remove(_dbIdentity.Roles.Find(id));
            _dbIdentity.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

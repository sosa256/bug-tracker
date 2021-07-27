using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class ProjectController : Controller
    {
        // PROPERTIES
        private SqlConnection _db;




        // CONSTRUCTORS
        public ProjectController()
        {
            _db = DbHelper.GetConnection();
        }




        // ACTIONS / METHODS

        // GET: Project/Details/5
        // Find a way to strip away the stringId and any other properties not needed.
        public ActionResult Details(int id)
        {
            ProjectDetailsViewModel model = new ProjectDetailsViewModel()
            {
                project = _db.Query<Project>(String.Format("SELECT * FROM Projects WHERE Projects.Id = {0};", id)).First(),
                /* SELECT * 
                 * FROM BTUsers
                 * WHERE BTUsers.Id IN(
                 *     SELECT Assignments.UserAssigned
                 *     FROM Assignments
                 *     WHERE Assignments.ProjectId = {0}
                 * ); 
                 */
                usersAssigned = _db.Query<BTUser>(String.Format("SELECT * FROM BTUsers WHERE BTUsers.Id IN(	SELECT Assignments.UserAssigned	FROM Assignments WHERE Assignments.ProjectId = {0});", id) ).ToList()
            };
            return View(model);
        }

        // GET: ProjectController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProjectController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        // POST: ProjectController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }





        // CRUD operations
        // GET: Project
        public ActionResult Manage()
        {
            // Get list of projects.
            ProjectViewModel model = new ProjectViewModel()
            {
                projList = _db.Query<Project>("SELECT * FROM Projects").ToList()
            };

            return View(model);
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // Insert new project to myDB.
                string query = "INSERT INTO Projects(Title) VALUES( @title );";
                _db.Execute(query, new { title = collection["currProject.Title"].ToString() });

                return RedirectToAction("Manage");
            }
            catch
            {
                return View();
            }
        }


        // GET: Project/Details/5
        public ActionResult Update(int id)
        {
            ProjectViewModel model = new ProjectViewModel()
            {
                // I'm allowing String.Format here b/c
                // only an int is allowed through anyways.
                currProject = _db.Query<Project>(String.Format("SELECT * FROM Projects WHERE Projects.Id = {0};", id)).First()
            };

            return View(model);
        }

        // POST: ProjectController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id, IFormCollection collection)
        {
            try
            {
                // Update the project with the new name.
                string query = "UPDATE Projects SET Title = @newTitle WHERE Projects.Id = @projId;";
                _db.Execute(query, new { newTitle = collection["currProject.Title"].ToString(), projId = id });

                return RedirectToAction("Manage");
            }
            catch
            {
                return View();
            }
        }


        // GET: ProjectController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // Delete project from myDB.
                string query = "DELETE FROM Projects WHERE Projects.Id = @projId ;";
                _db.Execute(query, new { projId = id });

                return RedirectToAction("Manage");
            }
            catch
            {
                return View();
            }
        }
    }
}

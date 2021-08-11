using BugTracker.Areas.Identity.Data;
using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProjectController : Controller
    {
        // PROPERTIES
        private readonly SqlHelper _sqlHelper;
        private readonly UserManager<BugTrackerUser> _userManager;




        // CONSTRUCTORS
        public ProjectController(UserManager<BugTrackerUser> userManager)
        {
            _sqlHelper = new SqlHelper();
            _userManager = userManager;
        }




        // ACTIONS / METHODS

        // GET: Project/Update/5
        [HttpGet]
        public ActionResult Update(int id)
        {
            Project currProject = _sqlHelper.SelectProject(id);

            // Make sure the project exists.
            if (currProject == null)
            {
                return Content("Whoops that project doesn't exist!");
            }

            ProjectViewModel model = new ProjectViewModel(currProject);

            return View(model);
        }


        // GET: Project/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            Project projectToLookAt = _sqlHelper.SelectProject(id);

            // Make sure the project exists.
            if (projectToLookAt == null)
            {
                return Content("Whoops that project doesn't exist!");
            }

            List<BTUser> usersAssigned = _sqlHelper.SelectUsersInProject(id);

            ProjectDetailsViewModel model = new ProjectDetailsViewModel(projectToLookAt, usersAssigned);
            
            return View(model);
        }


        // GET: Project/Assign/5
        [HttpGet]
        public ActionResult Assign(int projId)
        {
            Project currProject = _sqlHelper.SelectProject(projId);

            // Make sure the project exists.
            if (currProject == null)
            {
                return Content("Whoops that project doesn't exist!");
            }

            // List of avalible users
            List<BTUser> freeUsers = _sqlHelper.SelectUnassignedUsers();

            ProjectsAssignViewModel model = new ProjectsAssignViewModel(currProject, freeUsers);
            
            return View(model);
        }


        // POST: Project/Assign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(IFormCollection collection)
        {
            // ValidateAntiForgeryToken should take care of any tampering
            // since the feilds are hidden from the user.

            // Extract user input from form.
            int userId = Int32.Parse(collection["item.Id"].ToString());
            int projId = Int32.Parse(collection["project.Id"].ToString());

            // Assign user to project.
            _sqlHelper.AssignUserToProject(userId, projId);

            return RedirectToAction("Assign", new { projId = projId });
        }


        // POST: Project/Unassign/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unassign(IFormCollection collection)
        {
            int userId = Int32.Parse(collection["item.Id"].ToString());
            int projId = Int32.Parse(collection["project.Id"].ToString());

            // Remove the assignment.
            _sqlHelper.DeleteUserProjectAssignment(userId, projId);

            return RedirectToAction("Details", new { id = projId });
        }


        public List<ProjectReadable> ProjListToReadable(List<Project> projList)
        {
            List<ProjectReadable> ret = new List<ProjectReadable>();
            if (projList.Count == 0)
            {
                // There are no projects.
                return ret;
            }

            // Begin processing the list.
            foreach (Project item in projList)
            {
                string ownerFullName = _sqlHelper.GetUserFullName(item.Owner);

                ProjectReadable readable = new ProjectReadable(
                    item,
                    // Create the OpenedBy string.
                    ownerFullName
                );

                ret.Add(readable);
            }

            return ret;
        }


// CRUD operations for project.
        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            // Validate input
            string title = collection["currProject.Title"].ToString();
            if (title == "")
            {
                ProjectViewModel model = new ProjectViewModel(
                    ProjListToReadable(_sqlHelper.SelectAllProjects()),
                    "Title is required"
                );

                return View("Manage", model);
            }
            else if(title.Length <= 3)
            {
                ProjectViewModel model = new ProjectViewModel(
                    ProjListToReadable(_sqlHelper.SelectAllProjects()),
                    "Title must contain at least three characters"
                );

                return View("Manage", model);
            }
            else if (title.Length >= 255)
            {
                ProjectViewModel model = new ProjectViewModel(
                    ProjListToReadable(_sqlHelper.SelectAllProjects()),
                    "Title must contain a maximum of 255 characters!"
                );

                return View("Manage", model);
            }

            // The Title must be valid here.


            // Generate a projectId.
            int projectId = _sqlHelper.GenerateProjectId();

            // Extract the ownerId from ASP Identity.
            string ownerIdentityId = _userManager.GetUserId(User);
            int ownerId = _sqlHelper.SelectUserFromStringId(ownerIdentityId).Id;

            Project projectToInsert = new Project(projectId, title, ownerId);

            // Insert new project to myDB.
            _sqlHelper.InsertProject(projectToInsert);

            return RedirectToAction("Manage");
        }


        // GET: Project/Manage
        [HttpGet]
        public ActionResult Manage()
        {
            // Get list of projects.
            List<ProjectReadable> projectList = ProjListToReadable( _sqlHelper.SelectAllProjects() );

            ProjectViewModel model = new ProjectViewModel(projectList);

            return View(model);
        }
        
        
        // POST: Project/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id, IFormCollection collection)
        {
            string newTitle = collection["currProject.Title"].ToString();

            // Validate input.
            if (newTitle == "")
            {
                ProjectViewModel model = new ProjectViewModel(
                    new Project(id, newTitle),
                    "Title is required"
                );

                return View("Update", model);
            }
            else if (newTitle.Length <= 3)
            {
                ProjectViewModel model = new ProjectViewModel(
                    new Project(id, newTitle),
                    "Title must contain at least three characters"
                );

                return View("Update", model);
            }
            else if (newTitle.Length >= 255)
            {
                ProjectViewModel model = new ProjectViewModel(
                    new Project(id, newTitle),
                    "Title must contain a maximum of 255 characters!"
                );

                return View("Update", model);
            }

            // Title is valid.

            // Update the project with the new name.
            _sqlHelper.UpdateProjectTitle(id, newTitle);

            return RedirectToAction("Manage");
        }


        // GET: Project/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            // Process the user input.
            Project test = _sqlHelper.SelectProject(id);
            if (test == null)
            {
                return Content("Whoops that project doesn't exist!");
            }

            // Delete project from myDB.
            _sqlHelper.DeleteProject(id);

            return RedirectToAction("Manage");
        }
    }
}

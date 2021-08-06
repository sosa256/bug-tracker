﻿using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

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
                usersAssigned = _db.Query<BTUser>(String.Format("SELECT * FROM BTUsers WHERE BTUsers.Id IN(	SELECT Assignments.UserAssigned	FROM Assignments WHERE Assignments.ProjectId = {0});", id)).ToList()
            };
            return View(model);
        }


        [HttpPost]
        // POST: Project/Unassign/
        public ActionResult Unassign(IFormCollection collection)
        {
            int userId = Int32.Parse(collection["item.Id"].ToString());
            int projId = Int32.Parse(collection["project.Id"].ToString());
            string query = "DELETE FROM Assignments WHERE Assignments.UserAssigned = @userAssigned AND Assignments.ProjectId = @projectId;";

            // Remove the assignment.
            _db.Execute(query, new { userAssigned = userId, projectId = projId });

            return RedirectToAction("Details", new { id = projId });
        }


        [HttpGet]
        // GET: Project/Assign/5
        public ActionResult Assign(int projId)
        {
            // List of avalible users
            ProjectsAssignViewModel model = new ProjectsAssignViewModel()
            {
                project = _db.Query<Project>(String.Format("SELECT * FROM Projects WHERE Projects.Id = {0};", projId)).First(),
                freeUsers = _db.Query<BTUser>("SELECT * FROM BTUsers WHERE BTUsers.Id NOT IN (	SELECT Assignments.UserAssigned	FROM Assignments);").ToList()
            };
            return View(model);
        }


        [HttpPost]
        // POST: Project/Assign/5
        public ActionResult Assign(IFormCollection collection)
        {
            int userId = Int32.Parse(collection["item.Id"].ToString());
            int projId = Int32.Parse(collection["project.Id"].ToString());
            string query = "INSERT INTO Assignments VALUES(@userAssigned, @projectId);";

            // Assign user to project.
            _db.Execute(query, new {userAssigned = userId, projectId = projId} );

            return RedirectToAction("Assign", new { projId = projId } );
        }



        // CRUD operations for project
        // GET: Project/Manage
        [HttpGet]
        public ActionResult Manage()
        {
            // Get list of projects.
            ProjectViewModel model = new ProjectViewModel()
            {
                projReadableList = ProjListToProjReadableList( _db.Query<Project>("SELECT * FROM Projects").ToList() )
            };

            return View(model);
        }


        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            // Generate a projectId.
            string getMaxProjectIdQuery = "SELECT Projects.Id FROM Projects ORDER BY Projects.Id DESC;";
            // Make sure there is something in the database. Default is 0.
            int extractedMax = _db.Query<int>(getMaxProjectIdQuery).FirstOrDefault();
            int ticketId = extractedMax + 1;

            // Insert new project to myDB.
            string insertProjectQuery = "INSERT INTO Projects VALUES( @queryTicketId, @title, @ownerId );";
            var parameters = new { 
                queryTicketId = ticketId,
                title = collection["currProject.Title"].ToString(),
                // TODO: Get ownerId from Identity Logged in user.
                ownerId = 1
            };
            _db.Execute(insertProjectQuery, parameters);

            return RedirectToAction("Manage");
        }


        // GET: Project/Update/5
        [HttpGet]
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


        // POST: Project/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id, IFormCollection collection)
        {
            // Update the project with the new name.
            string query = "UPDATE Projects SET Title = @newTitle WHERE Projects.Id = @projId;";
            _db.Execute(query, new { newTitle = collection["currProject.Title"].ToString(), projId = id });

            return RedirectToAction("Manage");
        }


        [HttpGet]
        // GET: Project/Delete/5
        public ActionResult Delete(int id)
        {
            // Delete project from myDB.
            string query = "DELETE FROM Projects WHERE Projects.Id = @projId ;";
            _db.Execute(query, new { projId = id });

            return RedirectToAction("Manage");
        }

        public List<ProjectReadable> ProjListToProjReadableList(List<Project> projList)
        {
            List<ProjectReadable> ret = new List<ProjectReadable>();
            if (projList.Count == 0)
            {
                // There are no projects.
                return ret;
            }

            /* SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName
             * FROM BTUsers
             * WHERE BTUsers.Id IN(
             *     SELECT Projects.Owner
             *     FROM Projects
             *     WHERE Projects.Id = @projId
             * );
             */
            string getProjNameQuery = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Projects.Owner FROM Projects WHERE Projects.Id = @projId );";

            // Begin processing the list.
            foreach (Project item in projList)
            {
                string ownerFullName = _db.Query<string>(getProjNameQuery, new { projId = item.Id }).FirstOrDefault();

                ProjectReadable readable = new ProjectReadable(
                    item,
                    // Create the OpenedBy string.
                    ownerFullName
                );

                ret.Add(readable);
            }

            return ret;
        }
    }
}

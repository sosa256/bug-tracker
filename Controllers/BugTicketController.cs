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

namespace BugTracker.Controllers
{
    public class BugTicketController : Controller
    {
        // PROPERTIES
        private SqlConnection _db;




        // CONSTRUCTORS
        public BugTicketController()
        {
            _db = DbHelper.GetConnection();
        }




        // ACTIONS / METHODS

        // GET: BugTicketController
        public ActionResult Manage()
        {
            BugTicketViewModel model = new BugTicketViewModel()
            {
                bugTicketReadableList = TicketListToTicketReadableList( _db.Query<Ticket>("SELECT * FROM Tickets").ToList() )
            };
            return View(model);
        }

        // GET: BugTicketController/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            var parameterTicketId = new { ticketId = id };
            string getTicketQuery = "SELECT * FROM Tickets WHERE Tickets.Id = @ticketId;";
            /* SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName
             * FROM BTUsers
             * WHERE BTUsers.Id IN
             * (
	         *     SELECT Tickets.OpenedBy
	         *     FROM Tickets
	         *     WHERE Tickets.Id = 1
             * );
             */
            string getTicketOpenedByName = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Tickets.OpenedBy FROM Tickets WHERE Tickets.Id = @ticketId );";
            /* SELECT * 
             * FROM Projects 
             * WHERE Projects.Id IN 
             * (
	         *    SELECT Tickets.Id
	         *    FROM Tickets
	         *    WHERE Tickets.Id = 1
             *  );
             */
            string getProjectQuery = "SELECT * FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @ticketId );";
            string getProjectTitleQuery = "SELECT Projects.Title FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @ticketId );";

            BugTicketDetailsViewModel model = new BugTicketDetailsViewModel()
            {
                //currTicket = _db.Query<Ticket>(      getTicketQuery, parameterTicketId ).First(),
                parentProject = _db.Query<Project>( getProjectQuery, parameterTicketId ).First(),
                currTicketReadable = new TicketReadable(
                    _db.Query<Ticket>(        getTicketQuery, parameterTicketId ).First(),
                    _db.Query<string>(  getProjectTitleQuery, parameterTicketId ).First(),
                    _db.Query<string>( getTicketOpenedByName, parameterTicketId ).First()
                )
            };
            return View(model);
        }

        // GET: BugTicketController/Create
        [HttpGet]
        public ActionResult Create()
        {
            string query = "SELECT Projects.Id, Projects.Title FROM Projects;";
            BugTicketCreateViewModel model = new BugTicketCreateViewModel()
            {
                newTicket = new Ticket(),
                // Create the proj list (id, Title)
                projOptionList = _db.Query<BugTicketCreateViewModel.projOption>(query).ToList()
            };

            return View(model);
        }

        // POST: BugTicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            int projId = Int32.Parse( collection["newTicket.ProjectParent"].ToString() );
            string ticketTitle = collection["newTicket.Title"].ToString();
            int ticketSeverity = Int32.Parse( collection["newTicket.Severity"].ToString() );
            string ticketBehavior = collection["newTicket.UnwantedBehavior"].ToString();
            string ticketSteps = collection["newTicket.RepeatableSteps"].ToString();

            // Generate ticketHistoryId.
            string query = "SELECT Tickets.HistoryId FROM Tickets ORDER BY Tickets.HistoryId DESC;";
            // make sure there is something in the database.
            int extractedMax = _db.Query<int>(query).FirstOrDefault();
            int ticketHistoryId = extractedMax + 1;


            // Add Ticket to myDB.
            query = "INSERT INTO[Tickets](ProjectParent, HistoryId, IsCurr, Title, Severity, Status, UnwantedBehavior, RepeatableSteps, OpenedBy) VALUES( @projParent, @historyId, @isCurr, @title, @severity, @status, @unwantedBehavior, @repeatableSteps, @openedBy );";
            var parameters = new { 
                projParent = projId, 
                historyId = ticketHistoryId, 
                isCurr = Convert.ToInt32(true), 
                title = ticketTitle, 
                severity = ticketSeverity, 
                status = (int)TicketStatus.New, 
                unwantedBehavior = ticketBehavior, 
                repeatableSteps = ticketSteps,
                // assume userId = 1, Use Identity for this later
                openedBy = 1
            };
            _db.Execute(query, parameters);

            return RedirectToAction("Manage");
        }











        // GET: BugTicketController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BugTicketController/Edit/5
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

        // GET: BugTicketController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BugTicketController/Delete/5
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

        public List<TicketReadable> TicketListToTicketReadableList(List<Ticket> ticketList)
        {
            List<TicketReadable> ret = new List<TicketReadable>();
            if (ticketList.Count == 0)
            {
                // There are no tickets.
                return ret;

            }

            string getTicketQuery = "SELECT * FROM Tickets WHERE Tickets.Id = @ticketId;";
            string getProjectTitleQuery = "SELECT Projects.Title FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @ticketId );";
            string getTicketOpenedByName = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Tickets.OpenedBy FROM Tickets WHERE Tickets.Id = @ticketId );";


            // Begin processing the list.
            foreach (Ticket item in ticketList)
            {
                var parameter = new { ticketId = item.Id };
                TicketReadable readable = new TicketReadable(
                    _db.Query<Ticket>( getTicketQuery, parameter).First(),
                    // Create the OpenedBy string.
                    _db.Query<string>(  getProjectTitleQuery, parameter).First(),
                    _db.Query<string>( getTicketOpenedByName, parameter).First()
                );

                ret.Add(readable);
            }

            return ret;
        }
    }
}

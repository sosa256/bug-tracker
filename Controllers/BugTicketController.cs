using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Dapper;
using Dapper.Contrib.Extensions;
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

        // GET: BugTicket/Manage
        public ActionResult Manage()
        {
            BugTicketViewModel model = new BugTicketViewModel()
            {
                // Get all tickets' current version
                bugTicketReadableList = TicketListToTicketReadableList(_db.Query<Ticket>("SELECT * FROM Tickets WHERE Tickets.IsCurr = 1").ToList())
            };
            return View(model);
        }

        // GET: BugTicket/ManageSpecific/5
        public ActionResult ManageSpecific(int projId)
        {
            string getTicketFromProjectQuery = "SELECT * FROM Tickets WHERE Tickets.IsCurr = 1 AND Tickets.ProjectParent = @projectId";
            var parameter = new { projectId = projId };

            BugTicketViewModel model = new BugTicketViewModel()
            {
                // Get all tickets' current version
                bugTicketReadableList = TicketListToTicketReadableList(_db.Query<Ticket>(getTicketFromProjectQuery, parameter).ToList())
            };
            return View("Manage", model);
        }

        // GET: BugTicketController/Details/5
        [HttpGet]
        public ActionResult Details(int ticketId)
        {
            string getMostCurrTicketId = "SELECT Tickets.Id FROM Tickets WHERE Tickets.HistoryId IN ( SELECT Tickets.HistoryId FROM Tickets WHERE Tickets.Id = @queryTicketId) ORDER BY Tickets.Id DESC;";
            int mostCurrentTicketId = _db.Query<int>(getMostCurrTicketId, new { queryTicketId = ticketId }).FirstOrDefault();

            if (mostCurrentTicketId == 0)
            {
                return Content("Whoops that ticket doesn't exist!");
            }

            if (ticketId != mostCurrentTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }


            var parameterTicketId = new { queryTicketId = ticketId };
            string getTicketQuery = "SELECT * FROM Tickets WHERE Tickets.Id = @queryTicketId;";
            /* SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName
             * FROM BTUsers
             * WHERE BTUsers.Id IN
             * (
	         *     SELECT Tickets.OpenedBy
	         *     FROM Tickets
	         *     WHERE Tickets.Id = 1
             * );
             */
            string getTicketOpenedByName = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Tickets.OpenedBy FROM Tickets WHERE Tickets.Id = @queryTicketId );";
            /* SELECT * 
             * FROM Projects 
             * WHERE Projects.Id IN 
             * (
	         *    SELECT Tickets.Id
	         *    FROM Tickets
	         *    WHERE Tickets.Id = 1
             *  );
             */
            string getProjectQuery = "SELECT * FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @queryTicketId );";
            string getProjectTitleQuery = "SELECT Projects.Title FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @queryTicketId );";

            // DEBUGGING
            //Project test = _db.Query<Project>(getProjectQuery, parameterTicketId).First();
            //Ticket tr = _db.Query<Ticket>(getTicketQuery, parameterTicketId).First();
            //string st = _db.Query<string>(getProjectTitleQuery, parameterTicketId).First();
            //string msg = _db.Query<string>(getTicketOpenedByName, parameterTicketId).First();

            BugTicketDetailsViewModel model = new BugTicketDetailsViewModel()
            {
                //currTicket = _db.Query<Ticket>(      getTicketQuery, parameterTicketId ).First(),
                parentProject = _db.Query<Project>(getProjectQuery, parameterTicketId).First(),
                currTicketReadable = new TicketReadable(
                    _db.Query<Ticket>(getTicketQuery, parameterTicketId).First(),
                    _db.Query<string>(getProjectTitleQuery, parameterTicketId).First(),
                    _db.Query<string>(getTicketOpenedByName, parameterTicketId).First()
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
            int projId = Int32.Parse(collection["newTicket.ProjectParent"].ToString());
            string ticketTitle = collection["newTicket.Title"].ToString();
            int ticketSeverity = Int32.Parse(collection["newTicket.Severity"].ToString());
            string ticketBehavior = collection["newTicket.UnwantedBehavior"].ToString();
            string ticketSteps = collection["newTicket.RepeatableSteps"].ToString();
            DateTime date = DateTime.Now;

            // Generate ticketHistoryId.
            // ticketId (primary key) is generated by the DB.
            string query = "SELECT Tickets.HistoryId FROM Tickets ORDER BY Tickets.HistoryId DESC;";
            // Make sure there is something in the database. Default is 0.
            int extractedMax = _db.Query<int>(query).FirstOrDefault();
            int ticketHistoryId = extractedMax + 1;


            // Add Ticket to myDB.
            query = "INSERT INTO[Tickets](ProjectParent, HistoryId, IsCurr, Title, Severity, Status, UnwantedBehavior, RepeatableSteps, OpenedBy, DateCreated) VALUES( @projParent, @historyId, @isCurr, @title, @severity, @status, @unwantedBehavior, @repeatableSteps, @openedBy, @dateCreated );";
            var parameters = new {
                projParent = projId,
                historyId = ticketHistoryId,
                isCurr = Convert.ToInt32(true),
                title = ticketTitle,
                severity = ticketSeverity,
                status = (int)TicketStatus.New,
                unwantedBehavior = ticketBehavior,
                repeatableSteps = ticketSteps,
                // TODO: assume userId = 1, Use Identity for this later
                openedBy = 1,
                dateCreated = date
            };
            _db.Execute(query, parameters);

            return RedirectToAction("Manage");
        }

        // GET: BugTicket/History/5
        public ActionResult History(int historyId)
        {
            string getMostCurrTicketId = "SELECT Tickets.Id FROM Tickets WHERE Tickets.HistoryId = @histId ORDER BY Tickets.Id DESC;";
            string getTicketHistory = "SELECT * FROM Tickets WHERE Tickets.HistoryId = @histId ORDER BY Tickets.Id DESC;";
            var parameter = new { histId = historyId };

            BugTicketHistoryViewModel model = new BugTicketHistoryViewModel()
            {
                // Get a ticket's history from oldest to newest.
                ticketHistory = TicketListToTicketReadableList(_db.Query<Ticket>(getTicketHistory, parameter).ToList()),
                mostCurrTicketId = _db.Query<int>(getMostCurrTicketId, parameter).First()
            };

            return View(model);
        }







        // GET: BugTicketController/Edit/5
        public ActionResult Edit(int ticketId)
        {
            string getTicketQuery = "SELECT * FROM Tickets WHERE Tickets.Id = @queryTicketId";
            var parameter = new { queryTicketId = ticketId };

            BugTicketEditViewModel model = new BugTicketEditViewModel()
            {
                currTicket = TicketToTicketReadable(_db.Query<Ticket>(getTicketQuery, parameter).First())
            };

            return View(model);
        }

        // POST: BugTicketController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IFormCollection collection)
        {
            int ticketId = Int32.Parse(collection["currTicket.Id"].ToString());
            Ticket ogTicket = _db.Query<Ticket>("SELECT * FROM Tickets WHERE Tickets.Id = @queryTicketId", new { queryTicketId = ticketId }).First();

            // Make the predecesor not current.
            string makeTicketNotCurrentQuery = "UPDATE Tickets SET Tickets.IsCurr = 0 WHERE Tickets.Id = @queryTicketId;";
            var parameter = new { queryTicketId = ticketId };
            _db.Execute(makeTicketNotCurrentQuery, parameter);

            // Make a new ticket with updated info.
            string insertUpdatedTicket = "INSERT INTO Tickets ( ProjectParent, HistoryId, IsCurr, Title, Severity, Status, UnwantedBehavior, RepeatableSteps, OpenedBy, DateCreated )  VALUES( @projectParent, @historyId, @isCurr, @title, @severity, @status, @unwantedBehavior, @repeatableSteps, @openedBy, @dateCreated );";
            var parameters = new {
                projectParent = ogTicket.ProjectParent,
                historyId = ogTicket.HistoryId,
                isCurr = Convert.ToInt32(true),
                title = collection["currTicket.Title"],
                severity = collection["currTicket.Severity"],
                status = (int)TicketStatus.InProgress,
                unwantedBehavior = collection["currTicket.UnwantedBehavior"],
                repeatableSteps = collection["currTicket.RepeatableSteps"],
                openedBy = ogTicket.OpenedBy,
                dateCreated = DateTime.Now
            };

            // Insert new Ticket to myDB.
            _db.Execute(insertUpdatedTicket, parameters);

            return RedirectToAction("Manage");
        }

        // GET: BugTicket/Delete/5
        // TODO: Make this avalible only to original owner and Admin
        public IActionResult Delete(int ticketId)
        {
            // Delete Ticket.
            string deleteTicketQuery = "DELETE FROM Tickets WHERE Tickets.Id = @queryTicketId ;";
            var ticketDeleteParameters = new { queryTicketId = ticketId };
            _db.Execute(deleteTicketQuery, ticketDeleteParameters);

            return RedirectToAction("Manage");
        }







        // GET: BugTicket/Close/5
        [HttpGet]
        public ActionResult Close(int ticketId)
        {
            string getTicketQuery = "SELECT * FROM Tickets WHERE Tickets.Id = @queryTicketId";
            var parameters = new { queryTicketId = ticketId };

            TicketCloseViewModel model = new TicketCloseViewModel()
            {
                ticketToClose = _db.Query<Ticket>(getTicketQuery, parameters).First(),
                emptyCloseModel = new ClosedTicket()
            };

            return View(model);
        }

        // GET: BugTicket/Close/5
        [HttpPost]
        public ActionResult Close(int ticketId, IFormCollection collection)
        {
            string cause = collection["emptyCloseModel.UnwantedBehaviorCause"];
            string solution = collection["emptyCloseModel.UnwantedBehaviorSolution"];
            bool isTempSolution = Convert.ToBoolean( collection["emptyCloseModel.IsTemp"].ToArray()[0] );

            if (! isTempSolution)
            {
                // Solution is permanent.
                // Make the ticket not current so it doesn't appear on Manage.
                string makeTicketNotCurrQuery = "UPDATE Tickets SET Tickets.IsCurr = 0 WHERE Tickets.Id = @queryTicketId ;";
                var parameter = new { queryTicketId = ticketId };
                _db.Execute(makeTicketNotCurrQuery, parameter);
            }
            

            // Update ticket Status to TempSolution or Complete.
            string updateTicketStatusQuery = "UPDATE TICKETS SET Tickets.Status = @ticketStatus WHERE Tickets.Id = @queryTicketId;";
            // Using the ternary conditional operator to avoid a bunch of brackets.
            int newStatus = isTempSolution ? (int)TicketStatus.TempSolution : (int)TicketStatus.Complete;
            var parameters = new { ticketStatus = newStatus, queryTicketId = ticketId };
            _db.Execute(updateTicketStatusQuery, parameters);

            // Close ticket in myDB.
            string getTicketQuery = "SELECT * FROM Tickets WHERE Tickets.Id = @queryTicketId";
            Ticket ticket = _db.Query<Ticket>(getTicketQuery, new { queryTicketId = ticketId} ).First();
            ClosedTicket closedTicket = new ClosedTicket()
            {
                ProjectParent = ticket.ProjectParent,
                TicketClosed = ticket.Id,
                UnwantedBehaviorCause = cause,
                UnwantedBehaviorSolution = solution,
                IsTemp = isTempSolution,
                DateClosed = DateTime.Now,
                // Replace value with logged in user's Id.
                UserWhoClosed = 1
            };
            _db.Insert<ClosedTicket>(closedTicket);

            return RedirectToAction("Manage");
        }














        public List<TicketReadable> TicketListToTicketReadableList(List<Ticket> ticketList)
        {
            List<TicketReadable> ret = new List<TicketReadable>();
            if (ticketList.Count == 0)
            {
                // There are no tickets.
                return ret;

            }

            // Begin processing the list.
            foreach (Ticket item in ticketList)
            {
                ret.Add( TicketToTicketReadable(item) );
            }

            return ret;
        }

        public TicketReadable TicketToTicketReadable( Ticket ticket )
        {
            string getProjectTitleQuery = "SELECT Projects.Title FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @ticketId );";
            string getTicketOpenedByName = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Tickets.OpenedBy FROM Tickets WHERE Tickets.Id = @ticketId );";

            var parameter = new { ticketId = ticket.Id };
            TicketReadable readable = new TicketReadable(
                ticket,
                // Create the OpenedBy string.
                _db.Query<string>(getProjectTitleQuery, parameter).First(),
                _db.Query<string>(getTicketOpenedByName, parameter).First()
            );

            return readable;
        }
    }
}

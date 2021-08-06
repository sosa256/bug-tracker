using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class ClosedTicketController : Controller
    {
        // PROPERTIES
        private SqlConnection _db;




        // CONSTRUCTORS
        public ClosedTicketController()
        {
            _db = DbHelper.GetConnection();
        }




        // ACTIONS / METHODS

        // GET: ClosedTicket/Index
        [HttpGet]
        public IActionResult Index()
        {
            // Get List of closed Tickets.
            string getClosedTicketsQuery = "SELECT * FROM ClosedTickets;";
            List<ClosedTicket> closedTicketList = _db.Query<ClosedTicket>(getClosedTicketsQuery).ToList();

            ClosedTicketViewModel model = new ClosedTicketViewModel()
            {
                // Make list readable.
                closedTicketList = ClosedTicketListToReadableList(closedTicketList)
            };

            return View(model);
        }

        // GET: ClosedTicket/Details
        [HttpGet]
        public IActionResult Details(int ticketId, int projectId)
        {
            string getClosedTicketQuery = "SELECT * FROM ClosedTickets WHERE ClosedTickets.ProjectParent = @projId AND ClosedTickets.TicketClosed = @queryticketId ;";
            string getUnwantedBehaviorQuery = "SELECT Tickets.UnwantedBehavior FROM Tickets WHERE Tickets.Id = @queryticketId ;";
            string getCauseQuery = "SELECT ClosedTickets.UnwantedBehaviorCause FROM ClosedTickets WHERE ClosedTickets.ProjectParent = @projId AND ClosedTickets.TicketClosed = @queryticketId ;";
            string getSolutionQuery = "SELECT ClosedTickets.UnwantedBehaviorSolution FROM ClosedTickets WHERE ClosedTickets.ProjectParent = @projId AND ClosedTickets.TicketClosed = @queryticketId ;";

            // I know I could use just the first one.
            // Copied them over so that names make sense. 
            var getClosedTicketParameters = new { projId = projectId, queryticketId = ticketId };
            var getUnwantedBehaviorParameters = new { queryticketId = ticketId };
            var getCauseParameters = getClosedTicketParameters;
            var getSolutionParameters = getClosedTicketParameters;

            ClosedTicket currClosedTicket = _db.Query<ClosedTicket>(getClosedTicketQuery, getClosedTicketParameters).First();

            string unwantedBehavior = _db.Query<string>(getUnwantedBehaviorQuery, getUnwantedBehaviorParameters).First();
            string unwantedBehaviorCause = _db.Query<string>(getCauseQuery, getCauseParameters).First();
            string unwantedBehaviorSolution = _db.Query<string>(getSolutionQuery, getSolutionParameters).First();

            ClosedTicketDetailsViewModel model = new ClosedTicketDetailsViewModel()
            {
                closedTicketReadable = ClosedTicketToReadable(currClosedTicket),
                UnwantedBehavior = unwantedBehavior,
                UnwantedBehaviorCause = unwantedBehaviorCause,
                UnwantedBehaviorSolution = unwantedBehaviorSolution
            };

            return View(model);
        }

        public IActionResult Delete(int ticketId, int projectId)
        {
            // Delete Closed Ticket.
            string deleteClosedTicketQuery = "DELETE FROM ClosedTickets WHERE ClosedTickets.ProjectParent = @projId AND ClosedTickets.TicketClosed = @queryTicketId ;";
            var closedDeleteParameters = new { queryTicketId = ticketId, projId = projectId };
            _db.Execute(deleteClosedTicketQuery, closedDeleteParameters);

            // Also delete original Ticket.
            string deleteTicketQuery = "DELETE FROM Tickets WHERE Tickets.Id = @queryTicketId ;";
            var ticketDeleteParameters = new { queryTicketId = ticketId };
            _db.Execute(deleteTicketQuery, ticketDeleteParameters);

            return RedirectToAction("Index");
        }






        public List<ClosedTicketReadable> ClosedTicketListToReadableList(List<ClosedTicket> closedTicketList)
        {
            List<ClosedTicketReadable> ret = new List<ClosedTicketReadable>();
            if (closedTicketList.Count == 0)
            {
                // There are no closed tickets.
                return ret;
            }

            // Begin processing the list.
            foreach (ClosedTicket item in closedTicketList)
            {
                ret.Add(ClosedTicketToReadable(item));
            }

            return ret;
        }


        public ClosedTicketReadable ClosedTicketToReadable(ClosedTicket closedTicket)
        {
            string getProjectTitleQuery = "SELECT Projects.Title FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @ticketId );";
            string getTicketTitleQuery = "SELECT Tickets.Title FROM Tickets WHERE Tickets.Id = @ticketId;";
            string getUserWhoClosedNameQuery = "SELECT CONCAT([BTUsers].[FirstName], ' ',  [BTUsers].LastName) AS FullName FROM [BTUsers] WHERE [BTUsers].[Id] IN ( 	SELECT ClosedTickets.UserWhoClosed 	FROM ClosedTickets 	WHERE ClosedTickets.ProjectParent = @projId AND ClosedTickets.TicketClosed = @ticketId );";
            string getTicketOpenedByName = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Tickets.OpenedBy FROM Tickets WHERE Tickets.Id = @ticketId );";

            var parameter = new { ticketId = closedTicket.TicketClosed };
            var userWhoClosedParameter = new { projId = closedTicket.ProjectParent, ticketId = closedTicket.TicketClosed };

            string projName = _db.Query<string>(getProjectTitleQuery, parameter).First();
            string ticketName = _db.Query<string>(getTicketTitleQuery, parameter).First();
            string closedName = _db.Query<string>(getUserWhoClosedNameQuery, userWhoClosedParameter).First(); ;
            string openedName = _db.Query<string>(getTicketOpenedByName, parameter).First();

            // Create readable.
            ClosedTicketReadable ret = new ClosedTicketReadable(closedTicket, projName, ticketName, closedName, openedName);

            return ret;
        }
    }
}

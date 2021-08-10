using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BugTracker.Controllers
{
    public class ClosedTicketController : Controller
    {
        // PROPERTIES
        private readonly SqlHelper _sqlHelper;




        // CONSTRUCTORS
        public ClosedTicketController()
        {
            _sqlHelper = new SqlHelper();
        }




        // ACTIONS / METHODS

        // GET: ClosedTicket/Index
        [HttpGet]
        public IActionResult Index()
        {
            // Get List of closed Tickets.
            List<ClosedTicket> closedTicketList = _sqlHelper.SelectAllClosedTickets();
            List<ClosedTicketReadable> closedTicketRList = ClosedTicketListToReadableList(closedTicketList);

            ClosedTicketViewModel model = new ClosedTicketViewModel(closedTicketRList);

            return View(model);
        }


        // GET: ClosedTicket/Details
        [HttpGet]
        public IActionResult Details(int ticketId, int projectId)
        {
            // Validate input.
            Ticket ticketThatWasClosed = _sqlHelper.SelectTicket(ticketId);
            if (ticketThatWasClosed == null)
            {
                return Content("Whoops the ticket doesn't exist");
            }
            int mostCurrTicketId = _sqlHelper.SelectMostCurrentTicketInHistory(ticketId);
            if (ticketId != mostCurrTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }
            Project project = _sqlHelper.SelectProject(projectId);
            if (project == null)
            {
                return Content("Whoops the project doesn't exist");
            }
            // The TicketId and ProjectId must be valid here.

            ClosedTicketReadable currClosedTicketR = ClosedTicketToReadable(_sqlHelper.SelectClosedTicket(projectId, ticketId));
            
            ClosedTicketDetailsViewModel model = new ClosedTicketDetailsViewModel(currClosedTicketR, ticketThatWasClosed);

            return View(model);
        }


        // POST: ClosedTicket/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int ticketId, int projectId)
        {
            // Delete Closed Ticket.
            _sqlHelper.DeleteClosedTicket(projectId, ticketId);

            // Also delete original Ticket.
            _sqlHelper.DeleteTicket(ticketId);

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
            // Get the properties for a Readable Closed Ticket.
            Ticket ticketThatWasClosed = _sqlHelper.SelectTicket(closedTicket.TicketClosed);
            Project projectThatHeldTheTicket = _sqlHelper.SelectProject(ticketThatWasClosed.ProjectParent);
            string projectTitle = projectThatHeldTheTicket.Title;
            
            string ticketTitle = ticketThatWasClosed.Title;
            string closedName = _sqlHelper.GetUserFullName(closedTicket.UserWhoClosed);
            string openedName = _sqlHelper.GetUserFullName(ticketThatWasClosed.OpenedBy);

            // Create readable.
            ClosedTicketReadable ret = new ClosedTicketReadable(closedTicket, projectTitle, ticketTitle, closedName, openedName);

            return ret;
        }
    }
}

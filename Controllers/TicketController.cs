using BugTracker.Areas.Identity.Data;
using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Readable;
using BugTracker.ViewModels;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Administrator, Developer, Submitter")]
    public class TicketController : Controller
    {
        // PROPERTIES
        private readonly SqlHelper _sqlHelper;
        private readonly UserManager<BugTrackerUser> _userManager;
        private readonly SqlConnection _db;




        // CONSTRUCTORS
        public TicketController(UserManager<BugTrackerUser> userManager)
        {
            _sqlHelper = new SqlHelper();
            _userManager = userManager;
            _db = DbHelper.GetConnection();
        }




        // ACTIONS / METHODS

        // GET: Ticket/ManageSpecific/5
        [HttpGet]
        public ActionResult ManageSpecific(int projId)
        {
            // Verify the project exists.
            Project currProject = _sqlHelper.SelectProject(projId);
            if (currProject == null)
            {
                return Content("Whoops that project doesn't exist!");
            }
            // If we are here the project is valid.

            List<TicketReadable> ticketList = TicketListToReadable( _sqlHelper.SelectAllCurrentTickets(projId) );

            TicketViewModel model = new TicketViewModel(ticketList);
            
            return View("Manage", model);
        }


        // GET: Ticket/Create
        [HttpGet]
        public ActionResult Create()
        {
            List<TicketCreateViewModel.ProjectOption> projectOptions = _sqlHelper.SelectProjectIdAndTitle();

            TicketCreateViewModel model = new TicketCreateViewModel(projectOptions);

            return View(model);
        }


        // GET: Ticket/Edit/5
        [HttpGet]
        public ActionResult Edit(int ticketId)
        {
            // Verify the Ticket is valid.
            Ticket ticketToEdit = _sqlHelper.SelectTicket(ticketId);
            if (ticketToEdit == null)
            {
                return Content("Whoops that ticket doesn't exist!");
            }

            int mostCurrTicketId = _sqlHelper.SelectMostCurrentTicketInHistory(ticketId);

            if (ticketId != mostCurrTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }
            // If we are here the ticketId is valid.

            TicketReadable ticketToEditR = TicketToReadable(ticketToEdit);

            TicketEditViewModel model = new TicketEditViewModel(ticketToEditR);

            return View(model);
        }


        // GET: Ticket/Details/5
        [HttpGet]
        public ActionResult Details(int ticketId)
        {
            // Verify the Ticket is valid.
            // In case they tampered with the query string.
            int mostCurrTicketId = _sqlHelper.SelectMostCurrentTicketInHistory(ticketId);
            
            if (mostCurrTicketId == 0)
            {
                return Content("Whoops that ticket doesn't exist!");
            }

            if (ticketId != mostCurrTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }
            // If we are here the ticketId is valid.

            // Make the current ticket Readable.
            Ticket currTicket = _sqlHelper.SelectTicket(ticketId);
            // Get the readable properties.
            string userOpenedName = _sqlHelper.GetUserFullName(currTicket.OpenedBy);
            string projectTitle = _sqlHelper.SelectProjectFromTicket(ticketId).Title;
            TicketReadable currTicketReadable = new TicketReadable(currTicket, projectTitle, userOpenedName);
            
            // Get properties of view model.
            Project projectParent = _sqlHelper.SelectProjectFromTicket(ticketId);
            CommentController commentController = new CommentController(_userManager);
            List<CommentReadable> commentList = commentController.CommentListToReadable (_sqlHelper.SelectCommentsFromTicket(ticketId));

            TicketDetailsViewModel model = new TicketDetailsViewModel(currTicketReadable, projectParent, commentList);
            model.UserViewingId = _sqlHelper.SelectUserFromStringId(_userManager.GetUserId(User)).Id;

            return View(model);
        } // Details(int)


        public ActionResult DetailsError(string erroneousMsg, int ticketId)
        {
            // Verify the Ticket is valid.
            // In case they tampered with the query string.
            int mostCurrTicketId = _sqlHelper.SelectMostCurrentTicketInHistory(ticketId);

            if (mostCurrTicketId == 0)
            {
                return Content("Whoops that ticket doesn't exist!");
            }

            if (ticketId != mostCurrTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }
            // If we are here the ticketId is valid.

            // Make the current ticket Readable.
            Ticket currTicket = _sqlHelper.SelectTicket(ticketId);
            // Get the readable properties.
            string userOpenedName = _sqlHelper.GetUserFullName(currTicket.OpenedBy);
            string projectTitle = _sqlHelper.SelectProjectFromTicket(ticketId).Title;
            TicketReadable currTicketReadable = new TicketReadable(currTicket, projectTitle, userOpenedName);

            // Get properties of view model.
            Project projectParent = _sqlHelper.SelectProjectFromTicket(ticketId);
            CommentController commentController = new CommentController(_userManager);
            List<CommentReadable> commentList = commentController.CommentListToReadable(_sqlHelper.SelectCommentsFromTicket(ticketId));

            TicketDetailsViewModel model = new TicketDetailsViewModel(currTicketReadable, projectParent, commentList);
            model.ErrorExists = true;
            model.newComment.Msg = erroneousMsg;
            model.UserViewingId = _sqlHelper.SelectUserFromStringId(_userManager.GetUserId(User)).Id;

            return View("Details", model);
        }


        public ActionResult Details(TicketDetailsViewModel model)
        {
            return View(model);
        }

        // GET: Ticket/Close/5
        [HttpGet]
        [Authorize(Roles = "Administrator, Developer")]
        public ActionResult Close(int ticketId)
        {
            // Verify the Ticket is valid.
            int mostCurrTicketId = _sqlHelper.SelectMostCurrentTicketInHistory(ticketId);

            if (mostCurrTicketId == 0)
            {
                return Content("Whoops that ticket doesn't exist!");
            }

            if (ticketId != mostCurrTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }
            // If we are here the ticketId is valid.

            Ticket ticketToClose = _sqlHelper.SelectTicket(ticketId);

            TicketCloseViewModel model = new TicketCloseViewModel(ticketToClose);

            return View(model);
        }


        // POST: Ticket/Close/5
        [HttpPost]
        [Authorize(Roles = "Administrator, Developer")]
        [ValidateAntiForgeryToken]
        public ActionResult Close(int ticketId, IFormCollection collection)
        {
            // ValidateAntiForgeryToken should take care of any ticketId tampering.

            // Extract user input from form.
            string cause = collection["emptyCloseModel.UnwantedBehaviorCause"];
            string solution = collection["emptyCloseModel.UnwantedBehaviorSolution"];
            bool isTempSolution = Convert.ToBoolean(collection["emptyCloseModel.IsTemp"].ToArray()[0]);

            // Validate cause and solution strings.
            bool causeIsOutOfRange = cause.Length < 3 || cause.Length > 255;
            bool solutionIsOutOfRange = solution.Length < 3 || solution.Length > 255;
            if (causeIsOutOfRange || solutionIsOutOfRange)
            {
                // Length is out of range.
                // Set up the model.
                Ticket ticketToClose = _sqlHelper.SelectTicket(ticketId);
                TicketCloseViewModel model = new TicketCloseViewModel(ticketToClose);
                if (causeIsOutOfRange)
                {
                    model.CauseErrorMsg = "The cause must contain between 3 and 255 characters";
                }
                if(solutionIsOutOfRange)
                {
                    model.SolutionErrorMsg = "The solution must contain between 3 and 255 characters";
                }
                model.ErrorExists = true;
                model.emptyCloseModel.UnwantedBehaviorCause = cause;
                model.emptyCloseModel.UnwantedBehaviorSolution = solution;

                return View("Close", model);
            }
            // User input must be valid here.


            if (!isTempSolution)
            {
                // Solution is permanent.
                // Make the ticket not current so it doesn't appear on Manage.
                _sqlHelper.MakeTicketNotCurrent(ticketId);
            }

            // Update ticket Status to TempSolution or Complete.
            // Using the ternary conditional operator to avoid a bunch of brackets.
            TicketStatus newStatus = isTempSolution ? TicketStatus.TempSolution : TicketStatus.Complete;
            _sqlHelper.UpdateTicketStatus(ticketId, newStatus);

            // Add close ticket in myDB.
            Ticket ticket = _sqlHelper.SelectTicket(ticketId);
            ClosedTicket closedTicket = new ClosedTicket(
                ticket.ProjectParent, ticketId, cause, 
                solution, isTempSolution, DateTime.Now,
                _sqlHelper.SelectUserFromStringId( _userManager.GetUserId(User) ).Id
            );

            // Didn't turn it into a _sqlHelper method b/c it's already one line.
            _db.Insert<ClosedTicket>(closedTicket);

            return RedirectToAction("Manage");
        } // END OF: Close(int , IFormCollection)


        // GET: Ticket/History/5
        public ActionResult History(int historyId)
        {
            // Verify historyId exist.
            bool historyDoesNotExit = ! _sqlHelper.SelectAllHistoryIds().Contains(historyId);
            if (historyDoesNotExit)
            {
                return Content("Whoops the ticket doesn't exist so neither does it's history!");
            }
            // historyId must be valid.

            List<TicketReadable> ticketHistory = TicketListToReadable(_sqlHelper.SelectTicketHistory(historyId));
            int currentTicketId = _sqlHelper.SelectCurrentTicketIdInHistory(historyId);

            TicketHistoryViewModel model = new TicketHistoryViewModel(ticketHistory, currentTicketId);

            return View(model);
        }


        public List<TicketReadable> TicketListToReadable(List<Ticket> ticketList)
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
                ret.Add(TicketToReadable(item));
            }

            return ret;
        }

        public TicketReadable TicketToReadable(Ticket ticket)
        {
            // Create the readable properties.
            string title = _sqlHelper.SelectProject(ticket.ProjectParent).Title;
            string openedBy = _sqlHelper.GetUserFullName(ticket.OpenedBy);

            TicketReadable readable = new TicketReadable( ticket, title, openedBy );

            return readable;
        }


// CRUD operations for ticket.
        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            int projId = Int32.Parse(collection["newTicket.ProjectParent"].ToString());
            string ticketTitle = collection["newTicket.Title"].ToString();
            int ticketSeverity = Int32.Parse(collection["newTicket.Severity"].ToString());
            string ticketBehavior = collection["newTicket.UnwantedBehavior"].ToString();
            string ticketSteps = collection["newTicket.RepeatableSteps"].ToString();

            // Validate form input.
            // Integers are fine since they are from a list and ValidateAntiForgeryToken prevents tampering.
            bool titleIsOutOfRange = ticketTitle.Length < 3 || ticketTitle.Length > 255;
            bool behaviorIsOutOfRange = ticketBehavior.Length < 3 || ticketBehavior.Length > 255;
            bool stepsIsOutOfRange = ticketSteps.Length < 3 || ticketSteps.Length > 255;
            if (titleIsOutOfRange || behaviorIsOutOfRange || stepsIsOutOfRange)
            {
                // Length is out of range.
                // Set up the model.
                Ticket erroneousNewTicket = new Ticket(
                    projId, -1, -1, true,
                    ticketTitle, ticketSeverity, (int)TicketStatus.New, ticketBehavior,
                    ticketSteps, -1, DateTime.Now
                );
                TicketCreateViewModel model = new TicketCreateViewModel(erroneousNewTicket, _sqlHelper.SelectProjectIdAndTitle());

                if (titleIsOutOfRange)
                {
                    model.TitleErrorMsg = "The title must contain between 3 and 255 characters";
                }
                if (behaviorIsOutOfRange)
                {
                    model.BehaviorErrorMsg = "The behavior must contain between 3 and 255 characters";
                }
                if (stepsIsOutOfRange)
                {
                    model.StepsErrorMsg = "The steps must contain between 3 and 255 characters";
                }
                model.ErrorExists = true;

                return View("Create", model);
            }
            // User input must be valid here.


            // Generate Ids.
            int ticketHistoryId = _sqlHelper.GenerateHistoryId();
            int ticketId = _sqlHelper.GenerateTicketId();
            int userWhoOpened =  _sqlHelper.SelectUserFromStringId(_userManager.GetUserId(User)).Id;

            Ticket newTicket = new Ticket(
                projId, ticketId, ticketHistoryId, true,
                ticketTitle, ticketSeverity, (int)TicketStatus.New, ticketBehavior,
                ticketSteps, userWhoOpened, DateTime.Now
             );

            // Add Ticket to myDB.
            _sqlHelper.InsertTicket(newTicket);

            return RedirectToAction("Manage");
        } // END OF: Create (IFormCollection)


        // GET: Ticket/Manage
        [HttpGet]
        public ActionResult Manage()
        {
            List<TicketReadable> ticketList = TicketListToReadable( _sqlHelper.SelectAllCurrentTickets() );

            TicketViewModel model = new TicketViewModel(ticketList);

            return View(model);
        }


        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IFormCollection collection)
        {
            // Extract form input.
            string title = collection["currTicket.Title"];
            string behavior = collection["currTicket.UnwantedBehavior"];
            string steps = collection["currTicket.RepeatableSteps"];
            int severity = Int32.Parse(collection["currTicket.Severity"].ToString());
            int outdatedTicketId = Int32.Parse(collection["currTicket.Id"].ToString());
            Ticket ogTicket = _sqlHelper.SelectTicket(outdatedTicketId);

            // Validate form input.
            // Integers are fine since they are from a list,
            // hidden from the user anyway,
            // and ValidateAntiForgeryToken prevents tampering.

            bool titleIsOutOfRange = title.Length < 3 || title.Length > 255;
            bool behaviorIsOutOfRange = behavior.Length < 3 || behavior.Length > 255;
            bool stepsIsOutOfRange = steps.Length < 3 || steps.Length > 255;
            if (titleIsOutOfRange || behaviorIsOutOfRange || stepsIsOutOfRange)
            {
                // Length is out of range.
                // Set up the model.
                Ticket erroneousTicket = new Ticket(
                    ogTicket.ProjectParent, outdatedTicketId, ogTicket.HistoryId, true,
                    title, severity, (int)TicketStatus.New, behavior,
                    steps, ogTicket.OpenedBy, DateTime.Now
                );
                TicketEditViewModel model = new TicketEditViewModel( TicketToReadable(erroneousTicket) );

                if (titleIsOutOfRange)
                {
                    model.TitleErrorMsg = "The title must contain between 3 and 255 characters";
                }
                if (behaviorIsOutOfRange)
                {
                    model.BehaviorErrorMsg = "The behavior must contain between 3 and 255 characters";
                }
                if (stepsIsOutOfRange)
                {
                    model.StepsErrorMsg = "The steps must contain between 3 and 255 characters";
                }
                model.ErrorExists = true;

                return View("Edit", model);
            }
            // User input must be valid here.


            int updatedTicketId = _sqlHelper.GenerateTicketId();            
            // Make a new ticket with updated info.
            Ticket updatedTicket = new Ticket(
                ogTicket.ProjectParent, updatedTicketId, ogTicket.HistoryId, true,
                title, severity, (int)TicketStatus.InProgress, behavior,
                steps, ogTicket.OpenedBy, DateTime.Now
            );

            _sqlHelper.UpdateTicket(updatedTicket, outdatedTicketId);

            return RedirectToAction("Manage");
        } // END OF: Edit(IFormCollection)


        // POST: Ticket/Delete/5
        // TODO: Make this avalible only to original owner and Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(IFormCollection collection)
        {
            // Authorize or deny action. 
            // Can only delete if you are the Admin or the og owner.
            int userViewingId = Int32.Parse(collection["UserViewingId"]);
            BTUser user = _sqlHelper.SelectUserFromStringId(_userManager.GetUserId(User));
            bool authorizedAction = (user.Role == (int)BTUserRoles.Administrator) || (user.Id == userViewingId);
            if ( ! authorizedAction)
            {
                return Content("This is an unauthorized action");
            }

            // ValidateAntiForgeryToken makes sure ticketId was not tampered with.
            int ticketId = Int32.Parse(collection["currTicketReadable.Id"]);

            // Delete Ticket.
            _sqlHelper.DeleteTicket(ticketId);

            return RedirectToAction("Manage");
        }
    }
}

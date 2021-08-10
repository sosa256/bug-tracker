using BugTracker.Models;

namespace BugTracker.ViewModels
{
    public class TicketCloseViewModel
    {
        // PROPERTIES
        public Ticket ticketToClose { get; set; }
        public ClosedTicket emptyCloseModel { get; set; }
        public string CauseErrorMsg = ""; 
        public string SolutionErrorMsg = "";
        public bool ErrorExists = false;




        // CONSTRUCTORS
        public TicketCloseViewModel(Ticket ticketToClose)
        {
            this.ticketToClose = ticketToClose;
            this.emptyCloseModel = new ClosedTicket();
        }
    }
}

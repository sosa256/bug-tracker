using BugTracker.Models;

namespace BugTracker.ViewModels
{
    public class ClosedTicketDetailsViewModel
    {
        // PROPERTIES
        public ClosedTicketReadable closedTicketReadable { get; set; }
        public string UnwantedBehavior { get; set; }
        public string UnwantedBehaviorCause { get; set; }
        public string UnwantedBehaviorSolution { get; set; }




        // CONSTRUCTORS
        public ClosedTicketDetailsViewModel(ClosedTicketReadable closedTicket, Ticket ticketThatWasClosed)
        {
            this.closedTicketReadable = closedTicket;
            this.UnwantedBehavior = ticketThatWasClosed.UnwantedBehavior;
            this.UnwantedBehaviorCause = closedTicket.UnwantedBehaviorCause;
            this.UnwantedBehaviorSolution = closedTicket.UnwantedBehaviorSolution;
        }
    }
}

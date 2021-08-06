using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class TicketCloseViewModel
    {
        public Ticket ticketToClose { get; set; }
        public ClosedTicket emptyCloseModel { get; set; }
        // UnwantedBehaviorCause { get; set; }
        // UnwantedBehaviorSolution { get; set; }
        // IsTemp { get; set; }
    }
}

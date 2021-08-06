using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class ClosedTicketDetailsViewModel
    {
        public ClosedTicketReadable closedTicketReadable { get; set; }
        public string UnwantedBehavior { get; set; }
        public string UnwantedBehaviorCause { get; set; }
        public string UnwantedBehaviorSolution { get; set; }
    }
}

using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class ClosedTicketViewModel
    {
        // PROPERTIES
        public List<ClosedTicketReadable> closedTicketList { get; set; }




        // CONSTRUCTORS
        public ClosedTicketViewModel(List<ClosedTicketReadable> ticketList)
        {
            this.closedTicketList = ticketList;
        }
    }
}

using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class TicketHistoryViewModel
    {
        // PROPERTIES
        public List<TicketReadable> ticketHistory { get; set; }
        public int mostCurrTicketId { get; set; }




        // CONSTRUCTORS
        public TicketHistoryViewModel(List<TicketReadable> ticketList, int currentTicketId)
        {
            this.ticketHistory = ticketList;
            this.mostCurrTicketId = currentTicketId;
        }
    }
}

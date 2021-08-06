using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class ClosedTicketReadable : ClosedTicket
    {
        // PROPERTIES
        public string ProjectParentReadable { get; set; }
        public string TicketTitle { get; set; }
        public string UserWhoClosedName { get; set; }
        public string UserWhoOpenedName { get; set; }




        // CONSTRUCTOR
        public ClosedTicketReadable( ClosedTicket closedTicket, string projName, string ticketName, string closedName, string openedName )
        {
            // Only fill-in what is needed

            // ClosedTicket/Index
            this.DateClosed = closedTicket.DateClosed;
            this.TicketClosed = closedTicket.TicketClosed;
            this.ProjectParent = closedTicket.ProjectParent;

            this.ProjectParentReadable = projName;
            this.TicketTitle = ticketName;
            this.UserWhoClosedName = closedName;
            this.UserWhoOpenedName = openedName;
        }

    }
}

using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class ClosedTicketViewModel
    {
        public List<ClosedTicketReadable> closedTicketList { get; set; }
    }
}

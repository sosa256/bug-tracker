using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class BugTicketViewModel
    {
        public List<TicketReadable> bugTicketReadableList { get; set; }
        public List<Project> parentProject { get; set; }
    }
}

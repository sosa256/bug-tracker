using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class TicketViewModel
    {
        // PROPERTIES
        public List<TicketReadable> bugTicketReadableList { get; set; }
        public List<Project> parentProject { get; set; }




        // CONSTRUCTORS
        public TicketViewModel(List<TicketReadable> bugTicketReadableList)
        {
            this.bugTicketReadableList = bugTicketReadableList;
        }
    }
}

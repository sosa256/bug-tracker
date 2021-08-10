using BugTracker.Models;

namespace BugTracker.ViewModels
{
    public class TicketEditViewModel
    {
        // PROPERTIES
        public TicketReadable currTicket { get; set; }
        public string TitleErrorMsg = "";
        public string BehaviorErrorMsg = "";
        public string StepsErrorMsg = "";
        public bool ErrorExists = false;



        // CONSTRUCTORS
        public TicketEditViewModel(TicketReadable currTicket)
        {
            this.currTicket = currTicket;
        }
    }
}

using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class TicketCreateViewModel
    {
        // Internal class.
        public class ProjectOption
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }


        // PROPERTIES
        public Ticket newTicket { get; set; }
        public List<ProjectOption> projOptionList { get; set; }
        public string TitleErrorMsg = "";
        public string BehaviorErrorMsg = "";
        public string StepsErrorMsg = "";
        public bool ErrorExists = false;




        // CONSTRUCTORS
        public TicketCreateViewModel()
        {
        }


        public TicketCreateViewModel(List<ProjectOption> projOptions)
        {
            this.newTicket = new Ticket();
            this.projOptionList = projOptions;
        }

        public TicketCreateViewModel(Ticket errorneousNewTicket, List<ProjectOption> projOptions)
        {
            this.newTicket = errorneousNewTicket;
            this.projOptionList = projOptions;
        }
    }
}

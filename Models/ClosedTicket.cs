using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class ClosedTicket
    {
        // PROPERTIES
        [Key]
        [Display(Name = "Project Parent")]
        public int ProjectParent { get; set; }
        [Key]
        [Display(Name = "Ticket Closed")]
        public int TicketClosed { get; set; }
        [Display(Name = "Unwanted Behavior Cause")]
        public string UnwantedBehaviorCause { get; set; }
        [Display(Name = "Unwanted Behavior Solution")]
        public string UnwantedBehaviorSolution { get; set; }
        [Display(Name = "Is Temp")]
        public bool IsTemp { get; set; }
        [Display(Name = "Date Closed")]
        public DateTime DateClosed{ get; set; }
        [Display(Name = "User Who Closed")]
        public int UserWhoClosed { get; set; }




        // CONSTRUCTORS
        public ClosedTicket()
        {
            ProjectParent = -1;
            TicketClosed = -1;
            UnwantedBehaviorCause = "";
            UnwantedBehaviorSolution = "";
            IsTemp = true;
            DateClosed = DateTime.MinValue;
            UserWhoClosed = -1;
        }


        public ClosedTicket(ClosedTicket closedTicket)
        {
            ProjectParent = closedTicket.ProjectParent;
            TicketClosed = closedTicket.TicketClosed;
            UnwantedBehaviorCause = closedTicket.UnwantedBehaviorCause;
            UnwantedBehaviorSolution = closedTicket.UnwantedBehaviorSolution;
            IsTemp = closedTicket.IsTemp;
            DateClosed = closedTicket.DateClosed;
            UserWhoClosed = closedTicket.UserWhoClosed;
        }

        public ClosedTicket(
            int projectParent, int ticketId,        string cause, 
            string solution,   bool isSolutionTemp, DateTime dateClosed,
            int userIdWhoClosed)
        {
            ProjectParent = projectParent;
            TicketClosed = ticketId;
            UnwantedBehaviorCause = cause;
            UnwantedBehaviorSolution = solution;
            IsTemp = isSolutionTemp;
            DateClosed = dateClosed;
            UserWhoClosed = userIdWhoClosed;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
    }
}

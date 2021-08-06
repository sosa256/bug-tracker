using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public enum TicketStatus
    {
        New = 0,
        [Display(Name = "In Progress")]
        InProgress,
        [Display(Name = "Temporary Solution")]
        TempSolution,
        Complete
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Ticket
    {
        // PROPERTIES
        [Key]
        [Display(Name = "Project Parent")]
        public int ProjectParent { get; set; }
        [Key]
        public int Id { get; set; }
        public int HistoryId { get; set; }
        public bool IsCurr { get; set; }
        [MinLength(3)]
        [MaxLength(255)]
        public string Title { get; set; }
        public int Severity { get; set; }
        public int Status { get; set; }
        [MinLength(3)]
        [MaxLength(1023)]
        [Display(Name = "Unwanted Behavior")]
        public string UnwantedBehavior { get; set; }
        [MinLength(3)]
        [MaxLength(1023)]
        [Display(Name = "Repeatable Steps")]
        public string RepeatableSteps { get; set; }
        public int OpenedBy { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }




        // CONSTRUCTORS
        public Ticket(
            int projectParent,   int id,       int historyId, bool isCurr, 
            string title,        int severity, int status,    string unwantedBehavior, 
            string repeatableSteps, int openedBy, DateTime dateCreated)
        {
            this.ProjectParent = projectParent;
            this.Id = id;
            this.HistoryId = historyId;
            this.IsCurr = isCurr;
            this.Title = title;
            this.Severity = severity;
            this.Status = status;
            this.UnwantedBehavior = unwantedBehavior;
            this.RepeatableSteps = repeatableSteps;
            this.OpenedBy = openedBy;
            this.DateCreated = dateCreated;
        }

        public Ticket( Ticket ticket) 
        {
            this.ProjectParent = ticket.ProjectParent;
            this.Id        = ticket.Id;
            this.HistoryId = ticket.HistoryId;
            this.IsCurr    = ticket.IsCurr;
            this.Title     = ticket.Title;
            this.Severity  = ticket.Severity;
            this.Status    = ticket.Status;
            this.UnwantedBehavior = ticket.UnwantedBehavior;
            this.RepeatableSteps  = ticket.RepeatableSteps;
            this.OpenedBy    = ticket.OpenedBy;
            this.DateCreated = ticket.DateCreated;
        }

        public Ticket() {  }
    }
}

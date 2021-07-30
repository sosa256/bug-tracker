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
    }
}

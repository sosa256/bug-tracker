namespace BugTracker.Models
{
    public class TicketReadable : Ticket
    {
        // PROPERTIES
        public string ProjectParentReadable { get; set; }
        public string SeverityReadable { get; set; }
        public string StatusReadable { get; set; }
        public string OpenedByReadable { get; set; }




        // CONSTRUCTOR
        public TicketReadable(Ticket ticket, string projTitle, string openedByReadable)
        {
            // Figure out if you need to make a constructor in Ticket.cs and call it here.
            this.ProjectParent = ticket.ProjectParent;
            this.Id = ticket.Id;
            this.HistoryId = ticket.HistoryId;
            this.IsCurr = ticket.IsCurr;
            this.Title = ticket.Title;
            this.Severity = ticket.Severity;
            this.Status = ticket.Status;
            this.UnwantedBehavior = ticket.UnwantedBehavior;
            this.RepeatableSteps = ticket.RepeatableSteps;
            this.OpenedBy = ticket.OpenedBy;
            this.DateCreated = ticket.DateCreated;

            this.ProjectParentReadable = projTitle;
            this.SeverityReadable = ((TicketSeverity)this.Severity).ToString();
            this.StatusReadable = ((TicketStatus)this.Status).ToString();
            this.OpenedByReadable = openedByReadable;
        }
    }
}

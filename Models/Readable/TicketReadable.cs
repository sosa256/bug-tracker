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
        public TicketReadable(Ticket ticket, string projTitle, string openedByReadable) : base(ticket)
        {
            this.ProjectParentReadable = projTitle;
            this.SeverityReadable = ((TicketSeverity)this.Severity).ToString();
            this.StatusReadable = ((TicketStatus)this.Status).ToString();
            this.OpenedByReadable = openedByReadable;
        }
    }
}

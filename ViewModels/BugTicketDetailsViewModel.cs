using BugTracker.Models;
using BugTracker.Models.Readable;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class BugTicketDetailsViewModel
    {
        public TicketReadable currTicketReadable;
        public Project parentProject;
        // List of comments for the currentTicket.
        public List<CommentReadable> CommentList { get; set; }
        public Comment newComment { get; set; }
    }
}

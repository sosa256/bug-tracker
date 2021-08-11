using BugTracker.Models;
using BugTracker.Models.Readable;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class TicketDetailsViewModel
    {
        // PROPERTIES
        public TicketReadable currTicketReadable;
        public Project parentProject;


        // List of comments for the currentTicket.
        public List<CommentReadable> CommentList { get; set; }
        public Comment newComment { get; set; }
        public string MsgError = "The comment must contain between 3 and 255 characters";
        public bool ErrorExists = false;

        public int UserViewingId { get; set; }

        // CONSTRUCTORS
        public TicketDetailsViewModel(TicketReadable currTicket, Project parentProject, List<CommentReadable> commentList)
        {
            newComment = new Comment();
            this.currTicketReadable = currTicket;
            this.parentProject = parentProject;
            this.CommentList = commentList;
        }
    }
}

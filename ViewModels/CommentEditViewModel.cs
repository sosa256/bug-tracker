using BugTracker.Models;

namespace BugTracker.ViewModels
{
    public class CommentEditViewModel
    {
        // PROPERTIES
        public Comment currComment { get; set; }
        public int ReturnTicketId { get; set; }
        public bool ErrorExists = false;
        public int UserViewingId { get; set; }



        // CONSTRUCTORS
        public CommentEditViewModel(Comment currComment, int returnTicketId)
        {
            this.currComment = currComment;
            this.ReturnTicketId = returnTicketId;
        }
    }
}

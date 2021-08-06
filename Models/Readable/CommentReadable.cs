
namespace BugTracker.Models.Readable
{
    public class CommentReadable : Comment
    {
        // PROPERTIES
        public string OwnerReadable { get; set; }




        // CONSTRUCTORS
        public CommentReadable( Comment comment, string ownerName )
        {
            this.Id = comment.Id;
            this.Owner = comment.Owner;
            this.TicketId = comment.TicketId;
            this.Msg = comment.Msg;
            this.DateCreated = comment.DateCreated;
            
            this.OwnerReadable = ownerName;
        }

    }
}

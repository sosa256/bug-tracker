
namespace BugTracker.Models.Readable
{
    public class CommentReadable : Comment
    {
        // PROPERTIES
        public string OwnerReadable { get; set; }




        // CONSTRUCTORS
        public CommentReadable( Comment comment, string ownerName ) : base(comment)
        {
            this.OwnerReadable = ownerName;
        }

    }
}

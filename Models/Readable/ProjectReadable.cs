namespace BugTracker.Models
{
    public class ProjectReadable : Project
    {
        // PROPERTIES
        public string OwnerReadable { get; set; }




        // CONSTRUCTOR
        public ProjectReadable( Project proj , string ownerName) 
            : base( proj.Id, proj.Title, proj.Owner )
        {
            this.OwnerReadable = ownerName;
        }
    }
}

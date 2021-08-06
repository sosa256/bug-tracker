namespace BugTracker.Models
{
    public class ProjectReadable : Project
    {
        // PROPERTIES
        public string OwnerReadable { get; set; }




        // CONSTRUCTOR
        public ProjectReadable( Project proj , string ownerName)
        {
            this.Id = proj.Id;
            this.Title = proj.Title;
            this.Owner = proj.Owner;

            this.OwnerReadable = ownerName;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Project
    {
        // PROPERTIES
        public int Id { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 3)]
        [MinLength(3, ErrorMessage = "Title must contain at least three characters")]
        [MaxLength(255, ErrorMessage = "Title must contain a maximum of 255 characters!")]
        public string Title { get; set; }
        public int Owner { get; set; }




        // CONSTRUCTORS
        public Project()
        {
            this.Id = -1;
            this.Title = "";
            this.Owner = -1;
        }


        public Project(int projectId, string title, int ownerId)
        {
            this.Id = projectId;
            this.Title = title;
            this.Owner = ownerId;
        }

        public Project(int projectId, string title) 
        {
            this.Id = projectId;
            this.Title = title;
        }
    }
}

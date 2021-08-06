using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Project
    {
        // PROPERTIES
        public int Id { get; set; }

        [MinLength(3, ErrorMessage = "Title must contain at least three characters")]
        [MaxLength(255, ErrorMessage = "Title must contain a maximum of 255 characters!")]
        public string Title { get; set; }
        public int Owner { get; set; }
    }
}

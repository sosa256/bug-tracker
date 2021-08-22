using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class BTUser
    {
        // PROPERTIES
        [Key]
        public int Id { get; set; }
        public string StringId { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Username must contain at least three characters")]
        [MaxLength(40, ErrorMessage = "Username must contain a maximum of 40 characters!")]
        public string UserName { get; set; }

        [MaxLength(20, ErrorMessage = "First name must contain a maximum of 20 characters!")]
        public string FirstName { get; set; }

        [MaxLength(20, ErrorMessage = "Last name must contain a maximum of 20 characters!")]
        public string LastName { get; set; }

        public int Role { get; set; }
        public int Administrator { get; set; }




        // CONSTRUCTORS
        public BTUser()
        {
            this.Id = 0;
            this.UserName = null;
            this.FirstName = null;
            this.LastName = null;
            this.Role = (int) BTUserRoles.NoRole;
            this.Administrator = 0;
        }

        public BTUser(
            int id, string stringId, 
            string username, string firstName, string lastName,
            int adminId 
        )
        {
            this.Id = id;
            this.StringId = stringId;
            this.UserName = username;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Role = (int)BTUserRoles.NoRole;
            this.Administrator = adminId;
        }

        public BTUser(string username)
        {
            this.UserName = username;
        }

    }
}

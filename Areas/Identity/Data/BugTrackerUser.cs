using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the BugTrackerUser class
    public class BugTrackerUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName ="VARCHAR(20)")]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [PersonalData]
        [Column(TypeName = "VARCHAR(20)")]
        [MaxLength(20)]
        public string LastName { get; set; }
        
    }
}

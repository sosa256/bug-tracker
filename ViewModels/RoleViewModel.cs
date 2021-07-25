using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class RoleViewModel
    {
        // PROPERTIES
        public List<Microsoft.AspNetCore.Identity.IdentityRole> roleList;
        public Microsoft.AspNetCore.Identity.IdentityRole currRole;
    }
}

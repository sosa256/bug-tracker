using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class RoleViewModel
    {
        // PROPERTIES
        public List<IdentityRole> roleList;
        public IdentityRole currRole;
        public bool newRoleHasNoTitle;




        // CONSTRUCTORS
        public RoleViewModel(List<IdentityRole> roleList, bool newRoleHasNoTitle)
        {
            this.roleList = roleList;
            this.newRoleHasNoTitle = newRoleHasNoTitle;
        }

        public RoleViewModel(IdentityRole currRole)
        {
            this.currRole = currRole;
        }

        public RoleViewModel(IdentityRole currRole, bool newRoleHasNoTitle)
        {
            this.currRole = currRole;
            this.newRoleHasNoTitle = newRoleHasNoTitle;
        }
    }
}

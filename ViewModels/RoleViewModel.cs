using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class RoleViewModel
    {
        // PROPERTIES
        public List<Microsoft.AspNetCore.Identity.IdentityRole> roleList;
        public Microsoft.AspNetCore.Identity.IdentityRole currRole;

        // CONSTRUCTORS
        //public RoleViewModel(List<Microsoft.AspNetCore.Identity.IdentityRole> list)
        //{
        //    roleList = list;
        //}
    }
}

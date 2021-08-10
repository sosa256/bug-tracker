using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    // Copy of ProjectDetailsViewModel.
    // freeUsers is a more accurate name than usersAssigned.
    public class ProjectsAssignViewModel
    {
        public Project project;
        public List<BTUser> freeUsers;




        // CONSTRUCTORS
        public ProjectsAssignViewModel(Project project, List<BTUser> userList)
        {
            this.project = project;
            this.freeUsers = userList;
        }
    }
}

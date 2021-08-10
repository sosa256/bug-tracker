using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class ProjectDetailsViewModel
    {
        // PROPERTIES
        public Project project;
        public List<BTUser> usersAssigned;




        // CONSTRUCTORS
        public ProjectDetailsViewModel(Project project, List<BTUser> userList)
        {
            this.project = project;
            this.usersAssigned = userList;
        }
    }
}

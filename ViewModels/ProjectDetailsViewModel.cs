using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public Project project;
        public List<BTUser> usersAssigned;
    }
}

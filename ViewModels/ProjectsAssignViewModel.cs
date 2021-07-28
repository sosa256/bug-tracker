using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    // Copy of ProjectDetailsViewModel
    public class ProjectsAssignViewModel
    {
        public Project project;
        public List<BTUser> freeUsers;
    }
}

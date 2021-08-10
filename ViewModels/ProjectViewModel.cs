using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
    public class ProjectViewModel
    {
        // PROPERTIES
        public List<ProjectReadable> projReadableList;
        public Project currProject;
        public string errorMsg = "";




        // CONSTRUCTOR
        public ProjectViewModel(List<ProjectReadable> projectList)
        {
            this.projReadableList = projectList;
        }


        public ProjectViewModel(Project currProject, string errorMsg)
        {
            this.currProject = currProject;
            this.errorMsg = errorMsg;
        }


        public ProjectViewModel(List<ProjectReadable> projectList, string errorMsg)
        {
            this.projReadableList = projectList;
            this.errorMsg = errorMsg;
        }


        public ProjectViewModel(Project currProject)
        {
            this.currProject = currProject;
        }
    }
}

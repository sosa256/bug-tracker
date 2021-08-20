using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public enum AllBTUserRoles
    {
        Administrator = 1,
        Developer,
        Submitter,
        [Display(Name = "No Role")]
        NoRole,
        DemoAdministrator,
        DemoDeveloper,
        DemoSubmitter
    }
}

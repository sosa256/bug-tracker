using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public enum BTUserRoles
    {
        Administrator = 1,
        Developer,
        Submitter,
        [Display(Name = "Demo Administrator")]
        DemoAdministrator,
        [Display(Name = "Demo Developer")]
        DemoDeveloper,
        [Display(Name = "Demo Submitter")]
        DemoSubmitter,
        [Display(Name = "No Role")]
        NoRole
    }
}

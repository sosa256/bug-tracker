﻿using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public enum BTUserRoles
    {
        Administrator = 1,
        Developer,
        Submitter,
        [Display(Name = "No Role")]
        NoRole
    }
}

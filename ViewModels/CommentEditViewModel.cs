using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class CommentEditViewModel
    {
        public Comment currComment { get; set; }
        public int ReturnTicketId { get; set; }
    }
}

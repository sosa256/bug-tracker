using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
    public class BugTicketCreateViewModel
    {
        public Ticket newTicket { get; set; }
        public List<projOption> projOptionList { get; set; }

        public class projOption
        {
            public int Id { get; set; }
            public string Title { get; set; }
        } 
    }
}

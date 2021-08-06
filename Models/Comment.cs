using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Comment
    {
		// PROPERTIES
		[Key]
        public int Id { get; set; }
        public int Owner { get; set; }
		public int TicketId { get; set; }
		[MinLength(3)]
		[MaxLength(255)]
		public string Msg { get; set; }
		public DateTime DateCreated { get; set; }
	}
}

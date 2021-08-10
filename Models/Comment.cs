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




        // CONSTRUCTORS
        public Comment(int commentId, int userOwnerId, int ticketId, string msg, DateTime dateCreated)
        {
            this.Id = commentId;
            this.Owner = userOwnerId;
            this.TicketId = ticketId;
            this.Msg = msg;
            this.DateCreated = dateCreated;
        }

        public Comment(Comment comment)
        {
            this.Id = comment.Id;
            this.Owner = comment.Owner;
            this.TicketId = comment.TicketId;
            this.Msg = comment.Msg;
            this.DateCreated = comment.DateCreated;
        }

        public Comment()
        {
        }
    }
}

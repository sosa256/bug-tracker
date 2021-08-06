using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Readable;
using BugTracker.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class CommentController : Controller
    {
        // PROPERTIES
        private SqlConnection _db;




        // CONSTRUCTORS
        public CommentController()
        {
            _db = DbHelper.GetConnection();
        }



        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int returnticketId, IFormCollection collection)
        {
            // Generate comment Id.
            string query = "SELECT Comments.Id FROM Comments ORDER BY Comments.Id DESC;";
            // Make sure there is something in the database. Default is 0.
            int extractedMax = _db.Query<int>(query).FirstOrDefault();
            int commentId = extractedMax + 1;

            // Get The comment properties.
            int owner = 1;
            int ticketId = returnticketId;
            string msg = collection["newComment.Msg"].ToString();
            DateTime dateCreated = DateTime.Now;

            // Insert comment.
            string insertCommentQuery = "INSERT INTO [Comments] VALUES(@Id, @Owner, @TicketId, @Msg, @DateCreated );";
            var newComment = new Comment
            {
                Id = commentId,
                Owner = owner,
                TicketId = ticketId,
                Msg = msg,
                DateCreated = dateCreated
            };

            // Add Comment to database using Dapper.Contrib.Extensions.
            _db.Execute(insertCommentQuery, newComment);

            var routeValues = new { ticketId = returnticketId };
            return RedirectToAction("Details", "BugTicket", routeValues);
        }


        // Get: Comment/Delete
        [HttpGet]
        public IActionResult Delete(int commentId, int returnticketId)
        {
            // Delete Comment.
            string deleteCommentQuery = "DELETE FROM Comments WHERE Comments.Id = @CommentId ;";
            var parameter = new { CommentId = commentId }; 
            _db.Execute(deleteCommentQuery, parameter);

            var routeValues = new { ticketId = returnticketId };
            return RedirectToAction("Details", "BugTicket", routeValues);
        }


        // Get: Comment/Edit/5
        [HttpGet]
        public IActionResult Edit(int commentId, int returnticketId)
        {
            string getCommentQuery = "SELECT * FROM Comments WHERE Comments.Id = @CommentId;";
            var parameters = new { CommentId = commentId };
            Comment comment = _db.Query<Comment>(getCommentQuery, parameters).First();

            CommentEditViewModel model = new CommentEditViewModel()
            {
                currComment = comment,
                ReturnTicketId = returnticketId
            };

            return View(model);
        }


        // POST: Comment/Edit/5
        [HttpPost]
        public IActionResult Edit(int returnticketId, int commentId, IFormCollection collection)
        {
            string newMsg = collection["currComment.Msg"].ToString();

            // Update Comment.
            string updateCommentQuery = "UPDATE Comments SET Comments.Msg = @NewMsg WHERE Comments.Id = @CommentId ;";
            var parameters = new { NewMsg = newMsg, CommentId = commentId };
            _db.Execute(updateCommentQuery, parameters);


            var routeValues = new { ticketId = returnticketId };
            return RedirectToAction("Details", "BugTicket", routeValues);
        }




        public List<CommentReadable> CommentListToReadable(List<Comment> commentList)
        {
            List<CommentReadable> ret = new List<CommentReadable>();

            if (commentList.Count == 0)
            {
                // List is empty
                return ret;
            }

            foreach (Comment item in commentList)
            {
                ret.Add(CommentToReadable(item));
            }

            return ret;
        }


        public CommentReadable CommentToReadable(Comment comment)
        {
            // Get owner's full name.
            string getFullNameQuery = "SELECT CONCAT(BTUsers.FirstName, ' ',  BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Comments.Owner FROM Comments WHERE Comments.Id = @commentId );";
            var parameters = new { commentId = comment.Id };
            string ownerName = _db.Query<string>(getFullNameQuery, parameters).First(); ;

            CommentReadable ret = new CommentReadable(comment, ownerName);

            return ret;
        }
    }
}

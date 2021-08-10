﻿using BugTracker.Areas.Identity.Data;
using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Readable;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BugTracker.Controllers
{
    public class CommentController : Controller
    {
        // PROPERTIES
        private readonly SqlHelper _sqlHelper;
        private readonly UserManager<BugTrackerUser> _userManager;




        // CONSTRUCTORS
        public CommentController(UserManager<BugTrackerUser> userManager)
        {
            _sqlHelper = new SqlHelper();
            _userManager = userManager;
        }




        // ACTIONS / METHODS

        // Get: Comment/Edit/5
        [HttpGet]
        public IActionResult Edit(int commentId, int returnTicketId)
        {
            Comment comment = _sqlHelper.SelectComment(commentId);
            // Verify comment and returnTicket
            if (comment == null)
            {
                return Content("Comment doesn't exist.");
            }
            int mostCurrTicketId = _sqlHelper.SelectMostCurrentTicketInHistory(returnTicketId);

            if (mostCurrTicketId == 0)
            {
                return Content("Whoops that ticket doesn't exist!");
            }

            if (returnTicketId != mostCurrTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }
            // Comment and returnTicket must be valid here.

            CommentEditViewModel model = new CommentEditViewModel(comment, returnTicketId);

            return View(model);
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
            string ownerName = _sqlHelper.GetUserFullName(comment.Owner);

            CommentReadable ret = new CommentReadable(comment, ownerName);

            return ret;
        }


// CRUD operations for comment w/o Read (in Ticket/Details).
        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            int returnTicketId = Int32.Parse(collection["currTicketReadable.Id"]);
            // Validate returnTicketId.
            int mostCurrTicketId = _sqlHelper.SelectMostCurrentTicketInHistory(returnTicketId);

            if (mostCurrTicketId == 0)
            {
                return Content("Whoops that ticket doesn't exist!");
            }

            if (returnTicketId != mostCurrTicketId)
            {
                // TODO: Error pages.
                return Content("Whoops that ticket is outdated and cannot be viewed! Please use history to view old tickets.");
            }
            // returnTicket must be valid here.

            // Extract the form input.
            string msg = collection["newComment.Msg"].ToString();
            int owner = _sqlHelper.SelectUserFromStringId(_userManager.GetUserId(User)).Id;
            
            // Validate the msg.
            bool msgIsOutOfRange = msg.Length < 3 || msg.Length > 255;
            if (msgIsOutOfRange)
            {
                var redirectRouteValues = new
                {
                    erroneousMsg = msg,
                    ticketId = returnTicketId
                };
                return RedirectToAction("DetailsError", "Ticket", redirectRouteValues);
            }

            // Msg must be valid here.

            // Generate comment Id.
            int commentId = _sqlHelper.GenerateCommentId();

            // Add Comment to database.
            Comment newComment = new Comment(commentId, owner, returnTicketId, msg, DateTime.Now);
            _sqlHelper.InsertComment(newComment);

            var routeValues = new { ticketId = returnTicketId };
            return RedirectToAction("Details", "Ticket", routeValues);
        }

        // POST: Comment/Edit/5
        [HttpPost]
        public IActionResult Edit(int returnTicketId, int commentId, IFormCollection collection)
        {
            string newMsg = collection["currComment.Msg"].ToString();

            // Validate the msg.
            bool newMsgIsOutOfRange = newMsg.Length < 3 || newMsg.Length > 255;
            if (newMsgIsOutOfRange)
            {
                // Length is out of range.
                // Set up the model.
                Comment currComment = _sqlHelper.SelectComment(commentId);
                CommentEditViewModel model = new CommentEditViewModel(currComment, returnTicketId);
                model.ErrorExists = true;
                model.currComment.Msg = newMsg;

                return View("Edit", model);
            }

            // Msg must be valid here.

            // Update Comment.
            _sqlHelper.UpdateCommentMsg(commentId, newMsg);

            var routeValues = new { ticketId = returnTicketId };
            return RedirectToAction("Details", "Ticket", routeValues);
        }


        // POST: Comment/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(IFormCollection collection)
        {
            // Extract form data.
            int commentId = Int32.Parse(collection["item.Id"]);
            int returnTicketId = Int32.Parse(collection["currTicketReadable.Id"]);

            // Delete Comment.
            _sqlHelper.DeleteComment(commentId);

            var routeValues = new { ticketId = returnTicketId };
            return RedirectToAction("Details", "Ticket", routeValues);
        }
    }
}

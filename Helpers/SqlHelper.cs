using BugTracker.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using static BugTracker.ViewModels.TicketCreateViewModel;

namespace BugTracker.Helpers
{
    public class SqlHelper
    {
        // PROPERTIES
        private readonly SqlConnection _db;

        // QUERIES
        //
        // SELECT *
        // FROM BTUsers;
        private readonly string selectAllUsersQuery = "SELECT * FROM BTUsers;";
        // 
        // UPDATE BTUsers
        //     SET BTUsers.Role = @RoleId
        //     WHERE BTUsers.Id = @UserId;
        private readonly string updateUserRoleQuery = "UPDATE BTUsers SET BTUsers.Role = @RoleId WHERE BTUsers.Id = @UserId;";
        // 
        // DELETE FROM AspNetUserRoles
        // WHERE UserId = @UserId;
        private readonly string makeUserHaveNoRolesQuery = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId;";
        //
        // SELECT *
        // FROM Projects;
        private readonly string selectAllProjectsQuery = "SELECT * FROM Projects;";
        // 
        // SELECT Projects.Id
        // FROM Projects
        // ORDER BY Projects.Id DESC;
        private readonly string getMaxProjectIdQuery = "SELECT Projects.Id FROM Projects ORDER BY Projects.Id DESC;";
        // 
        // INSERT INTO Projects
        // VALUES( @TicketId, @Title, @OwnerId );
        // 
        private readonly string insertProjectQuery = "INSERT INTO Projects VALUES( @Id, @Title, @Owner );";
        // 
        // SELECT *
        // FROM BTUsers
        // WHERE BTUsers.stringId = @UserStringId;
        private readonly string selectUserFromStringIdQuery = "SELECT * FROM BTUsers WHERE BTUsers.stringId = @UserStringId;";
        // 
        // DELETE
        // FROM Projects
        // WHERE Projects.Id = @ProjectId ;
        private readonly string deleteProjectQuery = "DELETE FROM Projects WHERE Projects.Id = @ProjectId ;";
        // 
        // UPDATE Projects
        //     SET Title = @NewTitle
        //     WHERE Projects.Id = @ProjectId;
        private readonly string updateProjectTitleQuery = "UPDATE Projects SET Title = @NewTitle WHERE Projects.Id = @ProjectId;";
        // 
        // SELECT Tickets.Id
        // FROM Tickets
        // WHERE Tickets.HistoryId IN (
        //     SELECT Tickets.HistoryId
        //     FROM Tickets
        //     WHERE Tickets.Id = @TicketId
        // )
        // ORDER BY Tickets.Id DESC;
        // 
        private readonly string selectMostCurrentTicketOfItsHistoryQuery = "SELECT Tickets.Id FROM Tickets WHERE Tickets.HistoryId IN ( SELECT Tickets.HistoryId FROM Tickets WHERE Tickets.Id = @TicketId) ORDER BY Tickets.Id DESC;";
        // 
        // SELECT *
        // FROM Projects
        // WHERE Projects.Id = @ProjectId;
        private readonly string selectProjectQuery = "SELECT * FROM Projects WHERE Projects.Id = @ProjectId;";
        // 
        // SELECT *
        // FROM BTUsers
        // WHERE BTUsers.Id IN(	
        //     SELECT Assignments.UserAssigned
        //     FROM Assignments
        //     WHERE Assignments.ProjectId = @ProjectId
        // );
        private readonly string selectUsersInProjectQuery = "SELECT * FROM BTUsers WHERE BTUsers.Id IN(	SELECT Assignments.UserAssigned	FROM Assignments WHERE Assignments.ProjectId = @ProjectId);";
        //
        // SELECT *
        // FROM BTUsers
        // WHERE BTUsers.Id NOT IN (
        //     SELECT Assignments.UserAssigned
        //     FROM Assignments
        // );
        private readonly string selectUnassignedUsersQuery = "SELECT * FROM BTUsers WHERE BTUsers.Id NOT IN(SELECT Assignments.UserAssigned FROM Assignments);";
        // 
        // INSERT INTO Assignments
        // VALUES(@UserId, @ProjectId);
        private readonly string assignUserToProjectQuery = "INSERT INTO Assignments VALUES(@UserId, @ProjectId);";
        // 
        // DELETE FROM Assignments
        // WHERE Assignments.UserAssigned = @UserId
        //   AND Assignments.ProjectId = @ProjectId;
        private readonly string deleteUserProjectAssignmentQuery = "DELETE FROM Assignments WHERE Assignments.UserAssigned = @UserId AND Assignments.ProjectId = @ProjectId;";
        // 
        // SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName
        // FROM BTUsers
        // WHERE BTUsers.Id IN (
        //     SELECT Projects.Owner
        //     FROM Projects
        //     WHERE Projects.Id = @ProjectId
        // );
        private readonly string getProjectOwnerNameQuery = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Projects.Owner FROM Projects WHERE Projects.Id = @ProjectId );";
        // 
        // SELECT Tickets.HistoryId
        // FROM Tickets
        // ORDER BY Tickets.HistoryId DESC;
        private readonly string selectMaxHistoryIdQuery = "SELECT Tickets.HistoryId FROM Tickets ORDER BY Tickets.HistoryId DESC;";
        // 
        // SELECT Tickets.Id
        // FROM Tickets
        // ORDER BY Tickets.Id DESC;
        private readonly string selectMaxTicketIdQuery = "SELECT Tickets.Id FROM Tickets ORDER BY Tickets.Id DESC;";
        // 
        // INSERT INTO Tickets
        // VALUES(
        //     @ProjectParent,
        //     @Id,
        //     @HistoryId,
        //     @IsCurr,
        //     @Title,
        //     @Severity,
        //     @Status,
        //     @UnwantedBehavior,
        //     @RepeatableSteps,
        //     @OpenedBy,
        //     @DateCreated
        // );
        private readonly string insertTicketQuery = "INSERT INTO Tickets VALUES( @ProjectParent,  @Id,  @HistoryId, @IsCurr, @Title, @Severity, @Status, @UnwantedBehavior, @RepeatableSteps, @OpenedBy, @DateCreated );";
        // 
        // SELECT *
        // FROM Tickets
        // WHERE Tickets.IsCurr = 1;
        private readonly string selectAllCurrentTicketsQuery = "SELECT * FROM Tickets WHERE Tickets.IsCurr = 1;";
        // 
        // UPDATE Tickets
        //     SET Tickets.IsCurr = 0
        //     WHERE Tickets.Id = @TicketId;
        private readonly string makeTicketNotCurrentQuery = "UPDATE Tickets SET Tickets.IsCurr = 0 WHERE Tickets.Id = @TicketId;";
        // 
        // SELECT *
        // FROM Tickets
        // WHERE Tickets.Id = @TicketId;
        private readonly string selectTicketQuery = "SELECT * FROM Tickets WHERE Tickets.Id = @TicketId;";
        // 
        // DELETE FROM Tickets
        //     WHERE Tickets.Id = @TicketId ;
        private readonly string deleteTicketQuery = "DELETE FROM Tickets WHERE Tickets.Id = @TicketId ;";
        // 
        // SELECT *
        // FROM Tickets
        // WHERE Tickets.IsCurr = 1
        //   AND Tickets.ProjectParent = @ProjectId
        private readonly string selectAllCurrentTicketsInProjectQuery = "SELECT * FROM Tickets WHERE Tickets.IsCurr = 1 AND Tickets.ProjectParent = @ProjectId";
        // 
        // SELECT Projects.Id, Projects.Title
        // FROM Projects;
        private readonly string selectProjectIdAndTitleQuery = "SELECT Projects.Id, Projects.Title FROM Projects;";
        /* 
         * SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName
         * FROM BTUsers
         * WHERE BTUsers.Id IN
         * (
	     *     SELECT Tickets.OpenedBy
	     *     FROM Tickets
	     *     WHERE Tickets.Id = @TicketId
         * );
        */
        private readonly string selectTicketOpenedByNameQuery = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( SELECT Tickets.OpenedBy FROM Tickets WHERE Tickets.Id = @TicketId );";
        /* SELECT * 
         * FROM Projects 
         * WHERE Projects.Id IN 
         * (
	     *    SELECT Tickets.ProjectParent
	     *    FROM Tickets
	     *    WHERE Tickets.Id = @TicketId
         *  );
         */
        private readonly string selectProjectFromTicketIdQuery = "SELECT * FROM Projects WHERE Projects.Id IN ( SELECT Tickets.ProjectParent FROM Tickets WHERE Tickets.Id = @TicketId );";
        // 
        // SELECT *
        // FROM Comments
        // WHERE Comments.TicketId = @TicketId;
        private readonly string selectCommentsFromTicketQuery = "SELECT * FROM Comments WHERE Comments.TicketId = @TicketId;";
        // 
        // UPDATE TICKETS
        //     SET Tickets.Status = @TicketStatus
        //     WHERE Tickets.Id = @TicketId;
        private readonly string updateTicketStatusQuery = "UPDATE TICKETS SET Tickets.Status = @TicketStatus WHERE Tickets.Id = @TicketId;";
        //
        // SELECT *
        // FROM Tickets
        // WHERE Tickets.HistoryId = @HistoryId
        // ORDER BY Tickets.Id DESC;
        private readonly string selectTicketHistoryQuery = "SELECT * FROM Tickets WHERE Tickets.HistoryId = @HistoryId ORDER BY Tickets.Id DESC;";
        // 
        // SELECT Tickets.Id
        // FROM Tickets
        // WHERE Tickets.HistoryId = @HistoryId
        // ORDER BY Tickets.Id DESC;
        private readonly string selectCurrTicketIdInHistoryQuery = "SELECT Tickets.Id FROM Tickets WHERE Tickets.HistoryId = @HistoryId ORDER BY Tickets.Id DESC;";
        // 
        // SELECT Projects.Id
        // FROM Projects;
        private readonly string selectAllProjectIdsQuery = "SELECT Projects.Id FROM Projects;";
        // 
        // SELECT Tickets.HistoryId
        // FROM Tickets;
        private readonly string selectAllHistoryIdsQuery = "SELECT Tickets.HistoryId FROM Tickets;";
        // 
        // SELECT *
        // FROM ClosedTickets;
        private readonly string selectAllClosedTicketsQuery = "SELECT * FROM ClosedTickets;";
        //
        // SELECT *
        // FROM ClosedTickets
        // WHERE ClosedTickets.ProjectParent = @ProjectId
        //   AND ClosedTickets.TicketClosed = @TicketId ;
        private readonly string selectClosedTicketQuery = "SELECT * FROM ClosedTickets WHERE ClosedTickets.ProjectParent = @ProjectId AND ClosedTickets.TicketClosed = @TicketId ;";
        //
        // DELETE FROM ClosedTickets
        //     WHERE ClosedTickets.ProjectParent = @ProjectId
        //       AND ClosedTickets.TicketClosed = @TicketId ;
        private readonly string deleteClosedTicketQuery = "DELETE FROM ClosedTickets WHERE ClosedTickets.ProjectParent = @ProjectId AND ClosedTickets.TicketClosed = @TicketId ;";
        //
        // SELECT CONCAT(BTUsers.FirstName, ' ',  BTUsers.LastName) AS FullName
        // FROM BTUsers WHERE BTUsers.Id IN
        // ( 	
        //     SELECT ClosedTickets.UserWhoClosed 	
        //     FROM ClosedTickets 	
        //     WHERE ClosedTickets.ProjectParent = @ProjectId
        //       AND ClosedTickets.TicketClosed = @TicketId
        // );
        private readonly string getUserWhoClosedNameQuery = "SELECT CONCAT(BTUsers.FirstName, ' ',  BTUsers.LastName) AS FullName FROM BTUsers WHERE BTUsers.Id IN ( 	SELECT ClosedTickets.UserWhoClosed 	FROM ClosedTickets 	WHERE ClosedTickets.ProjectParent = @ProjectId AND ClosedTickets.TicketClosed = @TicketId );";
        //
        // SELECT Comments.Id
        // FROM Comments
        // ORDER BY Comments.Id DESC;
        private readonly string selectMaxCommentIdQuery = "SELECT Comments.Id FROM Comments ORDER BY Comments.Id DESC;";
        //
        // INSERT INTO Comments
        //     VALUES( @Id, @Owner, @TicketId, @Msg, @DateCreated );
        private readonly string insertCommentQuery = "INSERT INTO Comments VALUES( @Id, @Owner, @TicketId, @Msg, @DateCreated );";
        //
        // SELECT *
        // FROM Comments
        // WHERE Comments.Id = @CommentId;
        private readonly string selectCommentQuery = "SELECT * FROM Comments WHERE Comments.Id = @CommentId;";
        //
        // SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName
        // FROM BTUsers
        // WHERE  BTUsers.Id = @UserId;
        private readonly string getUserFullNameQuery = "SELECT CONCAT(BTUsers.FirstName,  ' ' + BTUsers.LastName) AS FullName FROM BTUsers WHERE  BTUsers.Id = @UserId;";
        //
        // UPDATE Comments
        //     SET Comments.Msg = @NewMsg
        //     WHERE Comments.Id = @CommentId ;
        private readonly string updateCommentMsgQuery = "UPDATE Comments SET Comments.Msg = @NewMsg WHERE Comments.Id = @CommentId ;";
        //
        // DELETE FROM Comments
        //   WHERE Comments.Id = @CommentId ;
        private readonly string deleteCommentQuery = "DELETE FROM Comments WHERE Comments.Id = @CommentId ;";



        // CONSTRUCTOR
        public SqlHelper()
        {
            _db = DbHelper.GetConnection();
        }




        // METHODS
        public List<BTUser> SelectAllUsers()
        {
            return _db.Query<BTUser>(selectAllUsersQuery).ToList();
        }


        public void AssignUserRole(int currUserId, int newRoleId)
        {
            var parameters = new 
            { 
                RoleId = newRoleId, 
                UserId = currUserId 
            };

            _db.Execute(updateUserRoleQuery, parameters);
        }


        public void MakeUserHaveNoRoles(string currUserIdentityId)
        {
            _db.Execute(makeUserHaveNoRolesQuery, new { UserId = currUserIdentityId });
        }


        public List<Project> SelectAllProjects()
        {
            return _db.Query<Project>(selectAllProjectsQuery).ToList();
        }


        public int GenerateProjectId()
        {
            int extractedMax = _db.Query<int>(getMaxProjectIdQuery).FirstOrDefault();
            int projectId = extractedMax + 1;

            return projectId;
        }


        public void InsertProject(Project projectToInsert)
        {
            _db.Execute(insertProjectQuery, projectToInsert);
        }


        public BTUser SelectUserFromStringId(string userStringId)
        {
            return _db.Query<BTUser>(selectUserFromStringIdQuery, new { UserStringId = userStringId }).First();
        }


        public void DeleteProject(int projectId)
        {
            _db.Execute(deleteProjectQuery, new { ProjectId = projectId });
        }


        public void UpdateProjectTitle(int projectId, string newTitle)
        {
            var parameters = new
            {
                ProjectId = projectId,
                NewTitle = newTitle
            };

            _db.Execute(updateProjectTitleQuery, parameters);
        }


        public int SelectMostCurrentTicketInHistory(int ticketId)
        {
            var parameters = new
            {
                TicketId = ticketId
            };

            int ret = _db.Query<int>(selectMostCurrentTicketOfItsHistoryQuery, parameters).FirstOrDefault();

            return ret;
        }


        public Project SelectProject(int projectId)
        {
            return _db.Query<Project>(selectProjectQuery, new { ProjectId = projectId }).FirstOrDefault();
        }


        public List<BTUser> SelectUsersInProject(int projectId)
        {
            return _db.Query<BTUser>(selectUsersInProjectQuery, new { ProjectId = projectId } ).ToList();
        }


        public List<BTUser> SelectUnassignedUsers()
        {
            return _db.Query<BTUser>(selectUnassignedUsersQuery).ToList();
        }

        public void AssignUserToProject(int userId, int projectId)
        {
            var parameters = new
            {
                UserId = userId,
                ProjectId = projectId
            };

            _db.Execute(assignUserToProjectQuery, parameters);
        }


        public void DeleteUserProjectAssignment(int userId, int projectId)
        {
            var parameters = new
            {
                UserId = userId,
                ProjectId = projectId
            };

            _db.Execute(deleteUserProjectAssignmentQuery, parameters);
        }


        public string GetProjectOwnerName(int projectId)
        {
            return _db.Query<string>(getProjectOwnerNameQuery, new { ProjectId = projectId }).First();
        }


        public int GenerateHistoryId()
        {
            int extractedMax = _db.Query<int>(selectMaxHistoryIdQuery).FirstOrDefault();
            int ticketHistoryId = extractedMax + 1;

            return ticketHistoryId;
        }


        public int GenerateTicketId()
        {
            int extractedMax = _db.Query<int>(selectMaxTicketIdQuery).FirstOrDefault();
            int ticketId = extractedMax + 1;

            return ticketId;
        }


        public void InsertTicket(Ticket ticketToInsert)
        {
            _db.Execute(insertTicketQuery, ticketToInsert);
        }


        public List<Ticket> SelectAllCurrentTickets()
        {
            return _db.Query<Ticket>(selectAllCurrentTicketsQuery).ToList();
        }


        public void MakeTicketNotCurrent(int ticketId)
        {
            _db.Execute(makeTicketNotCurrentQuery, new { TicketId = ticketId });
        }


        public void UpdateTicket(Ticket updatedTicket, int outdatedTicketId)
        {
            // Make the predecesor not current.
            // This way the old ticket doesn't appear in Project/Manage view.
            MakeTicketNotCurrent(outdatedTicketId);

            // Insert new updated Ticket to myDB.
            InsertTicket(updatedTicket);
        }


        public Ticket SelectTicket(int ticketId)
        {
            return _db.Query<Ticket>(selectTicketQuery, new { TicketId = ticketId }).FirstOrDefault();
        }


        public void DeleteTicket(int ticketId)
        {
            _db.Execute(deleteTicketQuery, new { TicketId = ticketId });
        }


        public List<Ticket> SelectAllCurrentTickets(int projectId)
        {
            var parameter = new { ProjectId = projectId };
            return _db.Query<Ticket>(selectAllCurrentTicketsInProjectQuery, parameter).ToList();
        }

        public List<ProjectOption> SelectProjectIdAndTitle()
        {
            return _db.Query<ProjectOption>(selectProjectIdAndTitleQuery).ToList();
        }


        public string GetTicketOpenedByName(int ticketId)
        {
            return _db.Query<string>(selectTicketOpenedByNameQuery, new { TicketId = ticketId }).First();
        }

        public Project SelectProjectFromTicket(int ticketId)
        {
            return _db.Query<Project>(selectProjectFromTicketIdQuery, new { TicketId = ticketId }).First();
        }

        public List<Comment> SelectCommentsFromTicket(int ticketId)
        {
            var parameter = new { TicketId = ticketId };
            return _db.Query<Comment>(selectCommentsFromTicketQuery, parameter).ToList();
        }


        public void UpdateTicketStatus(int ticketId, TicketStatus ticketStatus)
        {
            var parameters = new
            {
                TicketId = ticketId,
                TicketStatus = (int) ticketStatus
            };

            _db.Execute(updateTicketStatusQuery, parameters);
        }


        public List<Ticket> SelectTicketHistory(int historyId)
        {
            // Get a ticket's history from oldest to newest.
            return _db.Query<Ticket>(selectTicketHistoryQuery, new { HistoryId = historyId }).ToList();
        }


        public int SelectCurrentTicketIdInHistory(int historyId)
        {
            return _db.Query<int>(selectCurrTicketIdInHistoryQuery, new { HistoryId = historyId }).First();
        }


        public List<int> SelectAllProjectIds()
        {
            return _db.Query<int>(selectAllProjectIdsQuery).ToList();
        }


        public List<int> SelectAllHistoryIds()
        {
            return _db.Query<int>(selectAllHistoryIdsQuery).ToList();
        }

        public List<ClosedTicket> SelectAllClosedTickets()
        {
            return _db.Query<ClosedTicket>(selectAllClosedTicketsQuery).ToList();
        }

        public ClosedTicket SelectClosedTicket(int projectId, int ticketId)
        {
            var parameters = new
            {
                ProjectId = projectId,
                TicketId = ticketId
            };
            return _db.Query<ClosedTicket>(selectClosedTicketQuery, parameters).First();
        }

        public void DeleteClosedTicket(int projectId, int ticketId)
        {
            var parameters = new
            {
                ProjectId = projectId,
                TicketId = ticketId
            };
            _db.Execute(deleteClosedTicketQuery, parameters);
        }


        public string GetUserWhoClosedName(int projectId, int ticketId)
        {
            var parameters = new
            {
                ProjectId = projectId,
                TicketId = ticketId
            };
            return _db.Query<string>(getUserWhoClosedNameQuery, parameters).First();
        }

        public int GenerateCommentId()
        {
            int extractedMax = _db.Query<int>(selectMaxCommentIdQuery).FirstOrDefault();
            int commentId = extractedMax + 1;

            return commentId;
        }


        public void InsertComment(Comment commentToInsert)
        {
            _db.Execute(insertCommentQuery, commentToInsert);
        }


        public Comment SelectComment(int commentId)
        {
            return _db.Query<Comment>(selectCommentQuery, new { CommentId  = commentId }).FirstOrDefault();
        }

        public string GetUserFullName(int userId)
        {
            return _db.Query<string>(getUserFullNameQuery, new { UserId = userId }).First();
        }


        public void UpdateCommentMsg(int commentId, string newMsg)
        {
            var parameters = new
            {
                CommentId = commentId,
                NewMsg = newMsg
            };

            _db.Execute(updateCommentMsgQuery, parameters);
        }

        public void DeleteComment(int commentId)
        {
            _db.Execute(deleteCommentQuery, new { CommentId = commentId });
        }
    }
}

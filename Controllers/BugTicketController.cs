using BugTracker.Helpers;
using BugTracker.Models;
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
    public class BugTicketController : Controller
    {
        // PROPERTIES
        private SqlConnection _db;




        // CONSTRUCTORS
        public BugTicketController()
        {
            _db = DbHelper.GetConnection();
        }




        // ACTIONS / METHODS

        // GET: BugTicketController
        public ActionResult Manage()
        {
            BugTicketViewModel model = new BugTicketViewModel()
            {
                bugTicketList = _db.Query<Ticket>("SELECT * FROM Tickets").ToList()
            };
            return View(model);
        }










        // GET: BugTicketController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BugTicketController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BugTicketController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BugTicketController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BugTicketController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BugTicketController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BugTicketController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

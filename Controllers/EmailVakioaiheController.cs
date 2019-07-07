using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Demo.Controllers
{
    public class EmailVakioaiheController : Controller
    {
        // GET: EmailVakioaihe
        public ActionResult Index()
        {
            return View();
        }

        // GET: EmailVakioaihe/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmailVakioaihe/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailVakioaihe/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EmailVakioaihe/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmailVakioaihe/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EmailVakioaihe/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmailVakioaihe/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CRM_Demo.Models;

namespace CRM_Demo.Controllers
{
    public class LoginController : Controller
    {


        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Autherize(Login LoginModel)
        {
            using (ProjektitDBCareEntities db = new ProjektitDBCareEntities())
            {

                Login LoginDetails = db.Login.Where
               (x => x.Käyttäjätunnus == LoginModel.Käyttäjätunnus && x.Salasana == LoginModel.Salasana)
               .FirstOrDefault();

                if (LoginDetails == null)
                {
                    LoginModel.LoginErrorMessage = "Väärä käyttäjätunnus tai salasana.";

                    return View("Index", LoginModel);
                }
                else
                {
                    Session["LoginId"] = LoginDetails.LoginId;
                    return RedirectToAction("Index", "Home");
                }   //Edeltävällä rivillä määritellään minne kirjautuminen johtaa
                    
            }


        }
        public ActionResult LogOut()
        {
            Session.Abandon();


            return RedirectToAction("LoggedOut", "login");

        }

        public ActionResult LoggedOut()
        {
            ViewBag.LoggedOut = "Olet kirjautunut ulos järjestelmästä.";
            return View(); 
        }



    }
}

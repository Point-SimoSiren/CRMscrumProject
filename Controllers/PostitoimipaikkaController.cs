using CRM_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Demo.Controllers
{
    public class PostitoimipaikkaController : Controller
    {
        // GET: Postitoimipaikka
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetList()
        {
            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Haetaan Postinumerot -taulusta kaikki data
            var posnro = (from pos in entities.Postitoimipaikat
                             select new
                             {
                                 Postinumero = pos.Postinumero,
                                 Postitoimipaikka = pos.Postitoimipaikka
                             }).ToList();

            //Muutetaan data json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling =
                PreserveReferencesHandling.Objects };

            string json = JsonConvert.SerializeObject(posnro, serializerSettings);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        // GET: Postitoimipaikka/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
                
    }

}
    


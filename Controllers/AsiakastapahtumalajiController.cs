using CRM_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Demo.Controllers
{
    public class AsiakastapahtumalajiController : Controller
    {
        // GET: Tapahtumalaji
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Haetaan -taulusta kaikki data
            var tapahtumalajit = (from ak in entities.Tapahtumalajit
                                  select new {
                                      ak.TapahtumalajiId,
                                      ak.TapahtumalajiNimi,
                                      ak.TapahtumalajiKuvaus
                                  }).ToList();

            //Muutetaan data json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(tapahtumalajit, serializerSettings);
            entities.Dispose();


            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        // POST: Tapahtumalaji/Create
        [HttpPost]
        public ActionResult Create(int TapahtumalajiId, FormCollection collection)
        {
            try
            {

                if (collection["Tapahtumalajimuutostyyppi"].ToString() == "Uusi Tapahtumalaji")
                {

                    ProjektitDBCareEntities entities = new ProjektitDBCareEntities();
                    CRM_Demo.Models.Tapahtumalajit dbItem = new Models.Tapahtumalajit();

                    dbItem.TapahtumalajiNimi = collection["TapahtumalajiNimi"].ToString();
                    dbItem.TapahtumalajiKuvaus = collection["TapahtumalajiKuvaus"].ToString();

                    // tallennus tietokantaan
                    entities.Tapahtumalajit.Add(dbItem);
                    entities.SaveChanges();
                    entities.Dispose();
                    return Content("ok");
                }

                else
                {

                    ProjektitDBCareEntities entities = new ProjektitDBCareEntities();
                    CRM_Demo.Models.Tapahtumalajit dbItem = (from c in entities.Tapahtumalajit
                                                             where c.TapahtumalajiId == TapahtumalajiId
                                                             select c).FirstOrDefault();
                    if (dbItem != null)
                    {
                        // tietokannan muokkaus
                        dbItem.TapahtumalajiNimi = collection["TapahtumalajiNimi"].ToString();
                        dbItem.TapahtumalajiKuvaus = collection["TapahtumalajiKuvaus"].ToString();
                        entities.SaveChanges();
                    }
                    entities.Dispose();
                    return Content("ok");

                }//end else

            }
            catch (Exception e)
            {
                return Content("e:" + e);//palauta error teksti

            }
        }

        // GET: Tapahtumalaji/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }
        
        // POST: Tapahtumalaji/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {

            try
            {
                ProjektitDBCareEntities entities = new ProjektitDBCareEntities();
                CRM_Demo.Models.Tapahtumalajit dbItem = (from c in entities.Tapahtumalajit
                                                         where c.TapahtumalajiId == id
                                                         select c).FirstOrDefault();
                if (dbItem != null)
                {
                    // tietokannasta poisto
                    entities.Tapahtumalajit.Remove(dbItem);
                    entities.SaveChanges();

                }
                entities.Dispose();

                return Content("ok");

            }

            catch
            {
                return View();
            }
        }
    }
}

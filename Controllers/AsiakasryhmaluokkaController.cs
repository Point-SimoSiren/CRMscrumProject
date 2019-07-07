using CRM_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Demo.Controllers
{
    public class AsiakasryhmaluokkaController : Controller
    {
        // GET: Asiakasryhmaluokka
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Haetaan Asiakasryhmäluokat -taulusta kaikki data
            var asiakasryhmat = (from ar in entities.Asiakasryhmäluokat
                                 select new {
                                 ar.RyhmäId,
                                 ar.RyhmäNimi,
                                 ar.RyhmäKuvaus
                                 }).ToList();

            //Muutetaan data json -muotoon toimitettavaksi selaimelle. 
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(asiakasryhmat, serializerSettings);

            //Suljetaan tietokantayhteys
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }
       
        
        public JsonResult GetSingleGroup(string id)
        {
            //Haetaan tietokannasta "klikatun" ryhmän tiedot

            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Muutetaan modaali-ikkunasta tullut string-tyyppinen ryhmäId int-tyyppiseksi
            int ID = int.Parse(id);

            //Haetaan Asiakasryhmäluokat -taulusta kaikki data
            var asiakasryhma = (from ar in entities.Asiakasryhmäluokat
                                where ar.RyhmäId == ID
                                select ar).FirstOrDefault();

            //Muutetaan olio json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(asiakasryhma, serializerSettings);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Asiakasryhmäluokat asiakasryhmäluokka)
        {
            // TIETOJEN PÄIVITYS JA UUDEN ASIAKASRYHMÄN LISÄYS

            bool OK = false;    //tallennuksen onnistuminen

            //tietokantaan tallennetaan uusia tietoja vain, mikäli ryhmän  nimi
            //ja ryhmän kuvaus -kentät eivät ole tyhjiä
            if (!string.IsNullOrWhiteSpace(asiakasryhmäluokka.RyhmäNimi) &&
                !string.IsNullOrWhiteSpace(asiakasryhmäluokka.RyhmäKuvaus))
            {
                //luodaan uusi entiteettiolio
                ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

                int ryhmäid = asiakasryhmäluokka.RyhmäId;

                if (ryhmäid == 0)
                {
                    //Uuden ryhmän lisääminen tietokantaan dbItem-nimisen olion avulla
                    Asiakasryhmäluokat dbItem = new Asiakasryhmäluokat()
                    {
                        //dbItemin arvot/tiedot
                        RyhmäNimi = asiakasryhmäluokka.RyhmäNimi,
                        RyhmäKuvaus = asiakasryhmäluokka.RyhmäKuvaus
                    };

                    //lisätään tietokantaan dbItemin tiedot ja tallennetaan muutokset
                    entities.Asiakasryhmäluokat.Add(dbItem);
                    entities.SaveChanges();
                    OK = true;
                }
                else
                {
                    //muokataan olemassa olevia tietoja
                    //haetaan tiedot tietokannasta

                    Asiakasryhmäluokat dbItem = (from ar in entities.Asiakasryhmäluokat
                                                 where ar.RyhmäId == ryhmäid
                                                 select ar).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.RyhmäNimi = asiakasryhmäluokka.RyhmäNimi;
                        dbItem.RyhmäKuvaus = asiakasryhmäluokka.RyhmäKuvaus;

                        // tallennetaan uudet tiedot tietokantaan
                        entities.SaveChanges();
                        OK = true;
                    }
                }

                //suljetaan tietokantayhteys
                entities.Dispose();
            }

            //palautetaan tallennuskuittaus selaimelle (muuttuja OK)
            return Json(OK, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete (string id)
        {
            //luodaan uusi entiteettiolio
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //tallennuksen onnistuminen
            bool OK = false;

            //muutetaan selaimelta tullut string-tyyppinen ryhmäId (muuttuja: id) int-tyyppiseksi
            int intID = int.Parse(id);

            //haetaan poistettavan ryhmän tiedot kannasta dbItem-olioon id:n perusteella
            Asiakasryhmäluokat dbItem = (from ar in entities.Asiakasryhmäluokat
                                         where ar.RyhmäId == intID
                                         select ar).FirstOrDefault();

            //jos tiedot löytyy
            if (dbItem != null)
            {
                //poistetaan tiedot 
                entities.Asiakasryhmäluokat.Remove(dbItem);

                //tallennetaan muutokset tietokantaan
                entities.SaveChanges();
                OK = true;
            }

            //suljetaan tietokantayhteys
            entities.Dispose();

            //palautetaan tallennuskuittaus selaimelle
            return Json(OK, JsonRequestBehavior.AllowGet);
        }
    }
}



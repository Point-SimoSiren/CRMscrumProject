using CRM_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Demo.Controllers
{
    public class AsiakasryhmaController : Controller
    {
        // GET: Asiakasryhma
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //avataan tietokantayhteys
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //haetaan kaikki data Asiakasryhmät-taulusta
            var asiakasryhmät = (from asir in entities.Asiakasryhmät
                                 join ar in entities.Asiakasryhmäluokat
                                 on asir.RyhmäId equals ar.RyhmäId
                                 join asi in entities.Asiakkaat
                                 on asir.AsiakasId equals asi.AsiakasId
                                 orderby ar.RyhmäNimi
                                 select new
                                 {
                                     asir.AsiakasryhmäId,
                                     ar.RyhmäNimi,
                                     asi.Etunimi,
                                     asi.Sukunimi
                                 }).ToList();

            //muutetaan data json muotoon toimitettavaksi selaimelle
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(asiakasryhmät, serializerSettings);

            //suljetaan tietokantayhteys
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetSingleGroup(string id)  //tietojen haku kannasta klikatun rivin id:n perusteella
        {
            //Avataan tietokanta yhteys, luodaan entiteettiolio
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //muutetaan modaali-ikkunasta tullut string-tyyppinen asiakasryhmäId int-tyyppiseksi
            int ID = int.Parse(id);

            //haetaan Asiakasryhmät taulusta kaikki data
            var asiakasryhmä = (from ar in entities.Asiakasryhmät
                             where ar.AsiakasryhmäId == ID
                             select ar).FirstOrDefault();

            //Muutetaan olio json-muotoon toimitettavaksi selaimelle.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(asiakasryhmä, serializerSettings);

            //Suljetaan tietokantayhteys
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Palautetaan data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Update(Asiakasryhmät lisääminen)
        {
            //TIETOJEN LISÄYS JA PÄIVITYS

            bool OK = false;   //tallennuksen onnistuminen

            int asiakasId = lisääminen.AsiakasId;
            int ryhmäId = lisääminen.RyhmäId;
                        
            //UUSIEN TIETOJEN LISÄYS
            //Uusia tietoja lisätään vain mikäli AsiakasId ja RyhmäId eivät ole tyhjiä
            if ((asiakasId != 0) &&
                (ryhmäId != 0))
            {
                //avataan tietokantayhteys = uusi entiteettiolio
                ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

                //luodaan uusi muuttuja johon asetetaan selaimesta tullut tieto AsiakasryhmäId:stä
                int asiakasryhmäId = lisääminen.AsiakasryhmäId;

                if (asiakasryhmäId == 0)
                {
                    //tallennetaan uuden ryhmäjäsenyyden tiedot

                    //luodaan uusi olio dbItem, jonka avulla tiedot tallennetaan kantaan
                    Asiakasryhmät dbItem = new Asiakasryhmät()
                    {
                        //dbItemin arvot/tiedot, ei AsiakasryhmäId:tä
                        AsiakasId = lisääminen.AsiakasId,
                        RyhmäId = lisääminen.RyhmäId
                    };

                    //Lisätään dbItem kantaan ja tallennetaan muutokset
                    entities.Asiakasryhmät.Add(dbItem);
                    entities.SaveChanges();

                    //tallennus on onnistunut
                    OK = true;
                }
                else
                {
                    //päivitetään valitun tapahtuman tietoja
                    //haetaan tiedot tietokannasta

                    Asiakasryhmät dbItem = (from ar in entities.Asiakasryhmät
                                         where ar.AsiakasryhmäId == asiakasryhmäId
                                         select ar).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.AsiakasId = lisääminen.AsiakasId;
                        dbItem.RyhmäId = lisääminen.RyhmäId;
                    }

                    //tallennetaan uudet tiedot tietokantaan
                    entities.SaveChanges();

                    //tallennus ok
                    OK = true;
                }

                //suljetaan tietokantayhteys
                entities.Dispose();
            }

            //palautetaan tulostumisen onnistuminen selaimelle
            return Json(OK, JsonRequestBehavior.AllowGet);
        }  //tapahtuman tiedot: uuden lisäys ja olemassaolevien päivitys

        public ActionResult Delete(string id)  //tapahtuman poisto klikatun rivin id:n perusteella
        {
            //avataan tietokantayhteys
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            bool OK = false;  // poiston onnistuminen

            //muutetaan selaimelta tullut string-muotoinen id int-muotoon
            int asiakasryhmäID = int.Parse(id);

            //etsitään id:n perusteella tapahtuman tiedot kannasta
            Asiakasryhmät dbItem = (from ar in entities.Asiakasryhmät
                                 where ar.AsiakasryhmäId == asiakasryhmäID
                                 select ar).FirstOrDefault();

            //jos tiedot löytyy
            if (dbItem != null)
            {
                //poistetaan
                entities.Asiakasryhmät.Remove(dbItem);
                entities.SaveChanges();
                OK = true;
            }

            //suljetaan tietokantayhteys
            entities.Dispose();

            //palautetaan poistokuittaus selaimelle
            return Json(OK, JsonRequestBehavior.AllowGet);
        }
    }

}
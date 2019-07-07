using CRM_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Demo.Controllers
{
    public class AsiakastapahtumaController : Controller
    {
        // GET: Asiakastapahtuma
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetList()
        {
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Haetaan kaikki tiedot Tapahtumat-taulusta listalle

            //List<Tapahtumat> tapahtumat = entities.Tapahtumat.ToList();  //tulee undefined rivejä, tuo kaikki sarakkeet kannasta

            //var tapahtumat = (from t in entities.Tapahtumat
            //                  select t).ToList();    // tästäkin tulee undefined rivejä, tuo kaikki sarakkeet kannasta 

            var tapahtumat = (from t in entities.Tapahtumat         //tällä voi valita, mitä sarakkeita kannasta haetaan
                              join a in entities.Asiakkaat
                              on t.AsiakasId  equals a.AsiakasId
                              join tl in entities.Tapahtumalajit
                              on t.TapahtumalajiId equals tl.TapahtumalajiId
                              select new
                              {
                                  t.TapahtumaId,
                                  t.AsiakasId,
                                  a.Etunimi,
                                  a.Sukunimi,
                                  t.TapahtumalajiId,
                                  tl.TapahtumalajiNimi,
                                  t.TapahtumaPvm,
                                  t.TapahtumaKlo,
                                  t.TapahtumaKuvaus
                              }).ToList();




            //Muutetaan data json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(tapahtumat, serializerSettings);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }  //Haetaan tiedot kannasta


        public JsonResult GetSingleEvent(string id)  //tapahtuman tietojen haku kannasta klikatun rivin id:n perusteella
        {
            //Avataan tietokanta yhteys, luodaan entiteettiolio
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //muutetaan modaali-ikkunasta tullut string-tyyppinen tapahtumaId int-tyyppiseksi
            int tapahtumaID = int.Parse(id);

            //haetaan Tapahtumat taulusta kaikki data
            var tapahtuma = (from t in entities.Tapahtumat
                             where t.TapahtumaId == tapahtumaID
                             select t).FirstOrDefault();

            //Muutetaan olio json-muotoon toimitettavaksi selaimelle.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(tapahtuma, serializerSettings);

            //Suljetaan tietokantayhteys
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Palautetaan data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Update(Tapahtumat tapahtumat)
        {
            //TIETOJEN LISÄYS JA PÄIVITYS

            bool OK = false;   //tallennuksen onnistuminen

            //UUSIEN TIETOJEN LISÄYS
            //Uusia tietoja lisätään vain mikäli asiakasId ja TapahtumalajiId eivät ole tyhjiä
            if ((tapahtumat.AsiakasId != null) &&
                (tapahtumat.TapahtumalajiId != null))
            {
                //avataan tietokantayhteys = uusi entiteettiolio
                ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

                //luodaan uusi muuttuja johon asetetaan selaimesta tullut tieto TapahtumaId:stä
                int tapahtumaId = tapahtumat.TapahtumaId;

                if (tapahtumaId == 0)
                {
                    //tallennetaan uuden tapahtuman tiedot

                    //luodaan uusi Tapahtumat-tyyppinen olio dbItem, jonka avulla tiedot tallennetaan kantaan
                    Tapahtumat dbItem = new Tapahtumat()
                    {
                        //dbItemin arvot/tiedot, ei TapahtumaId:tä
                        AsiakasId = tapahtumat.AsiakasId,
                        TapahtumalajiId = tapahtumat.TapahtumalajiId,
                        TapahtumaPvm = tapahtumat.TapahtumaPvm,
                        TapahtumaKlo = tapahtumat.TapahtumaKlo,
                        TapahtumaKuvaus = tapahtumat.TapahtumaKuvaus
                    };

                    //Lisätään dbItem kantaan ja tallennetaan muutokset
                    entities.Tapahtumat.Add(dbItem);
                    entities.SaveChanges();

                    //tallennus on onnistunut
                    OK = true;
                }
                else
                {
                    //päivitetään valitun tapahtuman tietoja
                    //haetaan tiedot tietokannasta

                    Tapahtumat dbItem = (from t in entities.Tapahtumat
                                         where t.TapahtumaId == tapahtumaId
                                         select t).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.AsiakasId = tapahtumat.AsiakasId;
                        dbItem.TapahtumalajiId = tapahtumat.TapahtumalajiId;
                        dbItem.TapahtumaPvm = tapahtumat.TapahtumaPvm;
                        dbItem.TapahtumaKlo = tapahtumat.TapahtumaKlo;
                        dbItem.TapahtumaKuvaus = tapahtumat.TapahtumaKuvaus;
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


        public ActionResult Delete (string id)  //tapahtuman poisto klikatun rivin id:n perusteella
        {
            //avataan tietokantayhteys
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            bool OK = false;  // poiston onnistuminen

            //muutetaan selaimelta tullut string-muotoinen id int-muotoon
            int tapahtumaID = int.Parse(id);

            //etsitään id:n perusteella tapahtuman tiedot kannasta
            Tapahtumat dbItem = (from t in entities.Tapahtumat
                                 where t.TapahtumaId == tapahtumaID
                                 select t).FirstOrDefault();

            //jos tiedot löytyy
            if (dbItem != null)
            {
                //poistetaan
                entities.Tapahtumat.Remove(dbItem);
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

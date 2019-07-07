using CRM_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CRM_Demo.Controllers
{
    public class AsiakasController : Controller
    {
        // GET: Asiakas
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Haetaan Asiakkaat -taulusta kaikki data
            var asiakkaat = (from asi in entities.Asiakkaat
                             join ak in entities.Asiakaskategorialuokat
                             on asi.KategoriaId equals ak.KategoriaId
                             select new {
                                 asi.AsiakasId,
                                 asi.Etunimi,
                                 asi.Sukunimi,
                                 asi.Osoite,
                                 asi.Postinumero,
                                 asi.Puhelin,
                                 asi.Sähköposti,
                                 asi.KategoriaId,
                                 ak.KategoriaNimi,
                                 asi.Tila
                                 }).ToList();

            //Muutetaan data json -muotoon toimitettavaksi selaimelle. 
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            string json = JsonConvert.SerializeObject(asiakkaat, serializerSettings);

            //Suljetaan tietokantayhteys.
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingleGroup(string id)
        {
            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Muutetaan modaali-ikkunasta tullut string-tyyppinen AsiakasId int-tyyppiseksi
            int asiID = int.Parse(id);

            //Haetaan Asiakkaat -taulusta kaikki data
            var asiakas = (from asi in entities.Asiakkaat
                           where asi.AsiakasId == asiID
                           select asi).FirstOrDefault();

            //Muutetaan olio json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

            string json = JsonConvert.SerializeObject(asiakas, serializerSettings);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Asiakkaat asiakkaat)
        {
            //Tietojen päivitys ja uuden asiakkaan lisäys

            bool OK = false;    //tallennuksen onnistuminen

            //tietokantaan tallennetaan uusia tietoja vain, mikäli etunimi
            //ja sukunimi -kentät eivät ole tyhjiä
            if (!string.IsNullOrWhiteSpace(asiakkaat.Etunimi) &&
                !string.IsNullOrWhiteSpace(asiakkaat.Sukunimi))
            {
                //luodaan uusi entiteettiolio
                ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

                int asiakasid = asiakkaat.AsiakasId;

                if (asiakasid == 0)
                {
                    //Uuden asiakkaan lisääminen tietokantaan dbItem-nimisen olion avulla
                    Asiakkaat dbItem = new Asiakkaat()
                    {
                        //dbItemin arvot/tiedot
                        Etunimi = asiakkaat.Etunimi,
                        Sukunimi = asiakkaat.Sukunimi,
                        Osoite = asiakkaat.Osoite,
                        Postinumero = asiakkaat.Postinumero,
                        Puhelin = asiakkaat.Puhelin,
                        Sähköposti = asiakkaat.Sähköposti,
                        KategoriaId = asiakkaat.KategoriaId,
                        Tila = asiakkaat.Tila
                    };

                    //lisätään tietokantaan dbItemin tiedot ja tallennetaan muutokset
                    entities.Asiakkaat.Add(dbItem);
                    entities.SaveChanges();
                    OK = true;
                }
                else
                {
                    //muokataan olemassa olevia tietoja
                    //haetaan tiedot tietokannasta

                    Asiakkaat dbItem = (from asi in entities.Asiakkaat
                                        where asi.AsiakasId == asiakasid
                                        select asi).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.Etunimi = asiakkaat.Etunimi;
                        dbItem.Sukunimi = asiakkaat.Sukunimi;
                        dbItem.Osoite = asiakkaat.Osoite;
                        dbItem.Postinumero = asiakkaat.Postinumero;
                        dbItem.Puhelin = asiakkaat.Puhelin;
                        dbItem.Sähköposti = asiakkaat.Sähköposti;
                        dbItem.KategoriaId = asiakkaat.KategoriaId;
                        dbItem.Tila = asiakkaat.Tila;

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

        //Asiakkaan poisto
        public ActionResult Delete(string id)
        {
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            // etsitään id:n perusteella asiakasrivi kannasta
            bool OK = false;

            int asiakasid = int.Parse(id);

            Asiakkaat dbItem = (from asi in entities.Asiakkaat
                                where asi.AsiakasId == asiakasid
                                select asi).FirstOrDefault();

            //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
            if (dbItem != null)
            {
                //tietokannasta poisto
                entities.Asiakkaat.Remove(dbItem);
                entities.SaveChanges();
                OK = true;
            }
            //suljetaan tietokantayhteys
            entities.Dispose();

            //palautetaan tallennuskuittaus selaimelle (muuttuja OK)
            return Json(OK, JsonRequestBehavior.AllowGet);
        }
    }
}

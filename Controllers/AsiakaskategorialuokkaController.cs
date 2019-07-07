using CRM_Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CRM_Demo.Controllers
{
    public class AsiakaskategorialuokkaController : Controller

    {
        // GET: Asiakaskategorialuokka
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList()
        {
            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Haetaan Asiakaskategorialuokat -taulusta kaikki data
            

            var asiakaskategoriat = (from ak in entities.Asiakaskategorialuokat
            select new {
                   KategoriaId = ak.KategoriaId,
                   KategoriaNimi = ak.KategoriaNimi,
                   KategoriaKuvaus = ak.KategoriaKuvaus}).ToList();

            // 1. versio Muutetaan data json -muotoon toimitettavaksi selaimelle.
            string json = JsonConvert.SerializeObject(asiakaskategoriat);

            //Toinen versio edeltävään:
            //Muutetaan data json -muotoon toimitettavaksi selaimelle.

            //var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            //string json = JsonConvert.SerializeObject(asiakaskategoriat, serializerSettings);

            //Suljetaan tietokantayhteys (molemmissa versioissa)
            entities.Dispose();


            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";


            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingleGategory(string id)
        {
            //Haetaan tietokannasta "klikatun" kategorian tiedot

            //Luodaan uusi entiteettiolio 
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //Muutetaan modaali-ikkunasta tullut string-tyyppinen katogoriaId int-tyyppiseksi
            int ID = int.Parse(id);

            //Haetaan Asiakaskategorialuokka -taulusta kaikki data
            var asiakaskategoria = (from ak in entities.Asiakaskategorialuokat
                                    where ak.KategoriaId == ID
                                    select ak).FirstOrDefault();

            //Muutetaan olio json -muotoon toimitettavaksi selaimelle. Suljetaan tietokantayhteys.
            var serializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

            string json = JsonConvert.SerializeObject(asiakaskategoria, serializerSettings);
            entities.Dispose();

            //ohitetaan välimuisti, jotta näyttö päivittyy (IE-selainta varten) 
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            //Lähetetään data selaimelle
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Asiakaskategorialuokat asiakaskategorialuokka)
        {
                        
            // TIETOJEN PÄIVITYS JA UUDEN ASIAKASKATEGORIAN LISÄYS

            bool OK = false;    //tallennuksen onnistuminen

            //tietokantaan tallennetaan uusia tietoja vain, mikäli kategorian  nimi
            //ja kuvaus -kentät eivät ole tyhjiä
            if (!string.IsNullOrWhiteSpace(asiakaskategorialuokka.KategoriaNimi) &&
                !string.IsNullOrWhiteSpace(asiakaskategorialuokka.KategoriaKuvaus))
            {
                //luodaan uusi entiteettiolio
                ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

                int kategoriaid = asiakaskategorialuokka.KategoriaId;

                

                if (kategoriaid == 0)
                {

                    //Uuden kategorian lisääminen tietokantaan dbItem-nimisen olion avulla
                    Asiakaskategorialuokat dbItem = new Asiakaskategorialuokat()
                    {
                        //dbItemin arvot/tiedot

                        
                    KategoriaNimi = asiakaskategorialuokka.KategoriaNimi,
                        KategoriaKuvaus = asiakaskategorialuokka.KategoriaKuvaus
                        
                };
                    

                    //lisätään tietokantaan dbItemin tiedot ja tallennetaan muutokset
                    entities.Asiakaskategorialuokat.Add(dbItem);

                    entities.SaveChanges();
                    OK = true;
                    
                }
                else
                {
                    //muokataan olemassa olevia tietoja
                    //haetaan tiedot tietokannasta

                    Asiakaskategorialuokat dbItem = (from ak in entities.Asiakaskategorialuokat
                                                     where ak.KategoriaId == kategoriaid
                                                     select ak).FirstOrDefault();

                    //tallennetaan modaali-ikkunasta tulevat tiedot dbItem-olioon
                    if (dbItem != null)
                    {
                        dbItem.KategoriaNimi = asiakaskategorialuokka.KategoriaNimi;
                        dbItem.KategoriaKuvaus = asiakaskategorialuokka.KategoriaKuvaus;

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

        public ActionResult Delete(string id)
        {
            //luodaan uusi entiteettiolio
            ProjektitDBCareEntities entities = new ProjektitDBCareEntities();

            //tallennuksen onnistuminen
            bool OK = false;

            //muutetaan selaimelta tullut string-tyyppinen ryhmäId (muuttuja: id) int-tyyppiseksi
            int intID = int.Parse(id);

            //haetaan poistettavan ryhmän tiedot kannasta dbItem-olioon id:n perusteella
            Asiakaskategorialuokat dbItem = (from ak in entities.Asiakaskategorialuokat
                                         where ak.KategoriaId == intID
                                         select ak).FirstOrDefault();

            //jos tiedot löytyy
            if (dbItem != null)
            {
                //poistetaan tiedot 
                entities.Asiakaskategorialuokat.Remove(dbItem);

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


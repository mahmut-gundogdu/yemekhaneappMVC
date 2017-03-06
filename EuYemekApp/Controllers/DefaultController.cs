using EuYemekApp.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EuYemekApp.Controllers
{
    public class DefaultController : Controller
    {
        const string DataUrl = "http://www.erciyes.edu.tr/kategori/KAMPUSTE-YASAM/Yemek-Hizmetleri/22/167";
        // GET: Default
        [OutputCache(Duration = 40, VaryByParam = "none")]
        public async Task<ActionResult> Index()
        {
            if (Request.AcceptTypes.Contains("text/plain"))
            {
                return await Text();
            }
            else
            {
                DefaultViewModel Model = await GununMenusunuGetirAsync(DataUrl);
                Model.ProjeID = System.Configuration.ConfigurationManager.AppSettings["ProjectID"];
                return View(Model);
            }
        }
        //  [OutputCache(Duration = 40, VaryByParam = "none")]
        public async Task<ActionResult> Text()
        {

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return new HttpStatusCodeResult(400, "Bu gün ABIS yemekhaneye gitmez!");
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                return Content("Hayırlı cumalar.", "text/plain");
            }
            else
            {
                DefaultViewModel Model = await GununMenusunuGetirAsync(DataUrl);
                Model.ProjeID = System.Configuration.ConfigurationManager.AppSettings["ProjectID"];

                Regex rx = new Regex(@"<br\s*\/?>");
                //   var x=rx.IsMatch(Model.Menu);*
                var result = rx.Replace(Model.Menu, string.Empty); //"\n\r");
                return Content(result, "text/plain");
            }
        }

        public static async Task<DefaultViewModel> GununMenusunuGetirAsync(string _url)
        {
            Uri url = new Uri(_url);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            string html = await client.DownloadStringTaskAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var PersonelNode = doc.DocumentNode.SelectSingleNode("//div[@class='Personel']");

            var ListeSatirNode = PersonelNode.SelectSingleNode("//div[@class='ListeSatir']");
            var TarihNode = ListeSatirNode.SelectSingleNode("//div[@class='YemekTarih']");
            var YemekNode = ListeSatirNode.SelectSingleNode("//div[@class='YemekListe']//ul");
            var Model = new DefaultViewModel();
            Model.Tarih = TarihNode.InnerText;
            string htmlMenu = YemekNode.InnerHtml;
            htmlMenu = htmlMenu.Replace("</li>", "<br />");
            htmlMenu = htmlMenu.Replace("<li>", "");
            Model.Menu = htmlMenu;
            return Model;
        }
    }
}
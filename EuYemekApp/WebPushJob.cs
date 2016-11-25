using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using EuYemekApp.Controllers;
using System.Web.Configuration;
using System.Text;

namespace EuYemekApp
{
    public class WebPushJob : IJob
    {

        const string DataUrl = "http://www.erciyes.edu.tr/kategori/KAMPUSTE-YASAM/Yemek-Hizmetleri/22/167";
        public async void Execute(IJobExecutionContext context)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
              return;
            }

            string token = WebConfigurationManager.AppSettings["AuthToken"];
            string projectID = WebConfigurationManager.AppSettings["ProjectID"];
            //projects/PROJECT_ID/notifications
            string url = "https://pushpad.xyz/projects/" + projectID + "/notifications";
            string link = "http://yemekhane.azurewebsites.net/";
            string icon = "http://yemekhane.azurewebsites.net/pushicon.png";

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Accept, "application/json");

                client.Headers.Add(HttpRequestHeader.Authorization, "Token token=\"" + token + "\"");
                var header = client.Headers;


                var model = await DefaultController.GununMenusunuGetirAsync(DataUrl);
                model.Menu = model.Menu
                    .Replace("<br />", "")
                    .Trim();
                if (model.Menu.Length > 120)
                {
                    model.Menu = model.Menu.Substring(0, 118);
                }
                string PostParam = "{\"notification\": { \"body\": \"" + model.Menu + "\", \"title\": \"" + model.Tarih
                    + "\", \"target_url\": \"" + link + "\", \"icon_url\": \"" + icon + "\", \"ttl\": " + 600 + " }}";


                byte[] postData = Encoding.UTF8.GetBytes(PostParam);
                var response = await client.UploadDataTaskAsync(url, postData);
            }

        }

    }
}

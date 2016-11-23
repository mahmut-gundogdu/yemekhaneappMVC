using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using EuYemekApp.Controllers;
using System.Web.Configuration;

namespace EuYemekApp
{
    public class WebPushJob : IJob
    {

        const string DataUrl = "http://www.erciyes.edu.tr/kategori/KAMPUSTE-YASAM/Yemek-Hizmetleri/22/167";
        public async void Execute(IJobExecutionContext context)
        {
            string token = WebConfigurationManager.AppSettings["AuthToken"]; 
            string projectID = WebConfigurationManager.AppSettings["ProjectID"];
            string url = "https://pushpad.xyz/projects/" + projectID + "/notification";
            string link = "http://yemekhane.azurewebsites.net/";
            string icon = "http://yemekhane.azurewebsites.net/pushicon.png";

            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Accept, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Token token\"" + token + "\"");

                var model = await DefaultController.GununMenusunuGetirAsync(DataUrl);
                string PostParam = string.Format("\"notification\": { \"body\": \"{0}\", \"title\": \"{1}\", \"target_url\": \"{2}\", \"icon_url\": \"{3}\", \"ttl\": {4} }",
                    model.Menu,
                    model.Tarih,
                    link,
                    icon,
                    600);


                var response = await client.UploadStringTaskAsync(url, PostParam);
            };

        }

    }
}

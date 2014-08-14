using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using LindugDemoService.Hubs;
using ExternalDb;
using Newtonsoft.Json.Linq;

namespace LindugDemoService.Controllers {
    public class ExternalDataController : ApiController {

        public ApiServices Services {
            get;
            set;
        }

        // GET api/ExternalData
        public async Task Get() {


            using(var db = new ExternalDatabase()) {

                var currentCustomer =  db.Customers.Add(new Customer() {
                    FirstName = string.Format("customer_{0}", Guid.NewGuid().ToString()),
                    LastName = string.Format("customer_{0}",Guid.NewGuid().ToString())

                });

                await db.SaveChangesAsync();


                IHubContext hubContext = Services.GetRealtime<MessageHub>();
                hubContext.Clients.All.Send(string.Format("New customer:{0} added.",currentCustomer.FirstName));

                try {
                    string wnsToast = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><toast><visual><binding template=\"ToastText01\"><text id=\"1\">{0}</text></binding></visual></toast>", currentCustomer.LastName);
                    WindowsPushMessage message = new WindowsPushMessage();
                    message.XmlPayload = wnsToast;
                    await Services.Push.SendAsync(message);

                }

                catch (Exception e) {
                    Services.Log.Error(e.ToString());
                }
            }


        }



    }
}

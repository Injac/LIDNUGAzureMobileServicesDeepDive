using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace LindugDemoService.Hubs {
    public class MessageHub :Hub {

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [AuthorizeLevel(AuthorizationLevel.User)]
        public string Send(string message) {

            return message;
        }
    }
}
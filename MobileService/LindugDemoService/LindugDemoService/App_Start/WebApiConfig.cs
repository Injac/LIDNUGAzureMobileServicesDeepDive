using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Web.Http;
using LindugDemoService.DataObjects;
using LindugDemoService.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Config;
using Microsoft.WindowsAzure.Mobile.Service.Description;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Autofac;
using System.Net.Http.Headers;

namespace LindugDemoService {
    public static class WebApiConfig {
        public static void Register() {

            SignalRExtensionConfig.Initialize();

            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();


            ConfigBuilder builder = new ConfigBuilder(options, (httpConfig, autofac) => {
                //Only valid users can register for notifications
                options.PushAuthorization = AuthorizationLevel.User;

                //Basic Authorization level for SignalR
                options.SetRealtimeAuthorization(AuthorizationLevel.User);

            });


            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(builder);

            //Just a sample, on how to set the required options to get rid
            //of some of the sample page error-messages.
            //You can read more here: http://blogs.msdn.com/b/yaohuang1/archive/2012/10/13/asp-net-web-api-help-page-part-2-providing-custom-samples-on-the-help-page.aspx
            config.SetSampleForType(
                "Currently not used.",
                new MediaTypeHeaderValue("application/x-www-form-urlencoded"),
                typeof(TodoItem));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            //Uncomment this lines, if you apply changes
            //to your database-model, and F5 the project
            //Before publishing, comment them out.
            //var migrator = new DbMigrator(new Configuration());
            //migrator.Update();

            //Comment out this line during development, and the
            //un-comment the above 2 lines. Before publishing, un-
            //comment this line and comment-out the two lines above.
            Database.SetInitializer(new MobileServiceInitializer());
        }
    }

    public class MobileServiceInitializer : DropCreateDatabaseIfModelChanges<MobileServiceContext> {
        protected override void Seed(MobileServiceContext context) {
            List<TodoItem> todoItems = new List<TodoItem> {
                new TodoItem { Id = "1", Text = "First item", Complete = false },
                new TodoItem { Id = "2", Text = "Second item", Complete = false },
                new TodoItem { Id = "3", Text = "First item", Complete = false },
                new TodoItem { Id = "4", Text = "Second item", Complete = false },
                new TodoItem { Id = "5", Text = "First item", Complete = false },
                new TodoItem { Id = "6", Text = "Second item", Complete = false },
            };

            foreach (TodoItem todoItem in todoItems) {
                context.Set<TodoItem>().Add(todoItem);
            }

            base.Seed(context);
        }
    }
}


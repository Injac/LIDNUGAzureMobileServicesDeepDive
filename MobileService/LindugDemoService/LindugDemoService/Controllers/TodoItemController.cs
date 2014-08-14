using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LindugDemoService.DataObjects;
using LindugDemoService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Description;
using System.Net.Http.Headers;
using System.Web.Http.Description;

namespace LindugDemoService.Controllers {
    /// <summary>
    /// The TodoItem TableController.
    /// </summary>
    public class TodoItemController : TableController<TodoItem> {
        protected override void Initialize(HttpControllerContext controllerContext) {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();

            //This shows how to set the XmlDocumentationProvider to suck your XML comments on
            //Controller actions into your sample page.
            this.Configuration.SetDocumentationProvider(new XmlDocumentationProvider(this.Services));


            DomainManager = new EntityDomainManager<TodoItem>(context, Request, Services);
        }


        /// <summary>
        /// Gets all todo items.
        /// </summary>
        /// <example>
        /// <code>
        ///     Console.WriteLine("Hello World");
        /// </code>
        /// </example>
        /// <returns></returns>
        public IQueryable<TodoItem> GetAllTodoItems() {
            return Query();
        }


        /// <summary>
        /// Gets the todo item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public SingleResult<TodoItem> GetTodoItem(string id) {
            return Lookup(id);
        }


        /// <summary>
        /// Patches the todo item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="patch">The patch.</param>
        /// <returns></returns>
        public Task<TodoItem> PatchTodoItem(string id, Delta<TodoItem> patch) {
            return UpdateAsync(id, patch);
        }


        /// <summary>
        /// Posts the todo item.
        /// </summary>
        /// <param name="item">A TodoItem to be added</param>
        /// <returns></returns>
        [ResponseType(typeof(TodoItem))]
        public async Task<IHttpActionResult> PostTodoItem(TodoItem item) {
            TodoItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }


        /// <summary>
        /// Deletes the todo item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Task DeleteTodoItem(string id) {
            return DeleteAsync(id);
        }
    }
}
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Windows.UI.Core;
using ClientPocos;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace lidnugdemo {


    public sealed partial class MainPage : Page {

        //The mobile services user
        private MobileServiceUser user;

        //Used for data-binding
        private MobileServiceCollection<TodoItem, TodoItem> items;

        //The reference to the TodoItem-Table
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        //Used for SignalR
        private HubConnection hubConnection;

        public MainPage() {
            this.InitializeComponent();
        }

        private async void InsertTodoItem(TodoItem todoItem) {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile Services has assigned an Id, the item is added to the CollectionView
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);
        }

        private async void RefreshTodoItems() {
            MobileServiceInvalidOperationException exception = null;

            try {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems
                items = await todoTable
                        .Where(todoItem => todoItem.Complete == false)
                        .ToCollectionAsync();
            }

            catch (MobileServiceInvalidOperationException e) {
                exception = e;
            }

            if (exception != null) {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }

            else {
                ListItems.ItemsSource = items;
                this.ButtonSave.IsEnabled = true;
            }
        }

        private async void UpdateCheckedTodoItem(TodoItem item) {
            // This code takes a freshly completed TodoItem and updates the database. When the MobileService
            // responds, the item is removed from the list
            await todoTable.UpdateAsync(item);
            items.Remove(item);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e) {
            RefreshTodoItems();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e) {
            var todoItem = new TodoItem { Text = TextInput.Text };
            InsertTodoItem(todoItem);
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e) {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            UpdateCheckedTodoItem(item);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {

            //Start the authentication
            await AuthenticateAsync();

            //Initialize SignalR
            await ConnectToSignalR();

            RefreshTodoItems();
        }

        private async System.Threading.Tasks.Task AuthenticateAsync() {
            while (user == null) {
                string message;

                try {
                    user = await App.MobileService
                           .LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                    message =
                        string.Format("You are now logged in - {0}", user.UserId);
                }

                catch (InvalidOperationException) {
                    message = "You must log in. Login Required";
                }

                var dialog = new MessageDialog(message);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }


        /// <summary>
        /// Connect to SignalR, create the proxy
        /// and set the required headers.
        /// </summary>
        /// <returns></returns>
        private async Task ConnectToSignalR() {

            hubConnection = new HubConnection(App.MobileService.ApplicationUri.AbsoluteUri);

            if (user != null) {
                hubConnection.Headers["x-zumo-auth"] = user.MobileServiceAuthenticationToken;
            }

            else {
                hubConnection.Headers["x-zumo-application"] = App.MobileService.ApplicationKey;
            }

            //Creating the hub proxy. That allows us to send and receive
            //messages
            IHubProxy proxy = hubConnection.CreateHubProxy("MessageHub");
            await hubConnection.Start();

            //Sending messages using SignalR
            string result = await proxy.Invoke<string>("Send", "Hello World!");
            var invokeDialog = new MessageDialog(result);
            await invokeDialog.ShowAsync();

            //Receiving messages from SignalR
            proxy.On<string>("Send", async (msg) => {

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () => {
                    this.txtSigMessages.Text = msg;
                });
            });
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void Button_Click(object sender, RoutedEventArgs e) {
            try {

                //Create the push-channel
                var channel = await
                              Windows.Networking.PushNotifications.
                              PushNotificationChannelManager.
                              CreatePushNotificationChannelForApplicationAsync();
                //And register the push-channel
                await App.MobileService.GetPush().RegisterNativeAsync(channel.Uri);

                //Call a custom API
                await App.MobileService.InvokeApiAsync("ExternalData", HttpMethod.Get, null);
            }

            catch (MobileServiceInvalidOperationException ex) {

                throw;
            }

        }
    }
}

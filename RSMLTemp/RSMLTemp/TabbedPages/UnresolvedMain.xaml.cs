using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using RSMLTemp.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnresolvedMain : ContentPage
    {
        public int list_count = -1;
        public int previous_store_number;
        private ValidStores current_store = new ValidStores();
        public UnresolvedMain()
        {
            InitializeComponent();
            UnresolvedIncidents();

            if (Device.RuntimePlatform == Device.Android)
            {
                Title = "UNRESOLVED INCIDENTS";
            }

            else if (Device.RuntimePlatform == Device.iOS)
            {
                Title = "UNRESOLVED";
            }

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    UnresolvedIncidents();
                });
                return true;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            /*using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Unresolved>();
                var unresolved_incidents = conn.Query<Unresolved>("SELECT * FROM Unresolved ORDER BY TimeOccured DESC");

                UnresolvedList.ItemsSource = unresolved_incidents;
            }*/

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<ValidStores>();
                var current_store_list = conn.Query<ValidStores>("SELECT * FROM ValidStores");
                if(current_store_list.Count() > 0)
                {
                    current_store = current_store_list[0];
                }

                else
                {
                    current_store.StoreNumber = 158;
                    current_store.StoreName = "Grand Rapids";
                    previous_store_number = 158;
                }
            }

            UnresolvedIncidents();
        }

        private void UnresolvedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Unresolved unresolved_incident = (Unresolved)e.SelectedItem;
            int Id = unresolved_incident.Id;
            string DeviceId = unresolved_incident.DeviceId;
            string SuspiciousActivities = unresolved_incident.SuspiciousActivities;
            string _Date = unresolved_incident._Date;
            double TimeOccured = unresolved_incident.TimeOccured;
            int StoreNumber = unresolved_incident.StoreNumber;
            string StoreName = unresolved_incident.StoreName;
            
            UnresolvedDetailed unresolved_detailed_page = new UnresolvedDetailed(Id, DeviceId, SuspiciousActivities, _Date, TimeOccured, StoreNumber, StoreName);

            this.Navigation.PushModalAsync(unresolved_detailed_page);
        }

        public async void UnresolvedIncidents()
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/Unresolveds1");
            var incidents_list = JsonConvert.DeserializeObject<List<Unresolved>>(response);
            var new_incidents_list = incidents_list.OrderByDescending(x => x.TimeOccured);
            List<Unresolved> new_incidents_list2 = new List<Unresolved>();
            foreach(var item in new_incidents_list)
            {
                if(item.StoreNumber == current_store.StoreNumber)
                {
                    new_incidents_list2.Add(item);
                }
            }
            int incident_count = new_incidents_list2.Count();
            Console.WriteLine(current_store.StoreNumber);
            Console.WriteLine(previous_store_number);
            if(list_count != incident_count)
            {
                if(list_count < incident_count)
                {
                    if(list_count != -1 && current_store.StoreNumber == previous_store_number)
                    {
                        Console.WriteLine("Send notification");
                        DependencyService.Get<INotification>().CreateNotification("RSML", "A new incident has occurred");
                        list_count = incident_count;
                    }
                    
                    else
                    {
                        list_count = incident_count;
                    }
                }

                else if(list_count > incident_count)
                {
                    list_count = incident_count;
                }
            }
            UnresolvedList.ItemsSource = new_incidents_list2;
            previous_store_number = current_store.StoreNumber;
        }
    }
}
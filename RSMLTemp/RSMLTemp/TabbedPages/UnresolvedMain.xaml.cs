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
using Xamarin.Essentials;

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnresolvedMain : ContentPage
    {
        public int list_count = -1;
        public string previous_store_name;
        private ValidStores current_store = new ValidStores();
        public UnresolvedMain()
        {
            InitializeComponent();
            //UnresolvedIncidents();
            Console.WriteLine("DEVICE INFO: " + DeviceInfo.Model);

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
                    current_store.StoreName = "ST158";
                    current_store.StoreLocation = "1997 E Beltline Ave NE, Grand Rapids, MI 49525, USA";
                    previous_store_name = "ST158";
                }
            }

            //UnresolvedIncidents();
        }

        private void UnresolvedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Unresolved unresolved_incident = (Unresolved)e.SelectedItem;
            int Id = unresolved_incident.Id;
            string DeviceId = unresolved_incident.DeviceId;
            string SuspiciousActivities = unresolved_incident.SuspiciousActivities;
            string _Date = unresolved_incident._Date;
            double TimeOccured = unresolved_incident.TimeOccured;
            string StoreName = unresolved_incident.StoreName;
            string StoreLocation = unresolved_incident.StoreLocation;
            
            UnresolvedDetailed unresolved_detailed_page = new UnresolvedDetailed(Id, DeviceId, SuspiciousActivities, _Date, TimeOccured, StoreName, StoreLocation);

            this.Navigation.PushModalAsync(unresolved_detailed_page);
        }

        public async void UnresolvedIncidents()
        {
            //var httpClient = new HttpClient();
            //var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            //var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //httpClient.DefaultRequestHeaders.Authorization = header;

            var response = await App.httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/Unresolveds1");
            var incidents_list = JsonConvert.DeserializeObject<List<Unresolved>>(response);
            var new_incidents_list = incidents_list.OrderByDescending(x => x.TimeOccured);
            List<Unresolved> new_incidents_list2 = new List<Unresolved>();
            foreach(var item in new_incidents_list)
            {
                if(item.StoreName == current_store.StoreName)
                {
                    new_incidents_list2.Add(item);
                }
            }
            int incident_count = new_incidents_list2.Count();
            //Console.WriteLine(current_store.StoreName);
            //Console.WriteLine(previous_store_name);
            //Console.WriteLine("DEVICE INFO: " + DeviceInfo.Model);
            if(list_count != incident_count)
            {
                if(list_count < incident_count)
                {
                    if(list_count != -1 && current_store.StoreName == previous_store_name)
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
            previous_store_name = current_store.StoreName;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RSMLTemp.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using SQLite;
using System.Net.Http.Headers;

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmedDevicesInStorePage : ContentPage
    {
        public int list_count = -1;
        public string previous_store_name;
        private ValidStores current_store = new ValidStores();
        public ConfirmedDevicesInStorePage()
        {
            InitializeComponent();
            CurrentConfirmedDevices();

            if (Device.RuntimePlatform == Device.Android)
            {
                Title = "CONFIRMED DEVICES";
            }

            else if (Device.RuntimePlatform == Device.iOS)
            {
                Title = "CONFIRMED";
            }

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    CurrentConfirmedDevices();
                });
                return true;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<ValidStores>();
                var current_store_list = conn.Query<ValidStores>("SELECT * FROM ValidStores");
                if (current_store_list.Count() > 0)
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

            CurrentConfirmedDevices();
        }

        public async void CurrentConfirmedDevices()
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/ConfirmedDevicesInStores1");
            var incidents_list = JsonConvert.DeserializeObject<List<ConfirmedDevicesInStore>>(response);
            var new_incidents_list = incidents_list.OrderBy(x => x.LastSeenDepartment);
            List<ConfirmedDevicesInStore> new_incidents_list2 = new List<ConfirmedDevicesInStore>();
            foreach (var item in new_incidents_list)
            {
                if (item.StoreName == current_store.StoreName)
                {
                    new_incidents_list2.Add(item);
                }
            }
            int incident_count = new_incidents_list2.Count();
            if (list_count != incident_count)
            {
                if (list_count < incident_count)
                {
                    if (list_count != -1 && previous_store_name == current_store.StoreName)
                    {
                        Console.WriteLine("Send notification");
                        DependencyService.Get<INotification>().CreateNotification("RSML", "A confirmed shoplifting device has entered the store");
                        list_count = incident_count;
                    }

                    else
                    {
                        list_count = incident_count;
                    }
                }

                else if (list_count > incident_count)
                {
                    list_count = incident_count;
                }
            }
            ConfirmedList.ItemsSource = new_incidents_list2;
            previous_store_name = current_store.StoreName;
        }

        private void ConfirmedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ConfirmedDevicesInStore confirmed_devices_in_store = (ConfirmedDevicesInStore)e.SelectedItem;
            int Id = confirmed_devices_in_store.Id;
            string DeviceId = confirmed_devices_in_store.DeviceId;
            string SuspiciousActivities = confirmed_devices_in_store.SuspiciousActivities;
            string ZoneHistory = confirmed_devices_in_store.ZoneHistory;
            string LastSeenDepartment = confirmed_devices_in_store.LastSeenDepartment;
            double LastSeenTime = confirmed_devices_in_store.LastSeenTime;
            string StoreName = confirmed_devices_in_store.StoreName;
            string StoreLocation = confirmed_devices_in_store.StoreLocation;


            ConfirmedDevicesInStoreDetailed confirmed_detailed_page = new ConfirmedDevicesInStoreDetailed(Id, DeviceId, SuspiciousActivities, ZoneHistory, LastSeenDepartment, LastSeenTime, StoreName, StoreLocation);

            this.Navigation.PushModalAsync(confirmed_detailed_page);
        }
    }
}
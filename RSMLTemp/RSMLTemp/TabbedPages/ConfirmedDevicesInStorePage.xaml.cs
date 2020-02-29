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

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmedDevicesInStorePage : ContentPage
    {
        public int list_count = -1;
        public int previous_store_number;
        private ValidStores current_store = new ValidStores();
        public ConfirmedDevicesInStorePage()
        {
            InitializeComponent();
            CurrentConfirmedDevices();

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
                    current_store.StoreNumber = 158;
                    current_store.StoreName = "Grand Rapids";
                    previous_store_number = 158;
                }
            }

            CurrentConfirmedDevices();
        }

        public async void CurrentConfirmedDevices()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/ConfirmedDevicesInStores1");
            var incidents_list = JsonConvert.DeserializeObject<List<ConfirmedDevicesInStore>>(response);
            var new_incidents_list = incidents_list.OrderBy(x => x.LastSeenDepartment);
            List<ConfirmedDevicesInStore> new_incidents_list2 = new List<ConfirmedDevicesInStore>();
            foreach (var item in new_incidents_list)
            {
                if (item.StoreNumber == current_store.StoreNumber)
                {
                    new_incidents_list2.Add(item);
                }
            }
            int incident_count = new_incidents_list2.Count();
            if (list_count != incident_count)
            {
                if (list_count < incident_count)
                {
                    if (list_count != -1 && previous_store_number == current_store.StoreNumber)
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
            previous_store_number = current_store.StoreNumber;
        }

        private void ConfirmedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}
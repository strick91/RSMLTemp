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

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmedDevicesInStorePage : ContentPage
    {
        public int list_count = -1;
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
            CurrentConfirmedDevices();
        }

        public async void CurrentConfirmedDevices()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/ConfirmedDevicesInStores1");
            var incidents_list = JsonConvert.DeserializeObject<List<ConfirmedDevicesInStore>>(response);
            var new_incidents_list = incidents_list.OrderBy(x => x.LastSeenDepartment);
            int incident_count = new_incidents_list.Count();
            if (list_count != incident_count)
            {
                if (list_count < incident_count)
                {
                    if (list_count != -1)
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
            ConfirmedList.ItemsSource = new_incidents_list;
        }

        private void ConfirmedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}
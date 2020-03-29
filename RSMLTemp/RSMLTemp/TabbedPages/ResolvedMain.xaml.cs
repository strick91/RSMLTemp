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
using System.Web;
using System.Net.Http.Headers;

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResolvedMain : ContentPage
    {
        public int previous_store_number;
        private ValidStores current_store = new ValidStores();
        public ResolvedMain()
        {
            InitializeComponent();
            ResolvedIncidents();

            if (Device.RuntimePlatform == Device.Android)
            {
                Title = "RESOLVED INCIDENTS";
            }

            else if (Device.RuntimePlatform == Device.iOS)
            {
                Title = "RESOLVED";
            }

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ResolvedIncidents();
                });
                return true;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            /*using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Resolved>();
                var Resolved_incidents = conn.Query<Resolved>("SELECT * FROM Resolved ORDER BY TimeResolved DESC");

                ResolvedList.ItemsSource = Resolved_incidents;
            }*/

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

            ResolvedIncidents();
        }

        private void ResolvedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Resolved resolved_incident = (Resolved)e.SelectedItem;
            int Id = resolved_incident.Id;
            string DeviceId = resolved_incident.DeviceId;
            string Department = resolved_incident.Department;
            string ThreatLevel = resolved_incident.ThreatLevel;
            DateTime TimeOccured = resolved_incident.TimeOccured;
            DateTime TimeResolved = resolved_incident.TimeResolved;
            string Verdict = resolved_incident.Verdict;

            ResolvedDetailed resolved_detailed_page = new ResolvedDetailed(Id, DeviceId, Department, ThreatLevel, TimeOccured, TimeResolved, Verdict);

            this.Navigation.PushModalAsync(resolved_detailed_page);
        }

        public async void ResolvedIncidents()
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/Resolveds1");
            var incidents_list = JsonConvert.DeserializeObject<List<Resolved>>(response);
            var new_incidents_list = incidents_list.OrderByDescending(x => x.TimeResolved);
            List<Resolved> new_incidents_list2 = new List<Resolved>();
            foreach (var item in new_incidents_list)
            {
                if (item.StoreNumber == current_store.StoreNumber)
                {
                    if (new_incidents_list2.Count() < 75)
                    {
                        new_incidents_list2.Add(item);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            ResolvedList.ItemsSource = new_incidents_list2;
        }
    }
}
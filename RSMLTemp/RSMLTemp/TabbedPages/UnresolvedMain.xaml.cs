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

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnresolvedMain : TabbedPage
    {
        public int list_count = -1;
        public UnresolvedMain()
        {
            InitializeComponent();
            UnresolvedIncidents();

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

            UnresolvedIncidents();
        }

        private void UnresolvedList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Unresolved unresolved_incident = (Unresolved)e.SelectedItem;
            int Id = unresolved_incident.Id;
            string DeviceId = unresolved_incident.DeviceId;
            string Department = unresolved_incident.Department;
            string ThreatLevel = unresolved_incident.ThreatLevel;
            DateTime TimeOccured = unresolved_incident.TimeOccured;
            
            UnresolvedDetailed unresolved_detailed_page = new UnresolvedDetailed(Id, DeviceId, Department, ThreatLevel, TimeOccured);

            this.Navigation.PushModalAsync(unresolved_detailed_page);
        }

        public async void UnresolvedIncidents()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/Unresolveds1");
            var incidents_list = JsonConvert.DeserializeObject<List<Unresolved>>(response);
            var new_incidents_list = incidents_list.OrderByDescending(x => x.TimeOccured);
            int incident_count = new_incidents_list.Count();
            if(list_count != incident_count)
            {
                if(list_count < incident_count)
                {
                    if(list_count != -1)
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
            UnresolvedList.ItemsSource = new_incidents_list;
        }
    }
}
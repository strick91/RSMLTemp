using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RSMLTemp.Classes;
using SQLite;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace RSMLTemp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnresolvedDetailed : ContentPage
    {
        Resolved resolved_incident_object;
        Unresolved unresolved_incident_object;
        ConfirmedDevices confirmed_devices_object;
        public UnresolvedDetailed(int Id, string DeviceId, string SuspiciousActivities, string _Date, double TimeOccured, int StoreNumber, string StoreName)
        {
            InitializeComponent();

            DeviceIdField.Text = DeviceId;
            SuspiciousActivities = SuspiciousActivities.Replace(",", Environment.NewLine);
            Console.WriteLine("Suspicious Activities:" + SuspiciousActivities);
            SuspiciousActivitiesField.Text = SuspiciousActivities;
            _DateField.Text = _Date;
            //TimeOccuredField.Text = TimeOccured.ToString();

            resolved_incident_object = new Resolved();
            resolved_incident_object.DeviceId = DeviceId;
            resolved_incident_object.SuspiciousActivities = SuspiciousActivities;
            resolved_incident_object._Date = _Date;
            resolved_incident_object.StoreNumber = StoreNumber;
            resolved_incident_object.StoreName = StoreName;
            var temp_time = DateTime.UtcNow;
            TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            DateTime real_time = TimeZoneInfo.ConvertTimeFromUtc(temp_time, eastern);
            resolved_incident_object.TimeResolved = real_time;

            unresolved_incident_object = new Unresolved();
            unresolved_incident_object.Id = Id;
            unresolved_incident_object.DeviceId = DeviceId;
            unresolved_incident_object.SuspiciousActivities = SuspiciousActivities;
            unresolved_incident_object._Date = _Date;
            unresolved_incident_object.TimeOccured = TimeOccured;
            unresolved_incident_object.StoreNumber = StoreNumber;
            unresolved_incident_object.StoreName = StoreName;

            confirmed_devices_object = new ConfirmedDevices();
            confirmed_devices_object.DeviceId = DeviceId;
        }

        private void unresolvedBackButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        private void resolveShoplifting_Clicked(object sender, EventArgs e)
        {
            /*using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Resolved>();
                resolved_incident_object.Verdict = "Confirmed Shoplifting";
                int rowsAdded = conn.Insert(resolved_incident_object);
            }

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Unresolved>();
                var deleted = conn.Query<Unresolved>("DELETE FROM Unresolved WHERE Id='" + unresolved_incident_object.Id + "'");
            }*/

            DropUnresolvedIncident(unresolved_incident_object.Id);
            resolved_incident_object.Verdict = "Confirmed Shoplifting";
            AddResolvedIncident(resolved_incident_object);
            AddConfirmedDevice(confirmed_devices_object);

            this.Navigation.PopModalAsync();
        }

        private void resolveNotShoplifting_Clicked(object sender, EventArgs e)
        {
            /*using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Resolved>();
                resolved_incident_object.Verdict = "Not Shoplifting";
                int rowsAdded = conn.Insert(resolved_incident_object);
            }

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Unresolved>();
                var deleted = conn.Query<Unresolved>("DELETE FROM Unresolved WHERE Id='" + unresolved_incident_object.Id + "'");
            }*/

            DropUnresolvedIncident(unresolved_incident_object.Id);
            resolved_incident_object.Verdict = "Not Shoplifting";
            AddResolvedIncident(resolved_incident_object);

            this.Navigation.PopModalAsync();
        }

        public async void DropUnresolvedIncident(int Id)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            await httpClient.DeleteAsync("https://rsml.azurewebsites.net/api/Unresolveds1/" + Id.ToString());
        }

        private async void AddResolvedIncident(Resolved resolved_incident)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
            string json_object = JsonConvert.SerializeObject(resolved_incident);
            var content = new StringContent(json_object, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("https://rsml.azurewebsites.net/api/Resolveds1", content);
        }

        private async void AddConfirmedDevice(ConfirmedDevices device)
        {
            var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;

            bool add_device = true;
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/ConfirmedDevices1");
            var devices_list = JsonConvert.DeserializeObject<List<ConfirmedDevices>>(response);
            for(int i = 0; i < devices_list.Count(); i++)
            {
                if(devices_list[i].DeviceId == device.DeviceId)
                {
                    add_device = false;
                }
            }

            if(add_device == true)
            {
                string json_object = JsonConvert.SerializeObject(device);
                var content = new StringContent(json_object, Encoding.UTF8, "application/json");
                await httpClient.PostAsync("https://rsml.azurewebsites.net/api/ConfirmedDevices1", content);
            }
        }
    }
}
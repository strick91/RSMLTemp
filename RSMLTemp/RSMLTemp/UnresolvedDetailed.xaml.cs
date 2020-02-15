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

namespace RSMLTemp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnresolvedDetailed : ContentPage
    {
        Resolved resolved_incident_object;
        Unresolved unresolved_incident_object;
        ConfirmedDevices confirmed_devices_object;
        public UnresolvedDetailed(int Id, string DeviceId, string Department, string ThreatLevel, DateTime TimeOccured)
        {
            InitializeComponent();

            DeviceIdField.Text = "Device Id: " + DeviceId;
            DepartmentField.Text = "Department: " + Department;
            ThreatLevelField.Text = "Threat Level: " + ThreatLevel;
            TimeOccuredField.Text = "Time of Incident: " + TimeOccured;

            resolved_incident_object = new Resolved();
            resolved_incident_object.DeviceId = DeviceId;
            resolved_incident_object.Department = Department;
            resolved_incident_object.ThreatLevel = ThreatLevel;
            resolved_incident_object.TimeOccured = TimeOccured;
            var temp_time = DateTime.UtcNow;
            TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            DateTime real_time = TimeZoneInfo.ConvertTimeFromUtc(temp_time, eastern);
            resolved_incident_object.TimeResolved = real_time;

            unresolved_incident_object = new Unresolved();
            unresolved_incident_object.Id = Id;
            unresolved_incident_object.DeviceId = DeviceId;
            unresolved_incident_object.Department = Department;
            unresolved_incident_object.ThreatLevel = ThreatLevel;
            unresolved_incident_object.TimeOccured = TimeOccured;

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
            await httpClient.DeleteAsync("https://rsml.azurewebsites.net/api/Unresolveds1/" + Id.ToString());
        }

        private async void AddResolvedIncident(Resolved resolved_incident)
        {
            var httpClient = new HttpClient();
            string json_object = JsonConvert.SerializeObject(resolved_incident);
            var content = new StringContent(json_object, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("https://rsml.azurewebsites.net/api/Resolveds1", content);
        }

        private async void AddConfirmedDevice(ConfirmedDevices device)
        {
            var httpClient = new HttpClient();
            string json_object = JsonConvert.SerializeObject(device);
            var content = new StringContent(json_object, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("https://rsml.azurewebsites.net/api/ConfirmedDevices1", content);
        }
    }
}
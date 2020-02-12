using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RSMLTemp.Classes;
using SQLite;

namespace RSMLTemp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnresolvedDetailed : ContentPage
    {
        Resolved resolved_incident_object;
        Unresolved unresolved_incident_object;
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
            resolved_incident_object.TimeResolved = DateTime.Now;

            unresolved_incident_object = new Unresolved();
            unresolved_incident_object.Id = Id;
            unresolved_incident_object.DeviceId = DeviceId;
            unresolved_incident_object.Department = Department;
            unresolved_incident_object.ThreatLevel = ThreatLevel;
            unresolved_incident_object.TimeOccured = TimeOccured;
        }

        private void unresolvedBackButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        private void resolveShoplifting_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Resolved>();
                resolved_incident_object.Verdict = "Confirmed Shoplifting";
                int rowsAdded = conn.Insert(resolved_incident_object);
            }

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Unresolved>();
                var deleted = conn.Query<Unresolved>("DELETE FROM Unresolved WHERE Id='" + unresolved_incident_object.Id + "'");
            }

            this.Navigation.PopModalAsync();
        }

        private void resolveNotShoplifting_Clicked(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Resolved>();
                resolved_incident_object.Verdict = "Not Shoplifting";
                int rowsAdded = conn.Insert(resolved_incident_object);
            }

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Unresolved>();
                var deleted = conn.Query<Unresolved>("DELETE FROM Unresolved WHERE Id='" + unresolved_incident_object.Id + "'");
            }

            this.Navigation.PopModalAsync();
        }
    }
}
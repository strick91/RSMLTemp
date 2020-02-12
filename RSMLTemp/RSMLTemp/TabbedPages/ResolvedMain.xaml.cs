using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using RSMLTemp.Classes;

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResolvedMain : ContentPage
    {
        public ResolvedMain()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Resolved>();
                var Resolved_incidents = conn.Query<Resolved>("SELECT * FROM Resolved ORDER BY TimeResolved DESC");

                ResolvedList.ItemsSource = Resolved_incidents;
            }
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
    }
}
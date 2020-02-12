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
    public partial class UnresolvedMain : TabbedPage
    {
        public UnresolvedMain()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Unresolved>();
                var unresolved_incidents = conn.Query<Unresolved>("SELECT * FROM Unresolved ORDER BY TimeOccured DESC");

                UnresolvedList.ItemsSource = unresolved_incidents;
            }
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
    }
}
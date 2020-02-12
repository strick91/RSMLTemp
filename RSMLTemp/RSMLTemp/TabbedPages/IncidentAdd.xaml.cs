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
    public partial class IncidentAdd : ContentPage
    {
        public IncidentAdd()
        {
            InitializeComponent();
        }

        private void addButton_Clicked(object sender, EventArgs e)
        {
            Unresolved unresolved_incident = new Unresolved()
            {
                DeviceId = DeviceIdEntry.Text,
                Department = DepartmentEntry.Text,
                ThreatLevel = ThreatLevelEntry.Text,
                TimeOccured = Convert.ToDateTime(TimeOccuredEntry.Text)
            };

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Unresolved>();
                int rowsAdded = conn.Insert(unresolved_incident);
            }

            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RSMLTemp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmedDevicesInStoreDetailed : ContentPage
    {
        public ConfirmedDevicesInStoreDetailed(int Id, string DeviceId, string SuspiciousActivities, string ZoneHistory, string LastSeenDepartment, double LastSeenTime, int StoreNumber, string StoreName)
        {
            InitializeComponent();

            DeviceIdField.Text = DeviceId;
            if (SuspiciousActivities != null)
            {
                SuspiciousActivities = SuspiciousActivities.Replace(",", Environment.NewLine);
            }
            SuspiciousActivitiesField.Text = SuspiciousActivities;
            DateTime LastSeenTime_converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            LastSeenTime_converted = LastSeenTime_converted.AddSeconds(LastSeenTime);
            TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            LastSeenTime_converted = TimeZoneInfo.ConvertTimeFromUtc(LastSeenTime_converted, eastern);
            //LastSeenDepartment = LastSeenDepartment + " at " + LastSeenTime_converted.ToString();
            //LastSeenDepartmentField.Text = LastSeenDepartment;
            if (ZoneHistory != null)
            {
                ZoneHistory = ZoneHistory.Replace(",", Environment.NewLine);
            }
            ZoneHistoryField.Text = ZoneHistory;
        }

        private void confirmedBackButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
    }
}
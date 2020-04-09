using Newtonsoft.Json;
using RSMLTemp.Classes;
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
        public ConfirmedDevicesInStoreDetailed(int Id, string DeviceId, string SuspiciousActivities, string ZoneHistory, string LastSeenDepartment, double LastSeenTime, string StoreName, string StoreLocation)
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

            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    RefreshConfirmedDevice(Id);
                });
                return true;
            });
        }

        private void confirmedBackButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        public async void RefreshConfirmedDevice(int Id)
        {
            try
            {
                var response = await App.httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/ConfirmedDevicesInStores1/" + Id.ToString());
                Console.WriteLine("RESPONSE: " + response);
                var confirmed_device = JsonConvert.DeserializeObject<ConfirmedDevicesInStore>(response);

                DeviceIdField.Text = confirmed_device.DeviceId;
                if (confirmed_device.SuspiciousActivities != null)
                {
                    confirmed_device.SuspiciousActivities = confirmed_device.SuspiciousActivities.Replace(",", Environment.NewLine);
                }
                SuspiciousActivitiesField.Text = confirmed_device.SuspiciousActivities;
                DateTime LastSeenTime_converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                LastSeenTime_converted = LastSeenTime_converted.AddSeconds(confirmed_device.LastSeenTime);
                TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
                LastSeenTime_converted = TimeZoneInfo.ConvertTimeFromUtc(LastSeenTime_converted, eastern);
                //LastSeenDepartment = LastSeenDepartment + " at " + LastSeenTime_converted.ToString();
                //LastSeenDepartmentField.Text = LastSeenDepartment;
                if (confirmed_device.ZoneHistory != null)
                {
                    confirmed_device.ZoneHistory = confirmed_device.ZoneHistory.Replace(",", Environment.NewLine);
                }
                ZoneHistoryField.Text = confirmed_device.ZoneHistory;
            }

            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
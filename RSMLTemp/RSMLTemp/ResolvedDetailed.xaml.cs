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
    public partial class ResolvedDetailed : ContentPage
    {
        public ResolvedDetailed(int Id, string DeviceId, string SuspiciousActivities, string _Date, DateTime TimeResolved, string Verdict)
        {
            InitializeComponent();

            DeviceIdField.Text = DeviceId;
            SuspiciousActivities = SuspiciousActivities.Replace(",", Environment.NewLine);
            SuspiciousActivitiesField.Text = SuspiciousActivities;
            _DateField.Text = _Date;
            TimeResolvedField.Text = TimeResolved.ToString();
            VerdictField.Text = Verdict;
        }

        private void resolvedBackButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
    }
}
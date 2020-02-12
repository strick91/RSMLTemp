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
        public ResolvedDetailed(int Id, string DeviceId, string Department, string ThreatLevel, DateTime TimeOccured, DateTime TimeResolved, string Verdict)
        {
            InitializeComponent();

            DeviceIdField.Text = "Device Id: " + DeviceId;
            DepartmentField.Text = "Department: " + Department;
            ThreatLevelField.Text = "Threat Level: " + ThreatLevel;
            TimeOccuredField.Text = "Time of Incident: " + TimeOccured;
            TimeResolvedField.Text = "Time Resolved: " + TimeResolved;
            VerdictField.Text = "Verdict: " + Verdict;
        }

        private void resolvedBackButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
    }
}
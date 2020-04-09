using RSMLTemp.TabbedPages;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RSMLTemp
{
    public partial class App : Application
    {
        public static string FilePath;
        public static HttpClient httpClient = new HttpClient();
        public App(string filePath)
        {
            InitializeComponent();

            MainPage = new UnresolvedMainTabbed();
            FilePath = filePath;
            var byteArray = Encoding.ASCII.GetBytes("TeamMeijer:Need Anything?");
            var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.Authorization = header;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

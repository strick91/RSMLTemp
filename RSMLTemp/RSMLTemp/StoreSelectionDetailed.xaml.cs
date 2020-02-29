using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Net.Http;
using Newtonsoft.Json;
using RSMLTemp.Classes;

namespace RSMLTemp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreSelectionDetailed : ContentPage
    {
        public StoreSelectionDetailed()
        {
            InitializeComponent();
            StoreSelect();
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        public async void StoreSelect()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://rsml.azurewebsites.net/api/ValidStores1");
            var incidents_list = JsonConvert.DeserializeObject<List<ValidStores>>(response);
            var new_incidents_list = incidents_list.OrderByDescending(x => x.StoreName);

            StoreSelectionList.ItemsSource = new_incidents_list;
        }

        private void StoreSelectionList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ValidStores current_store = (ValidStores)e.SelectedItem;
            //Application.Current.Properties["StoreNumber"] = current_store.StoreNumber;
            //Application.Current.Properties["StoreName"] = current_store.StoreName;

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<ValidStores>();
                conn.Execute("DELETE FROM ValidStores");
                int rowsAdded = conn.Insert(current_store);
            }

            this.Navigation.PopModalAsync();
        }
    }
}
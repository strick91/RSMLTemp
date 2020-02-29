using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RSMLTemp.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using SQLite;

namespace RSMLTemp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreSelection : ContentPage
    {
        private ValidStores current_store = new ValidStores();
        public StoreSelection()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<ValidStores>();
                var current_store_list = conn.Query<ValidStores>("SELECT * FROM ValidStores");
                if (current_store_list.Count() > 0)
                {
                    current_store = current_store_list[0];
                }

                else
                {
                    current_store.StoreNumber = 158;
                    current_store.StoreName = "Grand Rapids";
                }
            }

            StoreNumberField.Text = "You are currently connected to store " + current_store.StoreNumber;
            StoreNameField.Text = current_store.StoreName;
        }

        private void StoreSelectionButton_Clicked(object sender, EventArgs e)
        {
            StoreSelectionDetailed store_selection_page = new StoreSelectionDetailed();
            
            this.Navigation.PushModalAsync(store_selection_page);
        }
    }
}
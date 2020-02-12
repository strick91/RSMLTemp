﻿using RSMLTemp.TabbedPages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RSMLTemp
{
    public partial class App : Application
    {
        public static string FilePath;
        public App(string filePath)
        {
            InitializeComponent();

            MainPage = new UnresolvedMain();
            FilePath = filePath;
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

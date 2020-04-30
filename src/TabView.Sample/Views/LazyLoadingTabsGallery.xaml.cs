using System;
using Xamarin.Forms;
using ScrolledEventArgs = Xamarin.Forms.TabView.ScrolledEventArgs;

namespace TabView.Sample.Views
{
    public partial class LazyLoadingTabsGallery : ContentPage
    {
        public LazyLoadingTabsGallery()
        {
            InitializeComponent();
            UpdateInfo();
        }

        void OnTabViewScrolled(object sender, ScrolledEventArgs e)
        {
            UpdateInfo();
        }

        void OnLazyCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            TabView.IsLazy = e.Value;
            UpdateInfo();
        }

        void UpdateInfo()
        {
            InfoLabel.Text = string.Empty;

            int index = 0;

            foreach(var tabItem in TabView.TabItems)
            {
                if(tabItem.CurrentContent == null)
                    InfoLabel.Text += $"{index} - {tabItem.Text} is null {Environment.NewLine}";
                else
                    InfoLabel.Text += $"{index} - {tabItem.Text} have Content {Environment.NewLine}";

                index++;
            }
        }
    }
}
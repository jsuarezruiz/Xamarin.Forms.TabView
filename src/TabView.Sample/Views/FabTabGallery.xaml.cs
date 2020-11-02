using System;
using Xamarin.Forms;
using Xamarin.Forms.TabView;

namespace TabView.Sample.Views
{
    public partial class FabTabGallery : ContentPage
    {
        public FabTabGallery()
        {
            InitializeComponent();
        }

        void OnFabTabTapped(object sender, TabTappedEventArgs e)
        {
            DisplayAlert("FabTabGallery", "Tab Tapped.", "Ok");
        }
    }
}
using System;
using Xamarin.Forms;
using Xamarin.Forms.TabView;

namespace TabView.Sample.Views
{
    public partial class IsTabStripVisibleGallery : ContentPage
    {
        public IsTabStripVisibleGallery()
        {
            InitializeComponent();
        }

        void OnChangeTabStripPlacementClicked(object sender, EventArgs e)
        {
            if (TabView.TabStripPlacement == TabStripPlacement.Bottom)
                TabView.TabStripPlacement = TabStripPlacement.Top;
            else
                TabView.TabStripPlacement = TabStripPlacement.Bottom;
        }
    }
}
using System;
using Xamarin.Forms;
using Xamarin.Forms.TabView;
using ScrolledEventArgs = Xamarin.Forms.TabView.ScrolledEventArgs;

namespace TabView.Sample.Views
{
    public partial class TabEventsGallery : ContentPage
    {
        public TabEventsGallery()
        {
            InitializeComponent();
        }

        void OnTabViewSelectionChanged(object sender, TabSelectionChangedEventArgs e)
        {
            InfoEventsLabel.Text += $"SelectionChanged - OldPosition: {e.OldPosition}, NewPosition: {e.NewPosition} {Environment.NewLine}";
        }

        void OnTabViewItemTapTapped(object sender, TabTappedEventArgs e)
        {
            InfoEventsLabel.Text += $"TabTapped - Position: {e.Position} {Environment.NewLine}";
        }

        void OnTabViewScrolled(object sender, ScrolledEventArgs e)
        {
            InfoEventsLabel.Text += $"Scrolled - Offset: {e.Offset} {Environment.NewLine}";
        }
    }
}
using Xamarin.Forms;
using Xamarin.Forms.TabView;
using Tabs = Xamarin.Forms.TabView.TabView;

namespace TabView.Sample.Views
{
    public class BasicCSharpTabsGallery : ContentPage
    {
        public BasicCSharpTabsGallery()
        {
            Title = "Basic Tabs Gallery (C#)";

            var layout = new Grid();

            var tabView = new Tabs
            {
                TabStripPlacement = TabStripPlacement.Bottom,
                TabStripBackgroundColor = Color.Blue,
                TabStripHeight = 60,
                TabIndicatorColor = Color.Yellow,
                TabContentBackgroundColor = Color.Yellow
            };

            var tabViewItem1 = new TabViewItem
            {
                Icon = "triangle.png",
                Text = "Tab 1",
                TextColor = Color.White,
                TextColorSelected = Color.Yellow,
            };

            var tabViewItem1Content = new Grid
            {
                BackgroundColor = Color.Gray
            };

            var label1 = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "TabContent1"
            };

            tabViewItem1Content.Children.Add(label1);

            tabViewItem1.Content = tabViewItem1Content;

            tabView.TabItems.Add(tabViewItem1);

            var tabViewItem2 = new TabViewItem
            {
                Icon = "circle.png",
                Text = "Tab 2",
                TextColor = Color.White,
                TextColorSelected = Color.Yellow
            };

            var tabViewItem2Content = new Grid
            {
                BackgroundColor = Color.OrangeRed
            };

            var label2 = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "TabContent2"
            };

            tabViewItem2Content.Children.Add(label2);

            tabViewItem2.Content = tabViewItem2Content;

            tabView.TabItems.Add(tabViewItem2);

            layout.Children.Add(tabView);

            Content = layout;
        }
    }
}
using System;
using Xamarin.Forms;

namespace TabView.Sample.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();
        }

        void OnFabTabBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FabTabGallery());
        }

        void OnBasicTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BasicTabsGallery());
        }

        void OnCustomTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustomTabsGallery());
        }

        void OnLazyTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LazyLoadingTabsGallery());
        }

        void OnTabItemsSourceBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabItemsSourceGallery());
        }

        void OnNestedTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NestedTabsGallery());
        }

        void OnScrollTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ScrollTabsGallery());
        }

        void OnSegmentControlBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SegmentTabsGallery());
        }

        void OnTabEventsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabEventsGallery());
        }

        void OnTabPlacementBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabPlacementGallery());
        }

        void OnTabFlowDirectionBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabFlowDirectionGallery());
        }

        void OnTabBadgeBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabBadgeGallery());
        }

        void OnTabIndicatorViewBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabIndicatorViewGallery());
        }

        void OnTabStripBackgroundViewBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabStripBackgroundViewGallery());
        }
    }
}

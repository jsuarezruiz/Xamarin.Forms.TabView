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

        void OnBasicCSharpTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BasicCSharpTabsGallery());
        }

        void OnlyTextTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new OnlyTextTabsGallery());
        }

        void OnCustomTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustomTabsGallery());
        }

        void OnTabItemsSourceBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabItemsSourceGallery());
        }

        void OnUpdateTabItemsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new UpdateTabItemsGallery());
        }

        void OnTabViewItemAnimationBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabViewItemAnimationGallery());
        }

        void OnTabWidthBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabWidthGallery());
        }

        void OnIsTabStripVisibleBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new IsTabStripVisibleGallery());
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

        void OnNoContentBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new NoContentGallery());
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

        void OnPerfBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PerformanceTestGallery());
        }
    }
}

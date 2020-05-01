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

        void OnCarouselViewBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CarouselViewGallery());
        }

        void OnFabTabBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FabTabGallery());
        }

        void OnBasicTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BasicTabsGallery());
        }

        void OnlyTextTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new OnlyTextTabsGallery());
        }

        void OnCustomTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustomTabsGallery());
        }

        void OnCyclicalTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CyclicalTabsGallery());
        }

        void OnLazyTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LazyLoadingTabsGallery());
        }

        void OnTabItemsSourceBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TabItemsSourceGallery());
        }

        void OnCustomTransitionTabsBtnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CustomTransitionTabsGallery());
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

using System;
using Xamarin.Forms;

namespace TabView.Sample.Views
{
    public partial class CarouselViewGallery : ContentPage
    {
        public CarouselViewGallery()
        {
            InitializeComponent();
        }

        void OnScrollToZeroClicked(object sender, EventArgs e)
        {
            bool isAnimated = IsAnimatedScrollToCheckBox.IsChecked;
            CarouselView.ScrollTo(0, isAnimated);
        }

        void OnScrollToFiveClicked(object sender, EventArgs e)
        {
            bool isAnimated = IsAnimatedScrollToCheckBox.IsChecked;
            CarouselView.ScrollTo(5, isAnimated);
        }
    }
}

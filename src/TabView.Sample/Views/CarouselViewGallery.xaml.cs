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

        async void OnScrollToZeroClicked(object sender, EventArgs e)
        {
            bool isAnimated = IsAnimatedScrollToCheckBox.IsChecked;
            await CarouselView.ScrollToAsync(0, isAnimated);
        }

        async void OnScrollToFiveClicked(object sender, EventArgs e)
        {
            bool isAnimated = IsAnimatedScrollToCheckBox.IsChecked;
            await CarouselView.ScrollToAsync(5, isAnimated);
        }
    }
}

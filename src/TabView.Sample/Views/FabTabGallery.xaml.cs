using Xamarin.Forms;

namespace TabView.Sample.Views
{
    public partial class FabTabGallery : ContentPage
    {
        public FabTabGallery()
        {
            InitializeComponent();

            fabTabButton.TabTapped += (s, e) =>
                DisplayAlert("FAB Tapped", "You tapped the FAB tab, notice how the content didn't change!", "OK"); ;
        }
    }
}
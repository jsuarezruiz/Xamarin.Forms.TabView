using System;
using Xamarin.Forms;

namespace TabView.Sample.Views
{
    public partial class TabBadgeGallery : ContentPage
    {
        int _counter;

        public TabBadgeGallery()
        {
            InitializeComponent();

            BindingContext = this;

            Counter = 2;
        }

        public int Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnPropertyChanged();
            }
        }

        void OnIncreaseClicked(object sender, EventArgs e)
        {
            Counter++;
        }

        void OnDecreaseClicked(object sender, EventArgs e)
        {
            if (Counter == 0)
                return;

            Counter--;
        }
    }
}
using System;
using Xamarin.Forms;

namespace TabView.Sample.Views
{
    public partial class TabFlowDirectionGallery : ContentPage
    {
        public TabFlowDirectionGallery()
        {
            InitializeComponent();
        }

        void OnChangeFlowDirectionClicked(object sender, EventArgs e)
        {
            if (TabView.FlowDirection == FlowDirection.LeftToRight)
            {
                TabView.FlowDirection = FlowDirection.RightToLeft;
                InfoLabel.Text = "FlowDirection is RightToLeft";
            }
            else
            {
                TabView.FlowDirection = FlowDirection.LeftToRight;
                InfoLabel.Text = "FlowDirection is LeftToRight";
            }
        }
    }
}
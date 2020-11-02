using Xamarin.Forms.Internals;

namespace Xamarin.Forms.TabView
{
    public class BadgeTemplate : Grid
    {
        public BadgeTemplate()
        {
            BatchBegin();

            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;

            var badgeBorder = new Frame
            {
                HasShadow = false,
                IsClippedToBounds = false,
                Padding = 2,
                Margin = 6
            };

            Label badgeText = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            badgeBorder.SetBinding(BackgroundColorProperty, new Binding("BackgroundColor", source: RelativeBindingSource.TemplatedParent));
            badgeBorder.SetBinding(Frame.BorderColorProperty, new Binding("BorderColor", source: RelativeBindingSource.TemplatedParent));

            badgeText.BatchBegin();

            badgeText.SetBinding(Label.TextProperty, new Binding("Text", source: RelativeBindingSource.TemplatedParent));
            badgeText.SetBinding(Label.TextColorProperty, new Binding("TextColor", source: RelativeBindingSource.TemplatedParent));

            badgeBorder.Content = badgeText;

            Children.Add(badgeBorder);

            badgeText.BatchCommit();

            BatchCommit();

            INameScope nameScope = new NameScope();
            NameScope.SetNameScope(this, nameScope);
            nameScope.RegisterName(BadgeView.ElementBorder, badgeBorder);
            nameScope.RegisterName(BadgeView.ElementText, badgeText);
        }
    }
}
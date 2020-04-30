using System;

namespace Xamarin.Forms.TabView
{
    public class WindowsTabViewItemTemplate : Grid
    {
        readonly Image _icon;
        readonly Label _text;
        readonly BadgeView _badge;

        public WindowsTabViewItemTemplate()
        {
            RowSpacing = 0;

            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;

            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            _icon = new Image
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 6, 0, 0)
            };

            _text = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 6)
            };

            _badge = new BadgeView
            {
                HeightRequest = 20,
                WidthRequest = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start
            };

            Children.Add(_icon);
            Children.Add(_text);
            Children.Add(_badge);

            SetRow(_icon, 0);
            SetRow(_text, 1);
            SetRow(_badge, 0);
            SetRowSpan(_badge, 2);
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            BindingContext = Parent;

            _icon.SetBinding(Image.SourceProperty, "CurrentIcon");

            _text.SetBinding(Label.TextProperty, "Text");
            _text.SetBinding(Label.TextColorProperty, "CurrentTextColor");
            _text.SetBinding(Label.FontSizeProperty, "CurrentFontSize");
            _text.SetBinding(Label.FontAttributesProperty, "CurrentFontAttributes");

            _badge.SetBinding(BadgeView.BackgroundColorProperty, "CurrentBadgeBackgroundColor");
            _badge.SetBinding(BadgeView.TextProperty, "BadgeText");
            _badge.SetBinding(BadgeView.TextColorProperty, "BadgeTextColor");
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            UpdateLayout();
            UpdateBadgePosition();
        }

        void UpdateLayout()
        {
            if (!(BindingContext is TabViewItem tabViewItem))
                return;

            if (tabViewItem.CurrentIcon == null)
            {
                SetRow(_text, 0);
                SetRowSpan(_text, 2);
            }
            else
            {
                SetRow(_text, 1);
                SetRowSpan(_text, 1);
            }
        }

        void UpdateBadgePosition()
        {
            if (_badge == null)
                return;

            var translationX = Math.Max(_icon.Width, _text.Width);

            if (translationX > 0 && _badge.TranslationX == 0)
                _badge.TranslationX = translationX / 2;

            if (_badge.TranslationY == 0)
                _badge.TranslationY = 2;
        }
    }
}
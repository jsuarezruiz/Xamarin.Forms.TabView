namespace Xamarin.Forms
{
    public class MaterialTabViewItemTemplate : Grid
    { 
        readonly Image _icon;
        readonly Label _text;
        readonly BadgeView _badge;

        public MaterialTabViewItemTemplate()
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
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 6)
            };

            _badge = new BadgeView
            {
                HeightRequest = 24,
                WidthRequest = 24,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                Scale = 0.75,
                TranslationX = 12
            };

            Children.Add(_icon);
            Children.Add(_text);
            Children.Add(_badge);

            SetRow(_icon, 0);
            SetRow(_text, 1);
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
    }
}
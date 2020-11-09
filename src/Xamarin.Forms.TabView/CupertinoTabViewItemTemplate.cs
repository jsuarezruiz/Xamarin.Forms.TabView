namespace Xamarin.Forms.TabView
{
    public class CupertinoTabViewItemTemplate : Grid
    {
        readonly VisualFeedbackEffect _visualFeedback;

        readonly Image _icon;
        readonly Label _text;
        readonly BadgeView _badge;

        public CupertinoTabViewItemTemplate()
        {
            _visualFeedback = new VisualFeedbackEffect();
            Effects.Add(_visualFeedback);

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
                Margin = new Thickness(0, 6)
            };

            _text = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 6)
            };

            _badge = new BadgeView
            {
                PlacementTarget = _icon,
                Margin = new Thickness(0)
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
            _badge.SetBinding(BadgeView.BorderColorProperty, "CurrentBadgeBorderColor");
            _badge.SetBinding(BadgeView.TextProperty, "BadgeText");
            _badge.SetBinding(BadgeView.TextColorProperty, "BadgeTextColor");

            VisualFeedbackEffect.SetFeedbackColor(this, Color.White);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            UpdateLayout();
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
    }
}
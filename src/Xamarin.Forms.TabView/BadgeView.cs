namespace Xamarin.Forms
{
    public class BadgeView : ContentView
    {
        Grid _badgeContainer;
        BoxView _badgeShape;
        Label _badgeText;

        public BadgeView()
        {
            Initialize();
        }

        public static BindableProperty AutoHideProperty =
            BindableProperty.Create(nameof(AutoHide), typeof(bool), typeof(BadgeView), defaultValue: true,
                propertyChanged: OnAutoHideChanged);

        public bool AutoHide
        {
            get { return (bool)GetValue(AutoHideProperty); }
            set { SetValue(AutoHideProperty, value); }
        }

        static void OnAutoHideChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is BadgeView badgeView)
            {
                badgeView.UpdateVisibility();
            }
        }

        public new static BindableProperty BackgroundColorProperty =
            BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(BadgeView), defaultValue: Color.Default,
                propertyChanged: OnBackgroundColorChanged);

        public new Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        static void OnBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is BadgeView badgeView)
            {
                badgeView.UpdateBackgroundColor((Color)newValue);
            }
        }

        public static BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(BadgeView), defaultValue: Color.Default,
                propertyChanged: OnTextColorChanged);

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is BadgeView badgeView)
            {
                badgeView.UpdateTextColor((Color)newValue);
            }
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(BadgeView), defaultValue: "0",
                propertyChanged: OnTextChanged);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is BadgeView badgeView)
            {
                badgeView.UpdateText((string)newValue);
            }
        }

        void Initialize()
        {
            _badgeShape = new BoxView
            {
                BackgroundColor = BackgroundColor,
                CornerRadius = 60
            };

            _badgeText = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = TextColor,
                Text = Text
            };

            _badgeContainer = new Grid
            {
                Children = { _badgeShape, _badgeText }
            };

            Content = _badgeContainer;
        }

        void UpdateBackgroundColor(Color backgroundColor)
        {
            _badgeShape.BackgroundColor = backgroundColor;
        }

        void UpdateTextColor(Color textColor)
        {
            _badgeText.TextColor = textColor;
        }

        void UpdateText(string text)
        {
            _badgeText.Text = text;
            UpdateVisibility();
        }

        void UpdateVisibility()
        {
            IsVisible = !AutoHide ||
                (!string.IsNullOrWhiteSpace(_badgeText.Text) && !_badgeText.Text.Trim().Equals("0"));
        }
    }
}
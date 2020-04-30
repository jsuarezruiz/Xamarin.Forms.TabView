using System.Threading.Tasks;

namespace Xamarin.Forms.TabView
{
    public class BadgeView : ContentView
    {
        Grid _badgeContainer;
        BoxView _badgeShape;
        Label _badgeText;
        bool _isVisible;

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

        static async void OnAutoHideChanged(BindableObject bindable, object oldValue, object newValue)
        {
            await (bindable as BadgeView)?.UpdateVisibilityAsync();
        }

        public static BindableProperty IsAnimatedProperty =
           BindableProperty.Create(nameof(IsAnimated), typeof(bool), typeof(BadgeView), defaultValue: true);

        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        public static BindableProperty BadgeAnimationProperty =
            BindableProperty.Create(nameof(BadgeAnimation), typeof(IBadgeAnimation), typeof(BadgeView), new BadgeAnimation());

        public IBadgeAnimation BadgeAnimation
        {
            get { return (IBadgeAnimation)GetValue(BadgeAnimationProperty); }
            set { SetValue(BadgeAnimationProperty, value); }
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
            (bindable as BadgeView)?.UpdateBackgroundColor((Color)newValue);
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
            (bindable as BadgeView)?.UpdateTextColor((Color)newValue);
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
            (bindable as BadgeView)?.UpdateText((string)newValue);
        }

        void Initialize()
        {
            _badgeShape = new BoxView
            {
                BackgroundColor = BackgroundColor,
                CornerRadius = GetCornerRadius()
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

        int GetCornerRadius()
        {
            if (Device.RuntimePlatform == Device.Android)
                return 60; 
            else if (Device.RuntimePlatform == Device.UWP)
                return 24;

            return 12;
        }

        void UpdateBackgroundColor(Color backgroundColor)
        {
            _badgeShape.BackgroundColor = backgroundColor;
        }

        void UpdateTextColor(Color textColor)
        {
            _badgeText.TextColor = textColor;
        }

        async void UpdateText(string text)
        {
            _badgeText.Text = text;
            await UpdateVisibilityAsync();
        }

        async Task UpdateVisibilityAsync()
        {
            string badgeText = _badgeText.Text;

            if (string.IsNullOrEmpty(badgeText))
            {
                IsVisible = false;
                return;
            }

            bool badgeIsVisible = !AutoHide || !badgeText.Trim().Equals("0");

            if (IsAnimated)
            {
                if (badgeIsVisible == _isVisible)
                    return;

                if (badgeIsVisible)
                {
                    IsVisible = true;
                    await BadgeAnimation.OnAppearing(this);
                }
                else
                {
                    await BadgeAnimation.OnDisappering(this);
                    IsVisible = false;
                }

                _isVisible = badgeIsVisible;
            }
            else
                IsVisible = badgeIsVisible;
        }
    }
}
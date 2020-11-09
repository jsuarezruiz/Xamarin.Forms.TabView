using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Xamarin.Forms.TabView
{
    public class BadgeView : TemplatedView
    {
        internal const string ElementBorder = "PART_Border";
        internal const string ElementText = "PART_Text";

        Frame _badgeBorder;
        Label _badgeText;
        bool _isVisible;

        public BadgeView()
        {
            ControlTemplate = new ControlTemplate(typeof(BadgeTemplate));
        }

        public static BindableProperty PlacementTargetProperty =
            BindableProperty.Create(nameof(PlacementTarget), typeof(View), typeof(BadgeView), null);

        public View PlacementTarget
        {
            get { return (View)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
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

        public static BindableProperty BorderColorProperty =
          BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(BadgeView), defaultValue: Color.Default,
              propertyChanged: OnBorderColorChanged);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as BadgeView)?.UpdateBorderColor((Color)newValue);
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

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _badgeBorder = GetTemplateChild(ElementBorder) as Frame;
            _badgeText = GetTemplateChild(ElementText) as Label;

            UpdateSize();
            UpdatePosition();
            UpdateIsEnabled();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEnabledProperty.PropertyName)
                UpdateIsEnabled();
        }

        void UpdateIsEnabled()
        {
            if (IsEnabled)
                _badgeText.PropertyChanged += OnBadgeTextPropertyChanged;
            else
                _badgeText.PropertyChanged -= OnBadgeTextPropertyChanged;
        }

        void OnBadgeTextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Height):
                case nameof(Width):
                    UpdateSize();
                    UpdatePosition();
                    break;
            }
        }

        void UpdateSize()
        {
            if (_badgeText == null || _badgeText.Width <= 0 || _badgeText.Height <= 0)
                return;

            double badgeTextHeight = _badgeText.Height + _badgeBorder.Padding.VerticalThickness / 2;
            double badgeTextWidth = Math.Max(_badgeText.Width + _badgeBorder.Padding.HorizontalThickness / 2, badgeTextHeight);

            _badgeBorder.HeightRequest = badgeTextHeight;
            _badgeBorder.WidthRequest = badgeTextWidth;

            _badgeBorder.CornerRadius = (int)Math.Round(badgeTextHeight / 2);
        }

        void UpdatePosition()
        {
            if (PlacementTarget == null)
                return;

            var x = PlacementTarget.X - PlacementTarget.Margin.HorizontalThickness;

            if (Device.RuntimePlatform != Device.Android)
                x += PlacementTarget.Width;

            _badgeBorder.Margin = new Thickness(x, 0, 0, 0);
        }

        void UpdateBackgroundColor(Color backgroundColor)
        {
            _badgeBorder.BackgroundColor = backgroundColor;
        }

        void UpdateBorderColor(Color borderColor)
        {
            _badgeBorder.BorderColor = borderColor;
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
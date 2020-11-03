namespace Xamarin.Forms.TabView
{
    public class VisualFeedbackEffect : RoutingEffect
    {
        public VisualFeedbackEffect() : base($"TabView.{nameof(VisualFeedbackEffect)}")
        {

        }

        public static readonly BindableProperty FeedbackColorProperty =
            BindableProperty.CreateAttached("FeedbackColor", typeof(Color), typeof(VisualFeedbackEffect), Color.Default);

        public static Color GetFeedbackColor(BindableObject view)
        {
            return (Color)view.GetValue(FeedbackColorProperty);
        }

        public static void SetFeedbackColor(BindableObject view, Color value)
        {
            view.SetValue(FeedbackColorProperty, value);
        }

        public static bool IsFeedbackColorSet(BindableObject element)
        {
            return GetFeedbackColor(element) != Color.Default;
        }
    }
}
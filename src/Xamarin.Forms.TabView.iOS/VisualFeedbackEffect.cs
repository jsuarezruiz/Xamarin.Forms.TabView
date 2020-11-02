using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.TabView.iOS;

[assembly: ResolutionGroupName("TabView")]
[assembly: ExportEffect(typeof(IosVisualFeedbackEffect), "VisualFeedbackEffect")]
namespace Xamarin.Forms.TabView.iOS
{
    [Foundation.Preserve(AllMembers = true)]
    public class IosVisualFeedbackEffect : PlatformEffect
    {
        TouchEvents _touchEvents;
        TouchEventsGestureRecognizer _touchRecognizer;
        UIView _view;
        UIView _layer;
        float _alpha;

        protected override void OnAttached()
        {
            _view = Control ?? Container;

            _view.UserInteractionEnabled = true;

            _layer = new UIView
            {
                Alpha = 0,
                Opaque = false,
                UserInteractionEnabled = false
            };
            _view.AddSubview(_layer);

            _layer.TranslatesAutoresizingMaskIntoConstraints = false;

            _layer.TopAnchor.ConstraintEqualTo(_view.TopAnchor).Active = true;
            _layer.LeftAnchor.ConstraintEqualTo(_view.LeftAnchor).Active = true;
            _layer.BottomAnchor.ConstraintEqualTo(_view.BottomAnchor).Active = true;
            _layer.RightAnchor.ConstraintEqualTo(_view.RightAnchor).Active = true;

            _view.BringSubviewToFront(_layer);

            _touchEvents = new TouchEvents();

            _touchRecognizer = new TouchEventsGestureRecognizer(_touchEvents);
            _touchRecognizer.Delegate = new ShouldRecognizeSimultaneouslyRecognizerDelegate();

            _view.AddGestureRecognizer(_touchRecognizer);

            _touchEvents.TouchBegin += OnTouchBegin;
            _touchEvents.TouchEnd += OnTouchEnd;
            _touchEvents.TouchCancel += OnTouchEnd;

            UpdateEffectColor();
        }

        protected override void OnDetached()
        {
            _touchEvents.TouchBegin -= OnTouchBegin;
            _touchEvents.TouchEnd -= OnTouchEnd;
            _touchEvents.TouchCancel -= OnTouchEnd;

            _view.RemoveGestureRecognizer(_touchRecognizer);
            _touchRecognizer.Delegate?.Dispose();
            _touchRecognizer.Delegate = null;
            _touchRecognizer.Dispose();

            _touchEvents = null;
            _touchRecognizer = null;

            _layer.RemoveFromSuperview();
            _layer.Dispose();
            _layer = null;

            _view = null;
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == VisualFeedbackEffect.FeedbackColorProperty.PropertyName)
            {
                UpdateEffectColor();
            }
        }

        void UpdateEffectColor()
        {
            var color = VisualFeedbackEffect.GetFeedbackColor(Element);
            _alpha = color.A < 1.0f ? 1f : 0.8f;
            _layer.BackgroundColor = color.ToUIColor();
        }

        async void OnTouchBegin(object sender, EventArgs e)
        {
            if (!(Element is VisualElement visualElement) || !visualElement.IsEnabled) return;

            _view.BecomeFirstResponder();

            await UIView.AnimateAsync(0.5, () =>
            {
                _layer.Alpha = _alpha;
            });
        }

        async void OnTouchEnd(object sender, EventArgs e)
        {
            if (!(Element is VisualElement visualElement) || !visualElement.IsEnabled) return;

            await UIView.AnimateAsync(0.5, () =>
            {
                _layer.Alpha = 0;
            });
        }
    }

    public class ShouldRecognizeSimultaneouslyRecognizerDelegate : UIGestureRecognizerDelegate
    {
        public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer) => true;
    }
}
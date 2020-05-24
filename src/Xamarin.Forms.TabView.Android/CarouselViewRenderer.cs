using System;
using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using CarouselView = Xamarin.Forms.TabView.CarouselView;
using CarouselViewRenderer = Xamarin.Forms.TabView.Android.CarouselViewRenderer;

[assembly: ExportRenderer(typeof(CarouselView), typeof(CarouselViewRenderer))]
namespace Xamarin.Forms.TabView.Android
{
    [Preserve(AllMembers = true)]
    public class CarouselViewRenderer : VisualElementRenderer<CarouselView>
    {
        readonly float _density;
        bool _isSwiping;
        float? _downX;
        float? _downY;

        public CarouselViewRenderer(Context context) : base(context)
        {
            _density = Resources.DisplayMetrics.Density;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (!Element.IsSwipeEnabled || !Element.IsEnabled)
            {
                base.OnInterceptTouchEvent(ev);
                return false;
            }

            if (ev.ActionMasked == MotionEventActions.Move)
            {
                float moveX = ev.GetX() / _density;
                float moveY = ev.GetY() / _density;

                float totalX = moveX - _downX.GetValueOrDefault();
                float totalY = moveY - _downY.GetValueOrDefault();

                return ShouldInterceptTouchEvent(totalX, totalY);
            }

            HandleTouchDown(ev);
            HandleTouchUpOrCancel(ev);

            return false;
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (ev.ActionMasked == MotionEventActions.Move)
                HandleTouchMove(ev);

            HandleTouchDown(ev);
            HandleTouchUpOrCancel(ev);

            return true;
        }

        bool ShouldInterceptTouchEvent(float xDelta, float yDelta)
        {
            var xDeltaAbs = Math.Abs(xDelta);
            var yDeltaAbs = Math.Abs(yDelta);

            var isHandled = yDeltaAbs < xDeltaAbs;

            Parent?.RequestDisallowInterceptTouchEvent(isHandled);

            return isHandled;
        }

        void HandleTouchDown(MotionEvent ev)
        {
            if (ev.ActionMasked != MotionEventActions.Down)
                return;

            _downX = ev.GetX() / _density;
            _downY = ev.GetY() / _density;

            Element.TouchStarted();

            _isSwiping = true;
        }

        void HandleTouchMove(MotionEvent ev)
        {
            float moveX = ev.GetX() / _density;
            float moveY = ev.GetY() / _density;

            float xDelta = moveX - _downX.GetValueOrDefault();
            float yDelta = moveY - _downY.GetValueOrDefault();

            ShouldInterceptTouchEvent(xDelta, yDelta);

            if (Math.Abs(yDelta) <= Math.Abs(xDelta))
                Element.TouchChanged(xDelta);
        }

        void HandleTouchUpOrCancel(MotionEvent ev)
        {
            var action = ev.ActionMasked;
            var isUpAction = action == MotionEventActions.Up;
            var isCancelAction = action == MotionEventActions.Cancel;

            if (!_isSwiping || (!isUpAction && !isCancelAction))
                return;

            Element.TouchEnded();

            _isSwiping = false;

            Parent?.RequestDisallowInterceptTouchEvent(false);

            _downX = null;
            _downY = null;
        }
    }
}
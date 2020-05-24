using System;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using CarouselView = Xamarin.Forms.TabView.CarouselView;
using CarouselViewRenderer = Xamarin.Forms.TabView.iOS.CarouselViewRenderer;

[assembly: ExportRenderer(typeof(CarouselView), typeof(CarouselViewRenderer))]
namespace Xamarin.Forms.TabView.iOS
{
	[Preserve(AllMembers = true)]
	public class CarouselViewRenderer : VisualElementRenderer<CarouselView>
	{
        readonly UIPanGestureRecognizer _panGestureRecognizer;
		CGPoint _initialPoint;
		bool _isSwiping;

		public CarouselViewRenderer()
		{
			_panGestureRecognizer = new UIPanGestureRecognizer(HandlePan)
			{
				CancelsTouchesInView = false,
				DelaysTouchesBegan = false,
				DelaysTouchesEnded = false
			};

			_panGestureRecognizer.ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true;
		}

		public static void Preserve() => CarouselView.Preserve();

		protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
				UpdatePanGesture();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CarouselView.IsSwipeEnabledProperty.PropertyName)
				UpdatePanGesture();
		}

		void UpdatePanGesture()
		{
			RemoveGestureRecognizer(_panGestureRecognizer);

			if (!Element.IsEnabled || !Element.IsSwipeEnabled)
				return;

			AddGestureRecognizer(_panGestureRecognizer);
		}

		void HandlePan(UIPanGestureRecognizer panGestureRecognizer)
		{
			if (panGestureRecognizer != null)
			{
				var point = panGestureRecognizer.LocationInView(this);
				var navigationController = GetUINavigationController(GetViewController());

				switch (panGestureRecognizer.State)
				{
					case UIGestureRecognizerState.Began:
						if (navigationController != null)
							navigationController.InteractivePopGestureRecognizer.Enabled = false;

						HandleTouchDown(point);
						break;
					case UIGestureRecognizerState.Changed:
						HandleTouchMove(point);
						break;
					case UIGestureRecognizerState.Ended:
					case UIGestureRecognizerState.Cancelled:
						if (navigationController != null)
							navigationController.InteractivePopGestureRecognizer.Enabled = true;

						HandleTouchUpOrCancel();
						break;
				}
			}
		}

		void HandleTouchDown(CGPoint point)
		{
			_initialPoint = point;
			_isSwiping = true;
			Element.TouchStarted();
		}

		void HandleTouchMove(CGPoint point)
		{
			nfloat xDelta = point.X - _initialPoint.X;
			nfloat yDelta = point.Y - _initialPoint.Y;

			if (Math.Abs(yDelta) <= Math.Abs(xDelta))
				Element.TouchChanged(xDelta);
		}

		void HandleTouchUpOrCancel()
		{
			if (!_isSwiping)
				return;

			_isSwiping = false;
			Element.TouchEnded();
		}

		UIViewController GetViewController()
		{
			var window = UIApplication.SharedApplication.GetKeyWindow();
			var viewController = window.RootViewController;

			while (viewController.PresentedViewController != null)
				viewController = viewController.PresentedViewController;

			return viewController;
		}

		UINavigationController GetUINavigationController(UIViewController controller)
		{
			if (controller != null)
			{
				if (controller is UINavigationController)
				{
					return controller as UINavigationController;
				}

				if (controller.ChildViewControllers.Any())
				{
					var childs = controller.ChildViewControllers.Count();

					for (int i = 0; i < childs; i++)
					{
						var child = GetUINavigationController(controller.ChildViewControllers[i]);

						if (child is UINavigationController)
						{
							return (child as UINavigationController);
						}
					}
				}
			}

			return null;
		}
	}
}
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.TabView;

namespace TabView.Sample.Views
{
    public partial class CustomTransitionTabsGallery : ContentPage
    {
        public CustomTransitionTabsGallery()
        {
            InitializeComponent();
        }
    }

    public class CustomTabTransition : IItemTransition
    {
        protected uint AnimationLength { get; } = 500;
        protected Easing AnimationEasing { get; } = Easing.SinInOut;

        public Task OnSelectionChanging(TransitionSelectionChangingArgs args)
        {
            if (args.CurrentView != null && Math.Abs(args.Offset) < args.Parent.Height)
                args.CurrentView.TranslationY = args.Offset;

            var nextTabTranslationY = Math.Sign((int)args.Direction) * args.Parent.Height + args.Offset;

            if (args.NextView != null && Math.Abs(nextTabTranslationY) < args.Parent.Height)
                args.NextView.TranslationY = nextTabTranslationY;

            return Task.FromResult(true);
        }

        public Task OnSelectionChanged(TransitionSelectionChangedArgs args)
        {
            if (args.Status == TransitionSelectionChanged.Reset)
            {
                var tcs = new TaskCompletionSource<bool>();

                Device.BeginInvokeOnMainThread(() =>
                {
                    Animation resetAnimation = new Animation();

                    var animationPercentLength = AnimationLength;

                    if (args.CurrentView != null)
                        resetAnimation.Add(0, 1, new Animation(v => args.CurrentView.TranslationY = v, args.CurrentView.TranslationY, 0));

                    if (args.NextView != null)
                    {
                        resetAnimation.Add(0, 1, new Animation(v => args.NextView.TranslationY = v, args.NextView.TranslationY, Math.Sign((int)args.Direction) * args.Parent.Height));
                        animationPercentLength = (uint)(AnimationLength * (args.Parent.Height - Math.Abs(args.NextView.TranslationY)) / args.Parent.Height);
                    }

                    resetAnimation.Commit(args.Parent, nameof(OnSelectionChanged), length: animationPercentLength, easing: AnimationEasing,
                        finished: (v, t) => tcs.SetResult(true));
                });

                return tcs.Task;
            }

            if (args.Status == TransitionSelectionChanged.Completed)
            {
                var tcs = new TaskCompletionSource<bool>();

                Device.BeginInvokeOnMainThread(() =>
                {
                    Animation completeAnimation = new Animation();

                    var animationPercentLength = AnimationLength;

                    if (args.CurrentView != null)
                    {
                        completeAnimation.Add(0, 1, new Animation(v => args.CurrentView.TranslationY = v, args.CurrentView.TranslationY, -Math.Sign((int)args.Direction) * args.Parent.Height));
                        animationPercentLength = (uint)(AnimationLength * (args.Parent.Height - Math.Abs(args.CurrentView.TranslationY)) / args.Parent.Height);
                    }

                    if (args.NextView != null)
                        completeAnimation.Add(0, 1, new Animation(v => args.NextView.TranslationY = v, args.NextView.TranslationY, 0));

                    completeAnimation.Commit(args.Parent, nameof(OnSelectionChanged), length: animationPercentLength, easing: AnimationEasing,
                        finished: (v, t) => tcs.SetResult(true));
                });

                return tcs.Task;
            }

            return Task.FromResult(true);
        }
    }
}
namespace Xamarin.Forms.TabView
{
    public interface ICarouselViewController
    {
        void TouchStarted();
        void TouchChanged(double offset);
        void TouchEnded();
    }
}
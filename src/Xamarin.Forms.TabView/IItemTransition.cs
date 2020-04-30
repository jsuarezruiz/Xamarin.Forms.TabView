using System.Threading.Tasks;

namespace Xamarin.Forms.TabView
{
    public interface IItemTransition
    {
        Task OnSelectionChanging(TransitionSelectionChangingArgs args);
        Task OnSelectionChanged(TransitionSelectionChangedArgs args);
    }

    public class TransitionSelectionChangingArgs
    {
        public View Parent { get; set; }
        public View CurrentView { get; set; }
        public View NextView { get; set; }
        public double Offset { get; set; }
        public ScrollDirection Direction { get; set; }
    }

    public enum TransitionSelectionChanged
    {
        Completed,
        Reset
    }

    public class TransitionSelectionChangedArgs
    {
        public View Parent { get; set; }
        public View CurrentView { get; set; }
        public View NextView { get; set; }
        public ScrollDirection Direction { get; set; }
        public TransitionSelectionChanged Status { get; set; }
    }
}
using System.Threading.Tasks;

namespace Xamarin.Forms.TabView
{
    public interface ITabViewItemAnimation
    {
        Task OnSelected(View tabViewItem);
        Task OnDeSelected(View tabViewItem);
    }
}
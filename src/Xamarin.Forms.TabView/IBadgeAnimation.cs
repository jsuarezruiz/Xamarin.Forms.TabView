using System.Threading.Tasks;

namespace Xamarin.Forms.TabView
{
    public interface IBadgeAnimation
    {
        Task OnAppearing(BadgeView badgeView);
        Task OnDisappering(BadgeView badgeView);
    }
}
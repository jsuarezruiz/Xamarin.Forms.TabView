using System.Collections.Generic;

namespace Xamarin.Forms.TabView
{
    public static class LayoutExtensions
    {
        public static IReadOnlyList<Element> GetChildren(this ILayoutController source)
        {
            return source.Children;
        }
    }
}
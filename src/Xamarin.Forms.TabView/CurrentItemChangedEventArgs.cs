using System;

namespace Xamarin.Forms.TabView
{
    public class CurrentItemChangedEventArgs : EventArgs
    {
        public CurrentItemChangedEventArgs(object currentItem)
        {
            CurrentItem = currentItem;
        }

        public object CurrentItem { get; set; }
    }
}
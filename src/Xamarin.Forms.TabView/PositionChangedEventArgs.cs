using System;

namespace Xamarin.Forms.TabView
{
    public class PositionChangedEventArgs : EventArgs
    {
        public PositionChangedEventArgs(int position)
        {
            CurrentPosition = position;
        }

        public int CurrentPosition { get; set; }
    }
}
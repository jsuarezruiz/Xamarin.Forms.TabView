using System;

namespace Xamarin.Forms
{
    public class TabSelectionChangedEventArgs : EventArgs
    {
        public int NewPosition { get; set; }
        public int OldPosition { get; set; }
    }
}
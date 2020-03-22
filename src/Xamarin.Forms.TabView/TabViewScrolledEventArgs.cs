using System;

namespace Xamarin.Forms
{
    public class TabViewScrolledEventArgs : EventArgs
	{
		public double HorizontalDelta { get; set; }

		public double VerticalDelta { get; set; }

		public double HorizontalOffset { get; set; }

		public double VerticalOffset { get; set; }

		public int FirstVisibleTabViewItemIndex { get; set; }

		public int CenterTabViewItemIndex { get; set; }

		public int LastVisibleTabViewItemIndex { get; set; }
	}
}
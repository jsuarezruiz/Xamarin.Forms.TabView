using System;

namespace Xamarin.Forms.TabView.iOS
{
    [Foundation.Preserve(AllMembers = true)]
    public class TouchEvents
    {
        public event EventHandler TouchBegin;

        public event EventHandler TouchMove;

        public event EventHandler TouchEnd;

        public event EventHandler TouchCancel;

        public virtual void OnTouchBegin()
        {
            TouchBegin?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnTouchMove()
        {
            TouchMove?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnTouchEnd()
        {
            TouchEnd?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnTouchCancel()
        {
            TouchCancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
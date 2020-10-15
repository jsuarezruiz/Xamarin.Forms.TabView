using AppKit;
using CoreGraphics;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace TabView.Sample.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        readonly NSWindow _window;

        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var mainFrame = NSScreen.MainScreen.Frame;
            var windowFrame = new CGRect(0, 0, 800, 600);

            var x = (mainFrame.X + mainFrame.Width) / 2 - windowFrame.Width / 2;
            var y = (mainFrame.Y + mainFrame.Height) / 2 - windowFrame.Height / 2;

            var rect = new CGRect(x, y, 800, 600);

            _window = new NSWindow(rect, style, NSBackingStore.Buffered, false)
            {
                Title = "Xamarin.Forms TabView",
                TitleVisibility = NSWindowTitleVisibility.Hidden
            };
        }

        public override NSWindow MainWindow
        {
            get { return _window; }
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Forms.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
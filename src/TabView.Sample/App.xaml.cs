using TabView.Sample.Views;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using FormsApplication = Xamarin.Forms.Application;

namespace TabView.Sample
{
    public partial class App : FormsApplication
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainView()); 
            
            On<Windows>().SetImageDirectory("Assets");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
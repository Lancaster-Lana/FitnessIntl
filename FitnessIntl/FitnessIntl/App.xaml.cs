using System.Linq;
using FitnessIntl.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FitnessIntl
{
    public partial class App : Application
    {
        public const string AppName = "FitnessIntl";

        public static bool IsUserLoggedIn { get; set; }

        public App()
        {
            InitializeComponent();

            SetMainPage(IsUserLoggedIn);
        }

        public static void SetMainPage(bool userLoggedIn)
        {
            if (!userLoggedIn)
            {
                Current.MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                Current.MainPage = new MenuPage();
            }
        }

        public static bool IsLoggedIn
        {
            get { return !string.IsNullOrWhiteSpace(_Token); }
        }

        static string _Token;
        public static string Token
        {
            get { return _Token; }
        }

        public static void SaveToken(string token)
        {
            _Token = token;
        }

        //public static void SetMainPage()
        //{
        //          Current.MainPage = new TabbedPage
        //          {
        //              Children =
        //              {
        //                  new NavigationPage(new GroupedListXaml())
        //                  {
        //                      Title = "Browse",
        //                      Icon = Device.OnPlatform<string>("tab_feed.png",null,null)
        //                  },
        //                  new NavigationPage(new AboutPage())
        //                  {
        //                      Title = "About",
        //                      Icon = Device.OnPlatform<string>("tab_about.png",null,null)
        //                  },
        //              }
        //          };
        //      }
    }
}

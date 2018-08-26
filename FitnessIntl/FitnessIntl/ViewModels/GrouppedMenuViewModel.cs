using FitnessIntl.Helpers;
using System.Collections.ObjectModel;

namespace NavigationMenu.ViewModels
{
    public class GrouppedMenuViewModel : ObservableCollection<MenuItemViewModel>
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
    }

    public class MenuItemViewModel //: ObservableObject
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public bool IsCustomizable { get; set; }

        //public ICommand NavigateDetailsCommand
        //{
        //    get
        //    {
        //        Page page = null;

        //        return new Command(async () =>//(value)
        //        {
        //            var mdp = (Application.Current.MainPage as MasterDetailPage);
        //            var navPage = mdp.Detail as NavigationPage;

        //            // Hide the Master toolbar page
        //            //mdp.IsPresented = false;

        //            switch (Name)
        //            {
        //                case "Dashboard":
        //                    page = new Dashboard();
        //                    break;
        //                case "Food":
        //                    page = new Food();
        //                    break;
        //                default:
        //                    page = new Dashboard();
        //                    break;
        //            }

        //            await navPage.PushAsync(page);

        //        });
        //    }
        //}

        //public MenuItemViewModel()
        //{
        //}
    }
}

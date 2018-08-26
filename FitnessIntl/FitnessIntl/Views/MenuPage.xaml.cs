using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using NavigationMenu.ViewModels;
using RevolutionarySHOP.ViewModels;
using FitnessIntl.ViewModels;
using System.Threading.Tasks;

namespace FitnessIntl.Views
{
    public partial class MenuPage : ContentPage
    {
        public static ObservableCollection<GrouppedMenuViewModel> groupedMenuItems { get; set; }

        AccountController _accountController;

        /// <summary>
        /// Init menu items
        /// </summary>
        static MenuPage()
        {
            groupedMenuItems = new ObservableCollection<GrouppedMenuViewModel>();

            var homeMenuGroup = new GrouppedMenuViewModel() { LongName = "Home", ShortName = "h" };
            var foodMenuGroup = new GrouppedMenuViewModel() { LongName = "Food", ShortName = "f" };
            var exercisesMenuGroup = new GrouppedMenuViewModel() { LongName = "Exercises", ShortName = "e" };
            var measurementsMenuGroup = new GrouppedMenuViewModel() { LongName = "Measurements", ShortName = "m" };
            var journalMenuGroup = new GrouppedMenuViewModel() { LongName = "Public journal", ShortName = "j" };

            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Account Settings", Name = "AccountSettings", Comment = "Your account profile", IsCustomizable = true });
            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Announcements", Name = "Announcements", Comment = "News, events", IsCustomizable = false });
            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Exercise Summary", Name = "ExerciseSummary", Comment = "Exercises results per week, month", IsCustomizable = true });
            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Food Summary", Name = "FoodSummary", Comment = "Food Summary", IsCustomizable = true });
            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Measurement Summary", Name = "MeasurementSummary", Comment = "Measurement body, weight, neck", IsCustomizable = true });

            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Tracking", Name = "Tracking", Comment = "Keep track of the your foods and exercises daily", IsCustomizable = true });
            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Monitor Measurements", Name = "MonitorMeasurements", Comment = "Monitor common measurements", IsCustomizable = false });
            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Share Journal", Name = "ShareJournal", Comment = "Easily share your journal and results with others, IsCustomizable = true" });
            homeMenuGroup.Add(new MenuItemViewModel() { Title = "Support My Space", Name = "SupportMySpace", Comment = "Now supporting MySpace", IsCustomizable = true });

            foodMenuGroup.Add(new MenuItemViewModel() { Title = "Custom Food", Name = "CustomFood", Comment = "Custom Food", IsCustomizable = false });
            foodMenuGroup.Add(new MenuItemViewModel() { Title = "Food log", Name = "FoodLog", Comment = "Log food dishes", IsCustomizable = false });

            exercisesMenuGroup.Add(new MenuItemViewModel() { Title = "Custom Exercises", Name = "CustomExercises", Comment = "Own exercises programs", IsCustomizable = false });
            exercisesMenuGroup.Add(new MenuItemViewModel() { Title = "Exercises log", Name = "ExerciseLog", Comment = "Log daily exercises", IsCustomizable = false });

            //measurementsMenuGroup.Add(new MenuItemViewModel() { Name = "Body Mass Index Calculator", IsCustomizable = true, Comment = "Body mass calculator" });
            //measurementsMenuGroup.Add(new MenuItemViewModel() { Name = "Custom Measurements", IsCustomizable = false, Comment = "Custom Measurement" });
            //measurementsMenuGroup.Add(new MenuItemViewModel() { Name = "MeasurementsLog", IsCustomizable = true, Comment = "Measurements Log" });

            //journalMenuGroup.Add(new MenuItemViewModel() { Name = "PublicJournal", IsCustomizable = true, Comment = "Public journal" });

            //Collect all menus
            groupedMenuItems.Add(homeMenuGroup);
            groupedMenuItems.Add(foodMenuGroup);
            groupedMenuItems.Add(exercisesMenuGroup);
            groupedMenuItems.Add(measurementsMenuGroup);
            groupedMenuItems.Add(journalMenuGroup);
        }

        public MenuPage()
        {
            InitializeComponent();

            _accountController = new AccountController();

            lstView.ItemsSource = groupedMenuItems; //BindingContext = groupedMenuItems; //new MenuViewModel();

            lstView.ItemTapped += async (sender, e) =>
            {
                var selectedMenu = e.Item as MenuItemViewModel;
                if (selectedMenu == null)
                    return;

                var page = GetNavPage(selectedMenu.Name); // new ItemDetailPage(new ItemDetailViewModel(item));

                await Navigation.PushAsync(page);

                //var mdp = (Application.Current.MainPage as MasterDetailPage);
                //var navPage = mdp.Detail as NavigationPage;
                //mdp.IsPresented = false;
                //mdp.Detail = new NavigationPage(new ItemDetailPage(new ItemDetailViewModel(item)));

                //await navPage.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

                //await DisplayAlert("Tapped", item.Title.ToString() + " was selected.", "OK");
                ((ListView)sender).SelectedItem = null;
            };

            //Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
        }

        #region Handlers 

        async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            //await _accountController.LogoutAsync();
            App.IsUserLoggedIn = false;
            Navigation.InsertPageBefore(new LoginPage(), this);
            await Navigation.PopAsync();
        }


        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as MenuItemViewModel;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item
            //ItemsListView.SelectedItem = null;
        }

        #endregion

        private ContentPage GetNavPage(string menuName)
        {
            switch (menuName)
            {
                case "AccountSettings":
                    //Forms.Context.ApplicationContext.use
                    //var currentAcount = _accountController.GetAccount();

                    var currentUser = new AccountSettingsViewModel //TODO: get from session
                    {
                        Email = "curr@gmail.com",
                        Password = "pwd",
                        SecurityQuestion = "Q",
                        SecurityAnswer = "A"
                    };
                    return new AccountSettings(currentUser);

                case "Announcements":
                    var viewModel = new AnnouncementsViewModel();
                    viewModel.List.Add(new AnnouncementViewModel  //TODO: get from DB
                    {
                        Id = 1,
                        CreatedDate = DateTime.Now,
                        Title = "WWCode",
                        Content = "BigData lecture in NTUU"
                    });
                    return new Announcements(viewModel);

                //case "ExerciseSummary":
                //    return new ExerciseSummary();
                //case "FoodSummary":
                //    return new FoodSummary();
                //case "MeasurementSummary":
                //    return new MeasurementSummary();
                default:
                    return new MainDashboard();
            }
        }
    }
}
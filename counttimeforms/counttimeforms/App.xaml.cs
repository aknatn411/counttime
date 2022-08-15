using counttimeforms.Data;
using counttimeforms.Models;
using counttimeforms.Services;
using counttimeforms.ViewModels;
using System;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace counttimeforms
{
    public partial class App : Application
    {
        static CountTimeDatabase database;

        // Create the database connection as a singleton.
        public static CountTimeDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new CountTimeDatabase(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "CountTimeDatabase.db3"));
                }
                return database;
            }
        }

        public static Profile UserProfile;
        public App()
        {
            DevExpress.XamarinForms.Charts.Initializer.Init();
            DevExpress.XamarinForms.CollectionView.Initializer.Init();
            DevExpress.XamarinForms.Scheduler.Initializer.Init();
            DevExpress.XamarinForms.DataGrid.Initializer.Init();
            DevExpress.XamarinForms.Editors.Initializer.Init();
            DevExpress.XamarinForms.Navigation.Initializer.Init();
            DevExpress.XamarinForms.DataForm.Initializer.Init();
            DevExpress.XamarinForms.Popup.Initializer.Init();

            var profiles = Database.GetProfiles();
            if(profiles.Count > 0)
            {
                UserProfile = profiles.First();
            }
            else
            {
                var profile = new Profile()
                {
                    UserName = "Default User",
                    FirstName = String.Empty,
                    LastName = String.Empty,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    ProjectedReleaseDate = DateTime.Now,
                    SentenceDate = DateTime.Now,
                    SentenceLengthMonths = 0,
                    TimeServedDays = 0,
                    UserId = Guid.NewGuid().ToString()
                };
                Database.SaveProfile(profile);
                UserProfile = Database.GetProfiles().First();
            }

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<NavigationService>();

            InitializeComponent();

            var navigationService = DependencyService.Get<INavigationService>();
            // Use the NavigateToAsync<ViewModelName> method
            // to display the corresponding view.
            // Code lines below show how to navigate to a specific page.
            // Comment out all but one of these lines
            // to open the corresponding page.
            //navigationService.NavigateToAsync<LoginViewModel>();
            navigationService.NavigateToAsync<MainViewModel>();
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

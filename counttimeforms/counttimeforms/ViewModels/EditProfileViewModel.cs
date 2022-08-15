using counttimeforms.Models;
using DevExpress.XamarinForms.DataForm;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace counttimeforms.ViewModels
{
    internal class EditProfileViewModel : BaseViewModel
    {
        public const string ViewName = "Edit Profile";


        string username;
        DateTime startDate;
        DateTime endDate;

        public EditProfileViewModel()
        {
            Title = "Edit Profile";
            username = App.UserProfile.UserName;
            startDate = App.UserProfile.StartDate.Value;
            endDate = App.UserProfile.EndDate.Value;
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }


        public string UserName
        {
            get => this.username;
            set => SetProperty(ref this.username, value);
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set => SetProperty(ref this.startDate, value);
        }
        public DateTime EndDate
        {
            get => this.endDate;
            set => SetProperty(ref this.endDate, value);
        }

        [DataFormDisplayOptions(IsVisible = false)]
        public Command SaveCommand { get; }

        [DataFormDisplayOptions(IsVisible = false)]
        public Command CancelCommand { get; }


        bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(this.username);
        }

        async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
        }

        async void OnSave()
        {
            Profile profile = App.UserProfile;
            profile.UserName = UserName;
            profile.StartDate = StartDate;
            profile.EndDate = EndDate;

            App.Database.SaveProfile(profile);
            App.UserProfile = profile;

            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
            
        }
    }
}

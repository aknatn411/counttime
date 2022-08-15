using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace counttimeforms.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public const string ViewName = "AboutPage";
        public AboutViewModel()
        {
            var test = App.Database.GetProfiles();
            Title = "About" + " results: "+ test.Count;
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.devexpress.com/xamarin/"));
        }

        public ICommand OpenWebCommand { get; }
    }
}
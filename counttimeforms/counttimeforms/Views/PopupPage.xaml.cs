using counttimeforms.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace counttimeforms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPage : ContentPage
    {
        public PopupPage()
        {
            InitializeComponent();
            BindingContext = new PopupViewModel();
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
            Popup.IsOpen = true;
        }
    }
}
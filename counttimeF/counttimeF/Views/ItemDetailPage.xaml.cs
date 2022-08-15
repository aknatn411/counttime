using counttimeF.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace counttimeF.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
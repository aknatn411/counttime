using counttimeforms.ViewModels;
using DevExpress.XamarinForms.CollectionView;
using DevExpress.XamarinForms.Navigation;
using System;
using System.Linq;
using Xamarin.Forms;

namespace counttimeforms.Views
{
    public partial class MainPage : DrawerPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
            MessagingCenter.Instance.Subscribe<View>(this, "OPEN_DRAWER",
                (sender) => { IsDrawerOpened = !IsDrawerOpened; });
        }

        void OnListViewItemSelected(object sender, CollectionViewSelectionChangedEventArgs e)
        {
            var collectionView = (DXCollectionView)sender;
            if (e.SelectedItems.Count == 0 && e.DeselectedItems.Count == 1)
            {
                collectionView.SelectedItem = e.DeselectedItems[0];
                return;
            }
            IsDrawerOpened = false;
        }

        void OnCloseTapped(object sender, EventArgs e)
        {
            IsDrawerOpened = false;
        }
    }
}

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.BottomNavigation;
using Xamarin.Essentials;

namespace counttime
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            Button calculate = FindViewById<Button>(Resource.Id.calculate);
            calculate.Click += (sender, e) => { OnCalculateClick(); };
            var namePref = Preferences.Get("Name", string.Empty);
            if (namePref != string.Empty)
            {
                var name = FindViewById<TextView>(Resource.Id.editText1);
                if (name != null)
                {
                    name.Text = namePref;
                }
            }
            var rDatePref = Preferences.Get("ReleaseDate", System.DateTime.Now);
            if (rDatePref != null)
            {
                DatePicker rDate = (DatePicker)FindViewById(Resource.Id.datePicker1);
                if (rDate != null)
                {
                    rDate.DateTime = rDatePref;
                    var rText = FindViewById<TextView>(Resource.Id.remainingdays);
                    if (rText != null)
                    {

                    }
                    rText.Text = (rDate.DateTime - System.DateTime.Now).TotalDays + " days to release.";
                }
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    SetContentView(Resource.Layout.activity_main);
                    return true;
                case Resource.Id.navigation_dashboard:
                    SetContentView(Resource.Layout.layout1);
                    return true;
            }
            return false;
        }
        public bool OnCalculateClick()
        {
            var name = FindViewById<TextView>(Resource.Id.editText1);
            if (name != null)
            {
                Preferences.Set("Name", name.Text);
            }
            DatePicker dp = (DatePicker)FindViewById(Resource.Id.datePicker1);
            if (dp != null)
            {
                Preferences.Set("ReleaseDate", dp.DateTime);
                FindViewById<TextView>(Resource.Id.remainingdays).Text = (dp.DateTime - System.DateTime.Now).TotalDays + " days to release.";
                return true;
            }
            
            return true;
        }
    }
}


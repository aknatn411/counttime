using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using counttime.Data;
using counttime.Models;
using Google.Android.Material.BottomNavigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace counttime
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class ProfileEditActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        static CountTimeDatabase database;
        private Profile UserProfile;

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
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    Intent intent1 = new Intent(this, typeof(MainActivity));
                    StartActivity(intent1);
                    return true;
                case Resource.Id.navigation_events:
                    Intent intent2 = new Intent(this, typeof(EventListActivity));
                    StartActivity(intent2);
                    //SetContentView(Resource.Layout.activity);
                    return true;
                case Resource.Id.navigation_diary:
                    Intent intent3 = new Intent(this, typeof(DiaryListActivity));
                    StartActivity(intent3);
                    //SetContentView(Resource.Layout.activity);
                    return true;
            }
            return false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_Profile);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigationProfile);
            navigation.SetOnNavigationItemSelectedListener(this);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();


            DatePickerDialog startDateDialog = new DatePickerDialog(this, OnStartDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DatePickerDialog releaseDateDialog = new DatePickerDialog(this, OnReleaseDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            Button calculate = FindViewById<Button>(Resource.Id.calculate);
            calculate.Click += (sender, e) => { OnCalculateClick(UserProfile, Database); };
            EditText txtStartDate = FindViewById<EditText>(Resource.Id.startDate);
            txtStartDate.Touch += (sender, e) =>
            {
                startDateDialog.Show();
            };
            EditText txtReleaseDate = FindViewById<EditText>(Resource.Id.releaseDate);
            txtReleaseDate.Touch += (sender, e) =>
            {
                releaseDateDialog.Show();
            };
            var UserNameField = FindViewById<TextView>(Resource.Id.UserName);
            if (UserNameField != null)
            {
                UserNameField.Text = UserProfile.UserName;
            }
            var sDatePref = UserProfile.StartDate ??= DateTime.Now;
            if (sDatePref != null)
            {
                startDateDialog.DatePicker.DateTime = sDatePref;
                EditText sDate = (EditText)FindViewById(Resource.Id.startDate);
                if (sDate != null)
                {
                    sDate.Text = sDatePref.ToShortDateString();
                    var rText2 = FindViewById<TextView>(Resource.Id.remainingdays);
                    if (rText2 != null)
                    {
                        rText2.Text = "You've been down " + Math.Round((System.DateTime.Now - DateTime.Parse(sDate.Text)).TotalDays, 0).ToString("n0") + " days.";
                    }

                }
            }
            var rDatePref = UserProfile.EndDate ??= DateTime.Now;
            if (rDatePref != null)
            {
                releaseDateDialog.DatePicker.DateTime = rDatePref;
                EditText rDate = (EditText)FindViewById(Resource.Id.releaseDate);
                if (rDate != null)
                {
                    rDate.Text = rDatePref.ToShortDateString();
                    var rText3 = FindViewById<TextView>(Resource.Id.remainingdays);
                    if (rText3 != null)
                    {

                    }
                    rText3.Text += System.Environment.NewLine + Math.Round((DateTime.Parse(rDate.Text) - System.DateTime.Now).TotalDays, 0).ToString("n0") + " days to release.";
                }
            }

            var totDays = (UserProfile.EndDate.Value - UserProfile.StartDate.Value).TotalDays;
            var comDays = (DateTime.Now - UserProfile.StartDate.Value).TotalDays;
            var totProgress = (comDays / totDays) * 100;
            var rText = FindViewById<TextView>(Resource.Id.remainingdays);
            rText.Text += System.Environment.NewLine + Math.Round(totProgress, 2) + "% of the way there, keep it up!";
        }
        private void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.startDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }

        private void OnReleaseDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.releaseDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }
        public bool OnCalculateClick(Profile UserProfile, CountTimeDatabase Database)
        {
            var name = FindViewById<TextView>(Resource.Id.UserName);
            if (name != null)
            {
                UserProfile.UserName = name.Text;
            }

            EditText rDate = (EditText)FindViewById(Resource.Id.releaseDate);
            EditText sDate = (EditText)FindViewById(Resource.Id.startDate);
            if (sDate != null)
            {
                UserProfile.StartDate = DateTime.Parse(sDate.Text);
                var rText4 = FindViewById<TextView>(Resource.Id.remainingdays);
                if (rText4 != null)
                {
                    rText4.Text = "You've been down " + Math.Round((System.DateTime.Now - DateTime.Parse(sDate.Text)).TotalDays, 0).ToString("n0") + " days.";
                }
            }

            if (rDate != null)
            {
                UserProfile.EndDate = DateTime.Parse(rDate.Text);
                FindViewById<TextView>(Resource.Id.remainingdays).Text += System.Environment.NewLine + Math.Round((DateTime.Parse(rDate.Text) - System.DateTime.Now).TotalDays, 0).ToString("n0") + " days to release.";

            }

            _ = Database.SaveProfile(UserProfile);
            Android.Widget.Toast.MakeText(this, "Profile Saved", Android.Widget.ToastLength.Long).Show();

            Intent intent1 = new Intent(this, typeof(MainActivity));
            StartActivity(intent1);

            var totDays = (DateTime.Parse(rDate.Text) - DateTime.Parse(sDate.Text)).TotalDays;
            var comDays = (DateTime.Now - (DateTime.Parse(sDate.Text))).TotalDays;
            var totProgress = (comDays / totDays) * 100;
            var rText = FindViewById<TextView>(Resource.Id.remainingdays);
            rText.Text += System.Environment.NewLine + Math.Round(totProgress, 2) + "% of the way there, keep it up!";
            var calc = (int)((comDays / totDays) * 100);
            return true;
        }
    }
}
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
using System.IO;
using System.Linq;

namespace counttime
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var UserProfile = getProfile();

            textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

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

            ProgressBar progressBar1 = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            var totDays = (rDatePref - sDatePref).TotalDays;
            var comDays = (DateTime.Now - sDatePref).TotalDays;
            var totProgress = (comDays / totDays) * 100;
            var rText = FindViewById<TextView>(Resource.Id.remainingdays);
            rText.Text += System.Environment.NewLine + Math.Round(totProgress, 2) + "% of the way there, keep it up!";

            progressBar1.Progress = (int)totProgress;

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
                    //SetContentView(Resource.Layout.activity_main);
                    return true;
                case Resource.Id.navigation_events:
                    Intent intent = new Intent(this, typeof(EventListActivity));
                    StartActivity(intent);
                    //SetContentView(Resource.Layout.activity);
                    return true;
                case Resource.Id.navigation_diary:
                    Intent intent2 = new Intent(this, typeof(DiaryListActivity));
                    StartActivity(intent2);
                    //SetContentView(Resource.Layout.activity);
                    return true;
            }
            return false;
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

            ProgressBar progressBar1 = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            var totDays = (DateTime.Parse(rDate.Text) - DateTime.Parse(sDate.Text)).TotalDays;
            var comDays = (DateTime.Now - (DateTime.Parse(sDate.Text))).TotalDays;
            var totProgress = (comDays / totDays) * 100;
            var rText = FindViewById<TextView>(Resource.Id.remainingdays);
            rText.Text += System.Environment.NewLine + Math.Round(totProgress, 2) + "% of the way there, keep it up!";
            var calc = (int)((comDays / totDays) *100);
            progressBar1.Progress = calc;

            return true;
        }
        public Profile getProfile()
        {
            var dbResult = Database.GetProfiles();
            if (dbResult.Count == 0)
            {
                var profile = new Profile();
                profile.FirstName = String.Empty;
                profile.LastName = String.Empty;
                profile.UserName = String.Empty;
                profile.UserId = String.Empty;
                profile.SentenceLengthMonths = 0;
                profile.TimeServedDays = 0;

                var saveResult = Database.SaveProfile(profile);
                if (saveResult > 0)
                {
                    var newProfile = Database.GetProfile(saveResult);
                    return newProfile;
                }
            }
            else if (dbResult.Count == 1)
            {
                return dbResult.First();
            }
            return null;
        }
    }
}


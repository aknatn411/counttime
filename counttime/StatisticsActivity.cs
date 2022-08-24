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
    public class StatisticsActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
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
        public Profile UserProfile;
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
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
                case Resource.Id.navigation_history:
                    Intent intent4 = new Intent(this, typeof(HistoryActivity));
                    StartActivity(intent4);
                    //SetContentView(Resource.Layout.activity);
                    return true;
            }
            return false;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SupportActionBar.Hide();
            SetContentView(Resource.Layout.activity_Statistics);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigationStatistics);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.Menu.GetItem(0).SetChecked(true);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();

            var sDatePref = UserProfile.StartDate ??= DateTime.Now;
            var rDatePref = UserProfile.EndDate ??= DateTime.Now;
            var totalDays = (rDatePref - sDatePref).TotalDays;
            var comDays = (DateTime.Now - sDatePref).TotalDays;
            var percentComplete= (comDays/totalDays) * 100;

            TextView remDaysValue = FindViewById<TextView>(Resource.Id.StatsDaysRemainingValue);
            remDaysValue.Text = Math.Round((rDatePref - DateTime.Now).TotalDays, 0).ToString("n0");
            
            var wednesdaysLeft = CountDays(DayOfWeek.Wednesday, sDatePref, rDatePref);
            var wednesdaysLefValuetView = FindViewById<TextView>(Resource.Id.StatsWednesdaysRemainingValue);
            wednesdaysLefValuetView.Text = wednesdaysLeft.ToString("n0");

            var daysServedValue = FindViewById<TextView>(Resource.Id.StatsDaysServedValue);
            daysServedValue.Text = Math.Round((DateTime.Now - sDatePref).TotalDays).ToString("n0");

            var percentCompleteValue = FindViewById<TextView>(Resource.Id.StatsPercentCompleteValue);

            percentCompleteValue.Text = Math.Round(percentComplete, 0) + "%";

            var locations = Database.GetLocations().OrderByDescending(l => l.ArrivalDate);
            string locationDays = String.Empty;

            var currentLocation = locations.FirstOrDefault();
            if (currentLocation != null)
            {
                locationDays += System.Environment.NewLine + Math.Round(((DateTime.Now - currentLocation.ArrivalDate).TotalDays / totalDays) * 100, 2) + "% served at " + currentLocation.Name;
            }
            else
            {
                locationDays = "Not currently tracking locations.";
            }
            

            foreach (Location location in locations)
            {
                var curDate = location.ArrivalDate;
                
                var nextLocation = locations.Where(l => l.ArrivalDate < curDate).OrderByDescending(l => l.ArrivalDate).FirstOrDefault();

                if(nextLocation != null)
                {
                    var daysDiff = (curDate - nextLocation.ArrivalDate).TotalDays;
                    locationDays += System.Environment.NewLine + Math.Round((daysDiff / totalDays) * 100, 2) + "% served at " + nextLocation.Name;
                }
            }

            var percentAtText = FindViewById<TextView>(Resource.Id.StatsPercentAtText);
            percentAtText.Text = locationDays;
        }

        static int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]
            if (sinceLastDay < 0) sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay) count++;

            return count;
        }
    }
}
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
    public class EventAddActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
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
                    Intent intent1 = new Intent(this, typeof(MainActivity));
                    StartActivity(intent1);
                    return true;
                case Resource.Id.navigation_events:
                    Intent intent2 = new Intent(this, typeof(EventListActivity));
                    StartActivity(intent2);
                    //SetContentView(Resource.Layout.activity);
                    return true;
            }
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_EventAdd);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation4);
            navigation.SetOnNavigationItemSelectedListener(this);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();

            DatePickerDialog startDateDialog = new DatePickerDialog(this, OnStartDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DatePickerDialog endDateDialog = new DatePickerDialog(this, OnEndDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            EditText txtStartDate = FindViewById<EditText>(Resource.Id.EventStartDate);
            if(txtStartDate.Text == String.Empty)
            {
                txtStartDate.Text = DateTime.Now.ToShortDateString();
            }
            txtStartDate.Touch += (sender, e) =>
            {
                startDateDialog.Show();
            };

            EditText txtEndDate = FindViewById<EditText>(Resource.Id.EventEndDate);
            if (txtEndDate.Text == String.Empty)
            {
                txtEndDate.Text = DateTime.Now.ToShortDateString();
            }
            txtEndDate.Touch += (sender, e) =>
            {
                endDateDialog.Show();
            };

            Button saveEvent = FindViewById<Button>(Resource.Id.SaveEvent);
            saveEvent.Click += (sender, e) => { OnSaveEventTouch(UserProfile, Database); };
        }

        private void OnSaveEventTouch(Profile userProfile, CountTimeDatabase database)
        {
            Event newEvent = new Event()
            {
                Name = FindViewById<EditText>(Resource.Id.EventName).Text,
                EventStartDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.EventStartDate).Text),
                EventEndDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.EventEndDate).Text),
                ProfileId = UserProfile.Id,
                EventType = "Default",
                IsEditable = true,
                IsSystem = false,
            };
            Database.SaveEvent(newEvent);
            Intent intent2 = new Intent(this, typeof(EventListActivity));
            StartActivity(intent2);
        }

        private void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.EventStartDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }

        private void OnEndDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.EventEndDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }
    }
}
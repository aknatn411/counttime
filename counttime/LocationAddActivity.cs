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
    public class LocationAddActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        static CountTimeDatabase database;
        private Location ctLocationEdit;

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
            SetContentView(Resource.Layout.activity_LocationAdd);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigationLocationAdd);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.Menu.GetItem(0).SetChecked(true);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();
            if(Intent.GetIntExtra("LocationId", 0) != 0)
            {
                ctLocationEdit = database.GetLocation(Intent.GetIntExtra("LocationId", 0));
            }

            DatePickerDialog createdtDateDialog = new DatePickerDialog(this, OnStartDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            

            EditText txtCreatedtDate = FindViewById<EditText>(Resource.Id.LocationArrivalDate);
            if(txtCreatedtDate.Text == String.Empty)
            {
                txtCreatedtDate.Text = DateTime.Now.ToShortDateString();
            }
            txtCreatedtDate.Touch += (sender, e) =>
            {
                createdtDateDialog.Show();
            };

            if (ctLocationEdit != null && ctLocationEdit.Id > 0)
            {
                createdtDateDialog.UpdateDate(ctLocationEdit.ArrivalDate);
                txtCreatedtDate.Text = ctLocationEdit.ArrivalDate.ToShortDateString();
                EditText Subject = FindViewById<EditText>(Resource.Id.LocationName);
                Subject.Text = ctLocationEdit.Name;
                EditText Detail = FindViewById<EditText>(Resource.Id.LocationNotes);
                Detail.Text = ctLocationEdit.Notes;
                EditText daysLost = FindViewById<EditText>(Resource.Id.LocationCell);
                daysLost.Text = ctLocationEdit.Cell.ToString();
            }
            else
            {
                createdtDateDialog.UpdateDate(DateTime.Now);
                FindViewById<Button>(Resource.Id.DeleteLocation).Visibility = ViewStates.Invisible;
            }

            Button saveLocation = FindViewById<Button>(Resource.Id.SaveLocation);
            saveLocation.Click += (sender, e) => { OnSaveLocationTouch(); };

            Button deleteLocation = FindViewById<Button>(Resource.Id.DeleteLocation);
            deleteLocation.Click += (sender, e) => { OnDeleteLocationTouch(); };
        }

        private void OnDeleteLocationTouch()
        {
            if (ctLocationEdit != null && ctLocationEdit.Id > 0)
            {
                Database.DeleteLocation(ctLocationEdit);
                Android.Widget.Toast.MakeText(this, "Location Deleted", Android.Widget.ToastLength.Long).Show();
                Intent intent3 = new Intent(this, typeof(LocationListActivity));
                StartActivity(intent3);
            }
            
        }

        private void OnSaveLocationTouch()
        {
            Location newLocation = new Location()
            {
                Name = FindViewById<EditText>(Resource.Id.LocationName).Text,
                ArrivalDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.LocationArrivalDate).Text),
                Notes = FindViewById<EditText>(Resource.Id.LocationNotes).Text,
                Cell = FindViewById<EditText>(Resource.Id.LocationCell).Text,
                UserId = UserProfile.Id
            };
            if(ctLocationEdit != null && ctLocationEdit.Id > 0)
            {
                newLocation.Id = ctLocationEdit.Id;
            }
            Database.SaveLocation(newLocation);
            Android.Widget.Toast.MakeText(this, "Location Saved", Android.Widget.ToastLength.Long).Show();

            Intent intent2 = new Intent(this, typeof(LocationListActivity));
            StartActivity(intent2);
        }

        private void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.LocationArrivalDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }
    }
}
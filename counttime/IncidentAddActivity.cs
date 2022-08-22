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
    public class IncidentAddActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        static CountTimeDatabase database;
        private Incident ctIncidentEdit;

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
            SetContentView(Resource.Layout.activity_IncidentAdd);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigationIncidentAdd);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.Menu.GetItem(0).SetChecked(true);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();
            if(Intent.GetIntExtra("IncidentId", 0) != 0)
            {
                ctIncidentEdit = database.GetIncident(Intent.GetIntExtra("IncidentId", 0));
            }

            DatePickerDialog createdtDateDialog = new DatePickerDialog(this, OnStartDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            

            EditText txtCreatedtDate = FindViewById<EditText>(Resource.Id.IncidentDate);
            if(txtCreatedtDate.Text == String.Empty)
            {
                txtCreatedtDate.Text = DateTime.Now.ToShortDateString();
            }
            txtCreatedtDate.Touch += (sender, e) =>
            {
                createdtDateDialog.Show();
            };

            if (ctIncidentEdit != null && ctIncidentEdit.Id > 0)
            {
                createdtDateDialog.UpdateDate(ctIncidentEdit.IncidentDate);
                txtCreatedtDate.Text = ctIncidentEdit.IncidentDate.ToShortDateString();
                EditText Subject = FindViewById<EditText>(Resource.Id.IncidentName);
                Subject.Text = ctIncidentEdit.Name;
                EditText Detail = FindViewById<EditText>(Resource.Id.IncidentDetails);
                Detail.Text = ctIncidentEdit.Details;
                EditText daysLost = FindViewById<EditText>(Resource.Id.IncidentDaysLost);
                daysLost.Text = ctIncidentEdit.DaysLost.ToString();
            }
            else
            {
                createdtDateDialog.UpdateDate(DateTime.Now);
                FindViewById<Button>(Resource.Id.DeleteIncident).Visibility = ViewStates.Invisible;
            }

            Button saveIncident = FindViewById<Button>(Resource.Id.SaveIncident);
            saveIncident.Click += (sender, e) => { OnSaveIncidentTouch(); };

            Button deleteIncident = FindViewById<Button>(Resource.Id.DeleteIncident);
            deleteIncident.Click += (sender, e) => { OnDeleteIncidentTouch(); };
        }

        private void OnDeleteIncidentTouch()
        {
            if (ctIncidentEdit != null && ctIncidentEdit.Id > 0)
            {
                Database.DeleteIncident(ctIncidentEdit);
                Android.Widget.Toast.MakeText(this, "Incident Deleted", Android.Widget.ToastLength.Long).Show();
                Intent intent3 = new Intent(this, typeof(IncidentListActivity));
                StartActivity(intent3);
            }
            
        }

        private void OnSaveIncidentTouch()
        {
            Incident newIncident = new Incident()
            {
                Name = FindViewById<EditText>(Resource.Id.IncidentName).Text,
                IncidentDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.IncidentDate).Text),
                Details = FindViewById<EditText>(Resource.Id.IncidentDetails).Text,
                DaysLost = (FindViewById<EditText>(Resource.Id.IncidentDaysLost).Text == null || FindViewById<EditText>(Resource.Id.IncidentDaysLost).Text == String.Empty) ? 0 : int.Parse(FindViewById<EditText>(Resource.Id.IncidentDaysLost).Text),
                UserId = UserProfile.Id
            };
            if(ctIncidentEdit != null && ctIncidentEdit.Id > 0)
            {
                newIncident.Id = ctIncidentEdit.Id;
            }
            Database.SaveIncident(newIncident);
            Android.Widget.Toast.MakeText(this, "Incident Saved", Android.Widget.ToastLength.Long).Show();

            Intent intent2 = new Intent(this, typeof(IncidentListActivity));
            StartActivity(intent2);
        }

        private void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.IncidentDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }
    }
}
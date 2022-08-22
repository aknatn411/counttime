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
    public class WorkAssignmentAddActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        static CountTimeDatabase database;
        private WorkAssignment ctWorkAssignmentEdit;

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
            SetContentView(Resource.Layout.activity_WorkAssignmentAdd);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigationWorkAssignmentAdd);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.Menu.GetItem(0).SetChecked(true);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();
            if(Intent.GetIntExtra("WorkAssignmentId", 0) != 0)
            {
                ctWorkAssignmentEdit = database.GetWorkAssignment(Intent.GetIntExtra("WorkAssignmentId", 0));
            }

            DatePickerDialog createdtDateDialog = new DatePickerDialog(this, OnStartDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            

            EditText txtCreatedtDate = FindViewById<EditText>(Resource.Id.WorkAssignmentArrivalDate);
            if(txtCreatedtDate.Text == String.Empty)
            {
                txtCreatedtDate.Text = DateTime.Now.ToShortDateString();
            }
            txtCreatedtDate.Touch += (sender, e) =>
            {
                createdtDateDialog.Show();
            };

            if (ctWorkAssignmentEdit != null && ctWorkAssignmentEdit.Id > 0)
            {
                createdtDateDialog.UpdateDate(ctWorkAssignmentEdit.StartDate);
                txtCreatedtDate.Text = ctWorkAssignmentEdit.StartDate.ToShortDateString();
                EditText Subject = FindViewById<EditText>(Resource.Id.WorkAssignmentName);
                Subject.Text = ctWorkAssignmentEdit.Name;
                EditText Detail = FindViewById<EditText>(Resource.Id.WorkAssignmentNotes);
                Detail.Text = ctWorkAssignmentEdit.Notes;
            }
            else
            {
                createdtDateDialog.UpdateDate(DateTime.Now);
                FindViewById<Button>(Resource.Id.DeleteWorkAssignment).Visibility = ViewStates.Invisible;
            }

            Button saveWorkAssignment = FindViewById<Button>(Resource.Id.SaveWorkAssignment);
            saveWorkAssignment.Click += (sender, e) => { OnSaveWorkAssignmentTouch(); };

            Button deleteWorkAssignment = FindViewById<Button>(Resource.Id.DeleteWorkAssignment);
            deleteWorkAssignment.Click += (sender, e) => { OnDeleteWorkAssignmentTouch(); };
        }

        private void OnDeleteWorkAssignmentTouch()
        {
            if (ctWorkAssignmentEdit != null && ctWorkAssignmentEdit.Id > 0)
            {
                Database.DeleteWorkAssignment(ctWorkAssignmentEdit);
                Android.Widget.Toast.MakeText(this, "WorkAssignment Deleted", Android.Widget.ToastLength.Long).Show();
                Intent intent3 = new Intent(this, typeof(WorkAssignmentListActivity));
                StartActivity(intent3);
            }
            
        }

        private void OnSaveWorkAssignmentTouch()
        {
            WorkAssignment newWorkAssignment = new WorkAssignment()
            {
                Name = FindViewById<EditText>(Resource.Id.WorkAssignmentName).Text,
                StartDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.WorkAssignmentArrivalDate).Text),
                Notes = FindViewById<EditText>(Resource.Id.WorkAssignmentNotes).Text,
                UserId = UserProfile.Id
            };
            if(ctWorkAssignmentEdit != null && ctWorkAssignmentEdit.Id > 0)
            {
                newWorkAssignment.Id = ctWorkAssignmentEdit.Id;
            }
            Database.SaveWorkAssignment(newWorkAssignment);
            Android.Widget.Toast.MakeText(this, "WorkAssignment Saved", Android.Widget.ToastLength.Long).Show();

            Intent intent2 = new Intent(this, typeof(WorkAssignmentListActivity));
            StartActivity(intent2);
        }

        private void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.WorkAssignmentArrivalDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }
    }
}
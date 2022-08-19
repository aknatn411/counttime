using Android.App;
using Android.Content;
using Android.OS;
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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class EventAddActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        static CountTimeDatabase database;
        private Event ctEventEdit;
        public string selectedEventType;

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
            if (Intent.GetIntExtra("EventId", 0) != 0)
            {
                ctEventEdit = database.GetEvent(Intent.GetIntExtra("EventId", 0));
            }

            Spinner spinner = FindViewById<Spinner>(Resource.Id.EventTypeSpinner);


            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.event_type_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            DatePickerDialog startDateDialog = new DatePickerDialog(this, OnStartDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DatePickerDialog endDateDialog = new DatePickerDialog(this, OnEndDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);



            EditText txtStartDate = FindViewById<EditText>(Resource.Id.EventStartDate);
            if (txtStartDate.Text == String.Empty)
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

            if (ctEventEdit != null && ctEventEdit.Id > 0)
            {
                startDateDialog.UpdateDate(ctEventEdit.EventStartDate);
                endDateDialog.UpdateDate(ctEventEdit.EventEndDate);
                txtStartDate.Text = ctEventEdit.EventStartDate.ToShortDateString();
                txtEndDate.Text = ctEventEdit.EventEndDate.ToShortDateString();
                EditText eventeventName = FindViewById<EditText>(Resource.Id.EventName);
                eventeventName.Text = ctEventEdit.Name;
                Switch isShowOnHomeScreen = FindViewById<Switch>(Resource.Id.EventIsshowOnHomeScreen);
                isShowOnHomeScreen.Checked = ctEventEdit.IsShowOnHomeScreen;
                selectedEventType = ctEventEdit.EventType;
                switch (this.selectedEventType)
                {
                    case "Single":
                        spinner.SetSelection(0);
                        break;
                    case "Duration":
                        spinner.SetSelection(1);
                        break;
                    case "Milestone":
                        spinner.SetSelection(2);
                        break;
                    case "Anniversary":
                        spinner.SetSelection(3);
                        break;
                    default:
                        spinner.SetSelection(0);
                        break;
                }

            }
            else
            {
                startDateDialog.UpdateDate(DateTime.Now);
                endDateDialog.UpdateDate(DateTime.Now);
                FindViewById<Button>(Resource.Id.DeleteEvent).Visibility = ViewStates.Invisible;
            }

            Button saveEvent = FindViewById<Button>(Resource.Id.SaveEvent);
            saveEvent.Click += (sender, e) => { OnSaveEventTouch(); };

            Button deleteEvent = FindViewById<Button>(Resource.Id.DeleteEvent);
            deleteEvent.Click += (sender, e) => { OnDeleteEventTouch(); };
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            selectedEventType = (string)spinner.GetItemAtPosition(e.Position);
        }

        private void OnDeleteEventTouch()
        {
            if (ctEventEdit != null && ctEventEdit.Id > 0)
            {
                Database.DeleteEvent(ctEventEdit);
                Android.Widget.Toast.MakeText(this, "Event Deleted", Android.Widget.ToastLength.Long).Show();
                Intent intent3 = new Intent(this, typeof(EventListActivity));
                StartActivity(intent3);
            }

        }

        private void OnSaveEventTouch()
        {
            Event newEvent = new Event()
            {
                Name = FindViewById<EditText>(Resource.Id.EventName).Text,
                EventStartDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.EventStartDate).Text),
                EventEndDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.EventEndDate).Text),
                ProfileId = UserProfile.Id,
                EventType = selectedEventType,
                IsEditable = true,
                IsSystem = false,
                IsShowOnHomeScreen = FindViewById<Switch>(Resource.Id.EventIsshowOnHomeScreen).Checked
            };
            if (ctEventEdit != null && ctEventEdit.Id > 0)
            {
                newEvent.Id = ctEventEdit.Id;
            }
            Database.SaveEvent(newEvent);
            Android.Widget.Toast.MakeText(this, "Event Saved", Android.Widget.ToastLength.Long).Show();

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
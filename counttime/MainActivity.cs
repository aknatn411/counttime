using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Widget;
using counttime.Data;
using counttime.Models;
using Google.Android.Material.BottomNavigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace counttime
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        static CountTimeDatabase database;

        // Create the database connection as a singleton.
        public static CountTimeDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new CountTimeDatabase(System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "CountTimeDatabase.db3"));
                }
                return database;
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SupportActionBar.Hide();
            SetContentView(Resource.Layout.activity_main);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.Menu.GetItem(0).SetChecked(true);

            var UserProfile = getProfile();

            Button editProfile = FindViewById<Button>(Resource.Id.EditProfileButton);
            editProfile.Touch += (sender, e) =>
            {
                Intent intent1 = new Intent(this, typeof(ProfileEditActivity));
                StartActivity(intent1);
            };

            ProgressBar progressBar1 = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            progressBar1.Touch += (sender, e) =>
            {
                Intent intent1 = new Intent(this, typeof(StatisticsActivity));
                StartActivity(intent1);
            };
            var totDays = (UserProfile.EndDate.Value - UserProfile.StartDate.Value).TotalDays;
            var comDays = (DateTime.Now - UserProfile.StartDate.Value).TotalDays;
            var totProgress = (comDays / totDays) * 100;

            var futureMilestoneText = FindViewById<TextView>(Resource.Id.MainFutureMilestone);
            var pastMilestoneText = FindViewById<TextView>(Resource.Id.MainPastMilestone);
            var futureMilestone = Database.GetNearestFutureMilestone();
            var pastMilestone = Database.GetNearestPastMilestone();

            if (futureMilestone != null)
            {
                futureMilestoneText.Text = futureMilestone.Name + " is coming up in " + Math.Round((futureMilestone.EventStartDate - DateTime.Now).TotalDays, 0) + " days";
            }

            if (pastMilestone != null)
            {
                pastMilestoneText.Text = Math.Round((DateTime.Now - pastMilestone.EventStartDate).TotalDays, 0) + " days since " + pastMilestone.Name;
            }


            progressBar1.Progress = (int)totProgress;
            var percentText = FindViewById<TextView>(Resource.Id.mainPercentText);
            var daysRemaining = FindViewById<TextView>(Resource.Id.MainDaysRemaining);
            var daysRemainingCalc = Math.Round((UserProfile.EndDate.Value - DateTime.Now).TotalDays, 0);
            daysRemaining.Text = daysRemainingCalc.ToString() + " days until release";
            percentText.Text = Math.Round(totProgress, 0) + "%";
            //percentText.Typeface = Typeface.CreateFromAsset(Assets, "Turis-Light.otf");

            var events = Database.GetHomeScreenEvents();
            if (events != null && events.Count > 0)
            {
                //rText.Text += System.Environment.NewLine + events.Count + " Events in range.";

                var eventList = FindViewById<ListView>(Resource.Id.MainEvents);
                eventList.Adapter = new EventDurationAdapter(this, events);
            }
        }

        public class EventDurationAdapter : BaseAdapter<Event>
        {
            public List<Event> sList;
            private Context sContext;
            public EventDurationAdapter(Context context, List<Event> list)
            {
                sList = list;
                sContext = context;
            }
            public override Event this[int position]
            {
                get
                {
                    return sList[position];
                }
            }
            public override int Count
            {
                get
                {
                    return sList.Count;
                }
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View row = convertView;
                try
                {
                    if (row == null)
                    {
                        row = LayoutInflater.From(sContext).Inflate(Resource.Layout.eventDuration_expandableListItem, null, false);
                    }
                    TextView txtName = row.FindViewById<TextView>(Resource.Id.ListEventName);
                    DateTime sDate = sList[position].EventStartDate;
                    DateTime endDate = sList[position].EventEndDate;
                    string eType = sList[position].EventType;
                    var progressBar = row.FindViewById<ProgressBar>(Resource.Id.eventDurationProgressBar);
                    progressBar.Id = progressBar.Id + 1;
                    if (eType == "Duration")
                    {
                        txtName.Text = sList[position].Name + " \n" + sDate.ToShortDateString() + " to " + endDate.ToShortDateString();
                        var totDays = (endDate - sDate).TotalDays;
                        var comDays = (DateTime.Now - sDate).TotalDays;
                        var totProgress = (comDays / totDays) * 100;
                        
                        progressBar.Progress = (int)totProgress;
                        progressBar.ScaleY = 2;
                    }
                    else
                    {
                        txtName.Text = sList[position].Name + " coming up in " + Math.Round((sDate - DateTime.Now).TotalDays, 0) + " days";
                        progressBar.Visibility = ViewStates.Invisible;
                    }


                    
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                finally { }
                return row;
            }
        }

        private void EditProfile_Touch(object sender, View.TouchEventArgs e)
        {
            throw new NotImplementedException();
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
                case Resource.Id.navigation_history:
                    Intent intent4 = new Intent(this, typeof(HistoryActivity));
                    StartActivity(intent4);
                    //SetContentView(Resource.Layout.activity);
                    return true;
            }
            return false;
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
                profile.StartDate = DateTime.Now;
                profile.EndDate = DateTime.Now;

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


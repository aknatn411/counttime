using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using counttime.Data;
using counttime.Models;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Button;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace counttime
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class EventListActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
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
        public List<Event> EventList;
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    return true;
                case Resource.Id.navigation_events:

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SupportActionBar.Hide();
            SetContentView(Resource.Layout.activity_EventList);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation3);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.Menu.GetItem(1).SetChecked(true);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();
            ListView listView = FindViewById<ListView>(Resource.Id.EventListView);
            MaterialButton addEventButton = FindViewById<MaterialButton>(Resource.Id.AddEventButton);
            addEventButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EventAddActivity));
                StartActivity(intent);
            };
            EventList = Database.GetEvents();
            EventList.OrderBy(e => e.EventStartDate);
            var adapter = new EventAdapter(this, EventList);
            listView.Adapter = adapter;
            listView.ItemClick += listViewItemClickHandler;
        }

        private void listViewItemClickHandler(object sender, AdapterView.ItemClickEventArgs e)
        {
            var ctEvent = EventList.ElementAt(e.Position);
            
            Intent intent = new Intent(this, typeof(EventAddActivity));
            intent.PutExtra("EventId", ctEvent.Id);
            StartActivity(intent);
        }
    }

    public class EventAdapter : BaseAdapter<Event>
    {
        public List<Event> sList;
        private Context sContext;
        public EventAdapter(Context context, List<Event> list)
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
                    row = LayoutInflater.From(sContext).Inflate(Resource.Layout.event_expandableListItem, null, false);
                }
                TextView txtName = row.FindViewById<TextView>(Resource.Id.ListEventName);
                txtName.Text = sList[position].Name + " " + sList[position].EventStartDate.ToShortDateString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally { }
            return row;
        }
    }
}
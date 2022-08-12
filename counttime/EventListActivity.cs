using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
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
            }
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_EventList);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation3);
            navigation.SetOnNavigationItemSelectedListener(this);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();
            ListView listView = FindViewById<ListView>(Resource.Id.EventListView);

            string[] items = new string[] { "Vegetables", "Fruits", "Flower Buds", "Legumes", "Bulbs", "Tubers" };
            List<string> itemsList = new List<string>(items);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, itemsList);
            listView.Adapter = adapter;
        }
    }
}
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
    public class HistoryActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
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
            SetContentView(Resource.Layout.activity_History);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation7);
            navigation.SetOnNavigationItemSelectedListener(this);
            navigation.Menu.GetItem(3).SetChecked(true);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();

            var historyResults = Database.GetHistory().OrderByDescending(p => p.Date).ToList();
            ListView listView = FindViewById<ListView>(Resource.Id.HistoryListView);
            var adapter = new HistoryViewModelAdapter(this, historyResults);
            listView.Adapter = adapter;
            //listView.ItemClick += listViewItemClickHandler;
        }
        public class HistoryViewModelAdapter : BaseAdapter<HistoryViewModel>
        {
            public List<HistoryViewModel> sList;
            private Context sContext;
            public HistoryViewModelAdapter(Context context, List<HistoryViewModel> list)
            {
                sList = list;
                sContext = context;
            }
            public override HistoryViewModel this[int position]
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
                        row = LayoutInflater.From(sContext).Inflate(Resource.Layout.history_expandableListItem, null, false);
                    }
                    TextView txtName = row.FindViewById<TextView>(Resource.Id.ListHistoryName);
                    txtName.Text = sList[position].Name + " " + sList[position].Date.ToShortDateString();
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
}
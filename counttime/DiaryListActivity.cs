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
    public class DiaryListActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
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
        public List<Diary> DiaryList;
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
                    //Intent intent = new Intent(this, typeof(DiaryListActivity));
                    //StartActivity(intent);
                    //SetContentView(Resource.Layout.activity);
                    return true;
            }
            return false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_DiaryList);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation5);
            navigation.SetOnNavigationItemSelectedListener(this);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();
            ListView listView = FindViewById<ListView>(Resource.Id.DiaryListView);
            MaterialButton addDiaryButton = FindViewById<MaterialButton>(Resource.Id.AddDiaryButton);
            addDiaryButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(DiaryAddActivity));
                StartActivity(intent);
            };
            DiaryList = Database.GetDiarys();
            DiaryList.OrderByDescending(e => e.CreatedDate);
            var adapter = new DiaryAdapter(this, DiaryList);
            listView.Adapter = adapter;
            listView.ItemClick += listViewItemClickHandler;
        }

        private void listViewItemClickHandler(object sender, AdapterView.ItemClickEventArgs e)
        {
            var ctDiary = DiaryList.ElementAt(e.Position);
            
            Intent intent = new Intent(this, typeof(DiaryAddActivity));
            intent.PutExtra("DiaryId", ctDiary.Id);
            StartActivity(intent);
        }
    }

    public class DiaryAdapter : BaseAdapter<Diary>
    {
        public List<Diary> sList;
        private Context sContext;
        public DiaryAdapter(Context context, List<Diary> list)
        {
            sList = list;
            sContext = context;
        }
        public override Diary this[int position]
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
                    row = LayoutInflater.From(sContext).Inflate(Resource.Layout.diary_expandableListItem, null, false);
                }
                TextView txtName = row.FindViewById<TextView>(Resource.Id.ListDiaryName);
                txtName.Text = sList[position].Subject + " " + sList[position].CreatedDate.ToShortDateString();
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
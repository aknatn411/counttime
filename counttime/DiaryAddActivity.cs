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
    public class DiaryAddActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        static CountTimeDatabase database;
        private Diary ctDiaryEdit;

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
            SetContentView(Resource.Layout.activity_DiaryAdd);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation6);
            navigation.SetOnNavigationItemSelectedListener(this);

            this.UserProfile = Database.GetProfiles().FirstOrDefault();
            if(Intent.GetIntExtra("DiaryId", 0) != 0)
            {
                ctDiaryEdit = database.GetDiary(Intent.GetIntExtra("DiaryId", 0));
            }

            DatePickerDialog createdtDateDialog = new DatePickerDialog(this, OnStartDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            

            EditText txtCreatedtDate = FindViewById<EditText>(Resource.Id.DiaryCreatedDate);
            if(txtCreatedtDate.Text == String.Empty)
            {
                txtCreatedtDate.Text = DateTime.Now.ToShortDateString();
            }
            txtCreatedtDate.Touch += (sender, e) =>
            {
                createdtDateDialog.Show();
            };

            if (ctDiaryEdit != null && ctDiaryEdit.Id > 0)
            {
                createdtDateDialog.UpdateDate(ctDiaryEdit.CreatedDate);
                txtCreatedtDate.Text = ctDiaryEdit.CreatedDate.ToShortDateString();
                EditText Subject = FindViewById<EditText>(Resource.Id.DiarySubject);
                Subject.Text = ctDiaryEdit.Subject;
                EditText Detail = FindViewById<EditText>(Resource.Id.DiaryDetail);
                Detail.Text = ctDiaryEdit.Details;
            }
            else
            {
                createdtDateDialog.UpdateDate(DateTime.Now);
                FindViewById<Button>(Resource.Id.DeleteDiary).Visibility = ViewStates.Invisible;
            }

            Button saveDiary = FindViewById<Button>(Resource.Id.SaveDiary);
            saveDiary.Click += (sender, e) => { OnSaveDiaryTouch(); };

            Button deleteDiary = FindViewById<Button>(Resource.Id.DeleteDiary);
            deleteDiary.Click += (sender, e) => { OnDeleteDiaryTouch(); };
        }

        private void OnDeleteDiaryTouch()
        {
            if (ctDiaryEdit != null && ctDiaryEdit.Id > 0)
            {
                Database.DeleteDiary(ctDiaryEdit);
                Android.Widget.Toast.MakeText(this, "Diary Deleted", Android.Widget.ToastLength.Long).Show();
                Intent intent3 = new Intent(this, typeof(DiaryListActivity));
                StartActivity(intent3);
            }
            
        }

        private void OnSaveDiaryTouch()
        {
            Diary newDiary = new Diary()
            {
                Subject = FindViewById<EditText>(Resource.Id.DiarySubject).Text,
                CreatedDate = DateTime.Parse(FindViewById<EditText>(Resource.Id.DiaryCreatedDate).Text),
                UpdatedDate = DateTime.Now,
                Details = FindViewById<EditText>(Resource.Id.DiaryDetail).Text,
                UserId = UserProfile.Id
            };
            if(ctDiaryEdit != null && ctDiaryEdit.Id > 0)
            {
                newDiary.Id = ctDiaryEdit.Id;
            }
            Database.SaveDiary(newDiary);
            Android.Widget.Toast.MakeText(this, "Diary Saved", Android.Widget.ToastLength.Long).Show();

            Intent intent2 = new Intent(this, typeof(DiaryListActivity));
            StartActivity(intent2);
        }

        private void OnStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            EditText txtPrefDate = FindViewById<EditText>(Resource.Id.DiaryCreatedDate);
            txtPrefDate.Text = e.Date.ToShortDateString();
            return;
        }
    }
}
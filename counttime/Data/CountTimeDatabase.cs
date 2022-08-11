using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using counttime.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace counttime.Data
{
    public class CountTimeDatabase
    {
        readonly SQLiteConnection database;

        public CountTimeDatabase(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            database.CreateTable<Profile>();
            database.CreateTable<Event>();
            database.CreateTable<EventEntry>();
            database.CreateTable<Diary>();
        }
        public List<Profile> GetProfiles()
        {
            //Get all notes.
            return database.Table<Profile>().ToList();
        }
        public Profile GetProfile(int id)
        {
            // Get a specific note.
            return database.Table<Profile>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public int SaveProfile(Profile profile)
        {
            if (profile.Id != 0)
            {
                // Update an existing note.
                return database.Update(profile);
            }
            else
            {
                // Save a new note.
                return database.Insert(profile);
            }
        }
        public int DeleteProfile(Profile profile)
        {
            // Delete a note.
            return database.Delete(profile);
        }
    }
}
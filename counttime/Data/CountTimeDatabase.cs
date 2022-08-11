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
        readonly SQLiteAsyncConnection database;

        public CountTimeDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Profile>().Wait();
            database.CreateTableAsync<Event>().Wait();
            database.CreateTableAsync<EventEntry>().Wait();
            database.CreateTableAsync<Diary>().Wait();
        }
        public Task<List<Profile>> GetProfilesAsync()
        {
            //Get all notes.
            return database.Table<Profile>().ToListAsync();
        }
        public Task<Profile> GetProfileAsync(int id)
        {
            // Get a specific note.
            return database.Table<Profile>()
                            .Where(i => i.Id == id)
                            .FirstOrDefaultAsync();
        }
        public Task<int> SaveProfileAsync(Profile profile)
        {
            if (profile.Id != 0)
            {
                // Update an existing note.
                return database.UpdateAsync(profile);
            }
            else
            {
                // Save a new note.
                return database.InsertAsync(profile);
            }
        }
        public Task<int> DeleteProfileAsync(Profile profile)
        {
            // Delete a note.
            return database.DeleteAsync(profile);
        }
    }
}
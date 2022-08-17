using counttime.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

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
            database.CreateTable<Diary>();
        }
        public List<Event> GetHomeScreenEvents()
        {
            var results = database.Table<Event>().ToList();
            return results.Where(e => e.IsShowOnHomeScreen == true
             && DateTime.Now >= e.EventStartDate
             && e.EventEndDate >= DateTime.Now).ToList();
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

        public List<Event> GetEvents()
        {
            return database.Table<Event>().ToList();
        }
        public Event GetEvent(int id)
        {
            return database.Table<Event>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public int SaveEvent(Event ctEvent)
        {
            if (ctEvent.Id != 0)
            {
                return database.Update(ctEvent);
            }
            else
            {
                return database.Insert(ctEvent);
            }
        }
        public int DeleteEvent(Event ctEvent)
        {
            // Delete a note.
            return database.Delete(ctEvent);
        }

        public List<Diary> GetDiarys()
        {
            return database.Table<Diary>().ToList();
        }
        public Diary GetDiary(int id)
        {
            return database.Table<Diary>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public int SaveDiary(Diary ctDiary)
        {
            if (ctDiary.Id != 0)
            {
                return database.Update(ctDiary);
            }
            else
            {
                return database.Insert(ctDiary);
            }
        }
        public int DeleteDiary(Diary ctDiary)
        {
            // Delete a note.
            return database.Delete(ctDiary);
        }
    }
}
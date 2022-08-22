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
            database.CreateTable<Incident>();
            database.CreateTable<Location>();
            database.CreateTable<WorkAssignment>();
        }
        public List<HistoryViewModel> GetHistory()
        {
            var history = database.Query<HistoryViewModel>("SELECT Name as 'Name', ArrivalDate As 'Date' FROM Location Union select Name As 'Name', IncidentDate as 'Date' from Incident Union select Name as 'Name', StartDate as 'Date' from WorkAssignment Union select Name, EventStartDate as 'Date' from Event");
            return history.ToList();

        }
        public Event GetNearestFutureMilestone()
        {
            var results = database.Table<Event>().ToList();
            return results.Where(e => DateTime.Now <= e.EventStartDate && e.EventType == "Milestone").OrderBy(e => e.EventStartDate).FirstOrDefault();
        }
        public Event GetNearestPastMilestone()
        {
            var results = database.Table<Event>().ToList();
            return results.Where(e => DateTime.Now >= e.EventStartDate && e.EventType == "Milestone").OrderByDescending(e => e.EventStartDate).FirstOrDefault();
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

        public List<Incident> GetIncidents()
        {
            return database.Table<Incident>().ToList();
        }
        public Incident GetIncident(int id)
        {
            return database.Table<Incident>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public int SaveIncident(Incident ctIncident)
        {
            if (ctIncident.Id != 0)
            {
                return database.Update(ctIncident);
            }
            else
            {
                return database.Insert(ctIncident);
            }
        }
        public int DeleteIncident(Incident ctIncident)
        {
            // Delete a note.
            return database.Delete(ctIncident);
        }

        public List<Location> GetLocations()
        {
            return database.Table<Location>().ToList();
        }
        public Location GetLocation(int id)
        {
            return database.Table<Location>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public int SaveLocation(Location ctLocation)
        {
            if (ctLocation.Id != 0)
            {
                return database.Update(ctLocation);
            }
            else
            {
                return database.Insert(ctLocation);
            }
        }
        public int DeleteLocation(Location ctLocation)
        {
            // Delete a note.
            return database.Delete(ctLocation);
        }

        public List<WorkAssignment> GetWorkAssignments()
        {
            return database.Table<WorkAssignment>().ToList();
        }
        public WorkAssignment GetWorkAssignment(int id)
        {
            return database.Table<WorkAssignment>()
                            .Where(i => i.Id == id)
                            .FirstOrDefault();
        }
        public int SaveWorkAssignment(WorkAssignment ctWorkAssignment)
        {
            if (ctWorkAssignment.Id != 0)
            {
                return database.Update(ctWorkAssignment);
            }
            else
            {
                return database.Insert(ctWorkAssignment);
            }
        }
        public int DeleteWorkAssignment(WorkAssignment ctWorkAssignment)
        {
            // Delete a note.
            return database.Delete(ctWorkAssignment);
        }
    }
}
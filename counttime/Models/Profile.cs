﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace counttime.Models
{
    public class Profile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int TimeServedDays { get; set; }
        public int SentenceLengthMonths { get; set; }
        public DateTime? SentenceDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ProjectedReleaseDate { get; set; }
        public int? LockdownEventId { get; set; }

        public bool IsSentenceDateEnabled { get; set; }
        public bool IsOnLockdown { get; set; }
    }
}
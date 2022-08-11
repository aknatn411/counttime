using Android.App;
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
        public int? SentenceDateId { get; set; }
        public int? StartDateId { get; set; }
        public int? EndDateId { get; set; }
        public int? ProjectedReleaseDateId { get; set; }
    }
}
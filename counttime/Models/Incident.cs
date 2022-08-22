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
    public class Incident
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public DateTime IncidentDate { get; set; }
        public int DaysLost { get; set; }
        public int UserId { get; set; }
    }
}
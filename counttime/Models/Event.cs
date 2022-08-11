﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace counttime.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProfileId { get; set; }
        public int TypeId { get; set; }
        public string Details { get; set; }
        public bool IsEditable { get; set; }
        public bool IsSystem { get; set; }
    }
}
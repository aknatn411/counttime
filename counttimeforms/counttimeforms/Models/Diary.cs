using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace counttimeforms.Models
{
    public class Diary
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

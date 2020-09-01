using SQLite;
using System;

namespace Bare10.Models.Walking
{
    public class LastWalkingQuery
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}

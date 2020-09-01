using SQLite;
using System;

namespace Bare10.Models
{
    /// <summary>
    /// Represents a single day in a history of whether or not a goal was reached
    /// </summary>
    public class GoalStorageModel
    {
        /// <summary>
        /// Which day (at midnight the achievement was unlocked)
        /// </summary>
        [PrimaryKey]
        public DateTime Day { get; set; }
        /// <summary>
        /// This is the enum value for the goal. Not used as primary key as it is NOT unique
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Whether or not the goal has been reached
        /// </summary>
        public bool HasBeenReached { get; set; }
        /// <summary>
        /// How many minutes it takes to reach this goal
        /// </summary>
        public uint MinutesToReach { get; set; }


        public override string ToString()
        {
            return Day.ToShortDateString() + ": 10 x " + MinutesToReach / 10 + " : " + HasBeenReached;
        }
    }
}

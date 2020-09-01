using SQLite;
using System;

namespace Bare10.Models.Walking
{
    /// <summary>
    /// Represents the history for a single day
    /// </summary>
    public class WalkingDayModel 
    {
        [PrimaryKey]
        public DateTime Day { get; set; }
        public uint MinutesBriskWalking { get; set; }
        public uint MinutesRegularWalking { get; set; }
        public uint MinutesUnknownWalking { get; set; }

        public override bool Equals(object obj)
        {
            if(!(obj is WalkingDayModel day))
            {
                return false;
            }
            return Day == day.Day;
        }

        public override int GetHashCode()
        {
            return Day.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Day.ToShortDateString()}: {MinutesBriskWalking}(B) / {MinutesRegularWalking} / {MinutesUnknownWalking} (U)";

        }
    }
}

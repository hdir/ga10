using SQLite;
using System;

namespace Bare10.Models.Walking
{
    public class WalkingDataPointModel : IEquatable<WalkingDataPointModel>
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public float AverageSpeedMetersPerSecond { get; set; }
        public bool WasBrisk { get; set; }
        public bool WasUnknown { get; set; }
        public bool HasBeenCounted { get; set; }

        public WalkingDataPointModel()
        {

        }
               
        public WalkingDataPointModel(DateTime start, DateTime stop, float averageSpeedMetersPerSecond)
        {
            Start = start;
            Stop = stop;
            AverageSpeedMetersPerSecond = averageSpeedMetersPerSecond;
            HasBeenCounted = false;

            UpdateBriskness();
        }

        public WalkingDataPointModel(DateTime start, DateTime stop, bool wasUnknown = true)
        {
            Start = start;
            Stop = stop;
            WasUnknown = wasUnknown;
            HasBeenCounted = false;
        }

        public bool Equals(WalkingDataPointModel other)
        {
            return (
                Start == other.Start &&
                Stop ==  other.Stop &&
                AverageSpeedMetersPerSecond == other.AverageSpeedMetersPerSecond &&
                WasBrisk == other.WasBrisk &&
                WasUnknown == other.WasUnknown);
        }

        public void UpdateBriskness()
        {
            WasBrisk = AverageSpeedMetersPerSecond >= Config.BriskPaceMetersPerSecond;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}: {2}m/s, {3}, {4}",
                Start.ToShortTimeString(),
                Stop.ToShortTimeString(),
                AverageSpeedMetersPerSecond.ToString("0.00"),
                ( WasUnknown ? "Unknown" : (WasBrisk ? "Brisk" : "Regular")),
                (HasBeenCounted ? string.Empty : " Counted"));
        }
    }
}

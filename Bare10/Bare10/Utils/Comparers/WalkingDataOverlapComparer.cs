using Bare10.Models.Walking;
using Bare10.Utils.Time;
using System.Collections.Generic;
using static Bare10.Utils.Time.DateTimeRange;

namespace Bare10.Utils.Comparers
{
    public class WalkingDataOverlapComparer : IEqualityComparer<WalkingDataPointModel>
    {
        public bool Equals(WalkingDataPointModel x, WalkingDataPointModel y)
        {
            var xRange = new DateTimeRange(x.Start, x.Stop);
            var yRange = new DateTimeRange(y.Start, y.Stop);

            return xRange.GetOverlapType(yRange) != OverlapType.None;
        }

        public int GetHashCode(WalkingDataPointModel obj)
        {
            return obj.Start.GetHashCode() ^ obj.Stop.GetHashCode();
        }
    }
}

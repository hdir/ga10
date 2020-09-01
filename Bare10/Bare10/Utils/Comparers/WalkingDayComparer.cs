using Bare10.Models.Walking;
using System.Collections.Generic;

namespace Bare10.Utils.Comparers
{
    public class WalkingDayComparer : IEqualityComparer<WalkingDayModel>
    {
        public bool Equals(WalkingDayModel x, WalkingDayModel y)
        {
            return x.Day == y.Day;
        }

        public int GetHashCode(WalkingDayModel obj)
        {
            return obj.Day.GetHashCode();
            //return obj.GetHashCode();
        }
    }
}

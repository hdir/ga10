using System.Collections.Generic;
using System.Text;

namespace Bare10.Models.Walking
{
    public class TodaysWalkingModel
    {
        public List<WalkingDataPointModel> todaysWalking = new List<WalkingDataPointModel>();
        public uint minutesRegularWalkToday = 0;
        public uint minutesBriskWalkToday = 0;
        public uint minutesUnknownWalkToday = 0;

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Todays data points:");
            foreach(var dp in todaysWalking)
            {
                sb.AppendLine("\t" + dp);
            }

            sb.AppendLine("Total minutes regular walk today: " + minutesRegularWalkToday);
            sb.AppendLine("Total minutes brisk walk today: " + minutesBriskWalkToday);

            return sb.ToString();
        }
    }
}

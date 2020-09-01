using System;

namespace Bare10.Utils.Time
{
    public class DateTimeRange : IEquatable<DateTimeRange>
    {
        /// <summary>
        /// A After B
        /// *----B-----*
        ///     *-----A-----*
        ///     
        /// A Before B
        /// *----A-----*
        ///     *-----B-----*
        /// 
        /// A_Contains_B
        /// *----A-----*
        ///  *--B--*
        ///  
        /// A Contained by B
        ///  *--A--*
        /// *----B-----*
        /// </summary>
        public enum OverlapType
        {
            None,
            After,
            Before,
            ContainsOther,
            ContainedByOther,
        };

        public struct OverlapResult
        {
            public OverlapType type;
            public double overlapThis;
            public double overlapOther;
        }

        public readonly DateTime Start;
        public readonly DateTime Stop;

        public TimeSpan Span { get { return Stop - Start; } }
        public double TotalSeconds { get { return Span.TotalSeconds; } }

        

        public DateTimeRange(DateTime start, DateTime stop)
        {
            Start = start;
            Stop = stop;
        }
        
        public override string ToString()
        {
            return Start.ToShortTimeString() + " - " + Stop.ToShortTimeString();
        }

        public bool Equals(DateTimeRange other)
        {
            return Start == other.Start && Stop == other.Stop;
        }

        /// <summary>
        /// Checking what kind of overlap there is
        /// </summary>
        /// <param name="other">The other DateTimeRange</param>
        /// <returns>The type of overlap</returns>
        public OverlapType GetOverlapType(DateTimeRange other)
        {
            if (Start <= other.Start)
            {
                if (Stop <= other.Stop)
                {
                    return (Stop >= other.Start) ? OverlapType.Before : OverlapType.None;
                }
                else
                {
                    return OverlapType.ContainsOther;
                }
            }
            else
            {
                if (other.Stop <= Stop)
                {
                    return (other.Stop >= Start) ? OverlapType.After : OverlapType.None;
                }
                else
                {
                    return OverlapType.ContainedByOther;
                }
            }
        }

        /// <summary>
        /// Checks how much of this instance is covered by the other Range
        /// </summary>
        /// <param name="other">The DateTimeRange to compare overlap with</param>
        /// <returns>The normalized amount of this instance covered by the other DateTimeRange</returns>
        public double OverlapAmount(DateTimeRange other)
        {
            switch (GetOverlapType(other))
            {
                case OverlapType.None:
                    {
                        return 0;
                    }
                case OverlapType.ContainsOther:
                    {
                        
                        if (Span.TotalSeconds == 0)
                            return 0;

                        return other.Span.TotalSeconds / Span.TotalSeconds;
                    }
                case OverlapType.ContainedByOther:
                    {
                        return 1;
                    }
                case OverlapType.After:
                    {
                        if (Span.TotalSeconds == 0)
                        {
                            return 0;
                        }

                        var overlap = other.Stop - Start;
                        return overlap.TotalSeconds / Span.TotalSeconds;
                    }
                case OverlapType.Before:
                    {
                        if (Span.TotalSeconds == 0)
                        {
                            return 0;
                        }

                        var overlap = Stop - other.Start;
                        return overlap.TotalSeconds / Span.TotalSeconds;
                    }
            }
            return 0;
        }

        /// <summary>
        /// Efficiently get an overlap comparison with another DateTimeRange
        /// </summary>
        /// <param name="other"></param>
        /// <returns>The type of overlap, with normalized overlap ranges</returns>
        public OverlapResult GetOverlap(DateTimeRange other)
        {
            OverlapResult result = new OverlapResult();
            if (Start <= other.Start)
            {
                if (Stop <= other.Stop)
                {
                    if(Stop >= other.Start)
                    {
                        result.type = OverlapType.Before;
                        var overlap = (Stop - other.Start).TotalMilliseconds;
                        if(Span.TotalMilliseconds > 0)
                        {
                            result.overlapThis = overlap / Span.TotalMilliseconds;
                        }
                        if(other.Span.TotalMilliseconds > 0)
                        {
                            result.overlapOther = overlap / other.Span.TotalMilliseconds;
                        }
                    }
                }
                else
                {
                    result.type = OverlapType.ContainsOther;
                    if(Span.TotalMilliseconds > 0)
                    {
                        result.overlapThis = other.Span.TotalMilliseconds / Span.TotalMilliseconds;
                        result.overlapOther = 1;
                    }
                }
            }
            else
            {
                if (other.Stop <= Stop)
                {
                    if(other.Stop >= Start)
                    {
                        result.type = OverlapType.After;
                        var overlap = (other.Stop - Start).TotalMilliseconds;
                        if(Span.TotalMilliseconds > 0)
                        {
                            result.overlapThis =  overlap / Span.TotalMilliseconds;
                        }
                        if(other.Span.TotalMilliseconds > 0)
                        {
                            result.overlapOther = overlap / other.Span.TotalMilliseconds;
                        }
                    }
                }
                else
                {
                    result.type = OverlapType.ContainedByOther;
                    result.overlapThis = 1;
                    if(other.Span.TotalMilliseconds > 0)
                    {
                        result.overlapOther = Span.TotalMilliseconds / other.Span.TotalMilliseconds;
                    }
                }
            }
            return result;
        }
    }
}

using System;

namespace Bare10.Utils
{
    public static class DateTimeUtils
    {
        public static int DateTimeToDayOfWeekIndexInNorwegian(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 0;
                case DayOfWeek.Tuesday:
                    return 1;
                case DayOfWeek.Wednesday:
                    return 2;
                case DayOfWeek.Thursday:
                    return 3;
                case DayOfWeek.Friday:
                    return 4;
                case DayOfWeek.Saturday:
                    return 5;
                case DayOfWeek.Sunday:
                    return 6;
                default:
                    throw new Exception("No such day of week");
            }
        }

        public static string DayOfWeekToShortName(DayOfWeek dt)
        {
            switch (dt)
            {
                case DayOfWeek.Monday:
                    return "MA";
                case DayOfWeek.Tuesday:
                    return "TI";
                case DayOfWeek.Wednesday:
                    return "ON";
                case DayOfWeek.Thursday:
                    return "TO";
                case DayOfWeek.Friday:
                    return "FR";
                case DayOfWeek.Saturday:
                    return "LØ";
                case DayOfWeek.Sunday:
                    return "SØ";
                default:
                    throw new Exception("No such day of week");
            }
        }

        public static string InNorwegian(this DayOfWeek day)
        {
            switch(day)
            {
                case DayOfWeek.Monday: return "Mandag";
                case DayOfWeek.Tuesday: return "Tirsdag";
                case DayOfWeek.Wednesday: return "Onsdag";
                case DayOfWeek.Thursday: return "Torsdag";
                case DayOfWeek.Friday: return "Fredag";
                case DayOfWeek.Saturday: return "Lørdag";
                case DayOfWeek.Sunday: return "Søndag";
                default: throw new Exception("Feil ukesdagverdi");
            }

        }
    }
}

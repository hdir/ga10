using System;

namespace Bare10.Utils.Enum
{
    public static class EnumBitflagExtensions
    {
        public static T Set<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type | (int)(object)value);
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not set value from enum type {0}",
                        typeof(T).Name
                        ),
                    e);
            }
        }

        public static T Unset<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type & ~(int)(object)value);
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not unset value from enum type {0}",
                        typeof(T).Name
                        ),
                    e);
            }
        }

        public static T Toggle<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type ^ (int)(object)value);
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not toggle value from enum type {0}",
                        typeof(T).Name
                        ),
                    e);
            }
        }

    }
}

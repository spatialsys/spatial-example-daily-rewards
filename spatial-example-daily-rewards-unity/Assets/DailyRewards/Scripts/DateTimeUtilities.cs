using System;

namespace DailyRewards
{
    public class DateTimeUtilities
    {
        public static int GetMinutePassed(DateTime dateTime)
        {
            return (int)(DateTime.Now - dateTime).TotalMinutes;
        }

        // Apply offsetHour so that the day starts at the specified hour
        public static int GetDayPassed(DateTime dateTime, int offsetHour)
        {
            return GetDayPassed(DateTime.Now, dateTime, offsetHour);
        }
        public static int GetDayPassed(DateTime now, DateTime dateTime, int offsetHour)
        {
            DateTime nowOffset = now.AddHours(offsetHour);
            DateTime dateTimeOffset = dateTime.AddHours(offsetHour);
            return nowOffset.Day - dateTimeOffset.Day;
        }

        public static int SecondsToNextDay(DateTime dateTime, int offsetHour)
        {
            DateTime dateTimeOffset = dateTime.AddHours(offsetHour);
            DateTime nextDay = dateTimeOffset.AddDays(1);
            nextDay = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
            return (int)(nextDay - dateTimeOffset).TotalSeconds;
        }

        // Week starts at resetDay and specified hour
        public static bool IsWeekChanged(DateTime dateTime, int offsetHour, DayOfWeek resetDay)
        {
            return IsWeekChanged(DateTime.Now, dateTime, offsetHour, resetDay);
        }
        public static bool IsWeekChanged(DateTime now, DateTime dateTime, int offsetHour, DayOfWeek resetDay)
        {
            DateTime nowOffset = now.AddHours(offsetHour);
            nowOffset = nowOffset.AddDays(-(int)resetDay);

            DateTime dateTimeOffset = dateTime.AddHours(offsetHour);
            dateTimeOffset = dateTimeOffset.AddDays(-(int)resetDay);

            int dayDiff = GetDayPassed(now, dateTime, offsetHour);
            if (dayDiff >= 7)
            {
                return true;
            }
            else if (dateTimeOffset.DayOfWeek > nowOffset.DayOfWeek)
            {
                return true;
            }

            return false;
        }

        // return given dateTime is the new day of the week
        public static bool IsNewDayOfWeek(DateTime dateTime, int offsetHour, DayOfWeek resetDay)
        {
            DateTime dateTimeOffset = dateTime.AddHours(offsetHour);
            return dateTimeOffset.DayOfWeek == resetDay;
        }

        public static int SecondsToNextWeek(DateTime dateTime, int offsetHour, DayOfWeek resetDay)
        {
            DateTime dateTimeOffset = dateTime.AddHours(offsetHour);
            int daysToNextWeek = (int)resetDay - (int)dateTimeOffset.DayOfWeek;
            if (daysToNextWeek < 0)
            {
                daysToNextWeek += 7;
            }
            DateTime nextWeek = dateTimeOffset.AddDays(daysToNextWeek);
            nextWeek = new DateTime(nextWeek.Year, nextWeek.Month, nextWeek.Day, 0, 0, 0);
            return (int)(nextWeek - dateTimeOffset).TotalSeconds;
        }
    }
}
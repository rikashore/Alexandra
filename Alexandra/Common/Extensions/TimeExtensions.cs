using System;
using System.Text;

namespace Alexandra.Common.Extensions
{
    public static class TimeExtensions
    {
        private const string DayFormat = "%d' day '";
        private const string DaysFormat = "%d' days '";
        private const string HourFormat = "%h' hour '";
        private const string HoursFormat = "%h' hours '";
        private const string MinuteFormat = "%m' min '";
        private const string MinutesFormat = "%m' mins '";
        private const string SecondFormat = "%s' sec '";
        private const string SecondsFormat = "%s' secs '";
        
        private const string TodayFormatString = "h:mm tt";
        private const string DateFormatString = "dd/MM/yy a\\t h:mm tt";


        //simple formatter for timespans to avoid:  0 days 0 hours 0 minutes 30 seconds
        //instead it will produce:  30 seconds
        public static string Humanize(this TimeSpan ts)
        {
            //"%d' days '%h' hours '%m' mins '%s' secs'"
            var formatBuilder = new StringBuilder();

            switch (ts.Days)
            {
                case 1:
                    formatBuilder.Append(DayFormat);
                    break;
                case > 1:
                    formatBuilder.Append(DaysFormat);
                    break;
            }

            switch (ts.Hours)
            {
                case 1:
                    formatBuilder.Append(HourFormat);
                    break;
                case > 1:
                    formatBuilder.Append(HoursFormat);
                    break;
            }

            switch (ts.Minutes)
            {
                case 1:
                    formatBuilder.Append(MinuteFormat);
                    break;
                case > 1:
                    formatBuilder.Append(MinutesFormat);
                    break;
            }

            switch (ts.Seconds)
            {
                case 1:
                    formatBuilder.Append(SecondFormat);
                    break;
                case > 1:
                    formatBuilder.Append(SecondsFormat);
                    break;
            }

            return ts.ToString(formatBuilder.ToString());
        }

        public static string Humanize(this DateTime dt)
            => dt.ToString(dt.Date == DateTime.Today ? TodayFormatString : DateFormatString);
    }
}
using System;

namespace Core
{
    /// <summary>
    /// A Helper class providing useful string functions.
    /// </summary>
    public class StringUtil
    {
        #region Public Methods

        /// <summary>
        /// Formats a number to include commas.  Additionally provides an option to present the number in terms of thousands in order to shorten it.
        /// </summary>
        /// <returns>The long as a string.</returns>
        /// <param name="value">The number value to format.</param>
        /// <param name="shorten">Provides an option to present the number in terms of thousands in order to shorten it..</param>
        public static string FormatNumber(long value, bool shorten)
        {
            if ((!shorten) || (value < 1000))
            {
                return string.Format(CurrentCultureInfo, "{0:n0}", value);
            }

            return string.Format("{0:n0}", (value / 1000)) + "k";
        }

        /// <summary>
        /// Formats a number to include commas.  Additionally provides an option to present the number in terms of thousands in order to shorten it.
        /// </summary>
        /// <returns>The int as a string.</returns>
        /// <param name="value">The number value to format.</param>
        /// <param name="shorten">Provides an option to present the number in terms of thousands in order to shorten it..</param>
        public static string FormatNumber(int value, bool shorten)
        {
            return FormatNumber((long)value, shorten);
        }

        /// <summary>
        /// Formats a number to include commas.  Additionally provides an option to present the number in terms of thousands in order to shorten it.
        /// </summary>
        /// <returns>The float as a string, truncated to integer precision.</returns>
        /// <param name="value">The number value to format.</param>
        /// <param name="shorten">Provides an option to present the number in terms of thousands in order to shorten it..</param>
        public static string FormatNumber(float value, bool shorten)
        {
            return FormatNumber((long)value, shorten);
        }

        /// <summary>
        /// Formats a float according to the precision. Ignores precision if value is whole
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static string FormatNumber(float value, uint precision)
        {
            if (precision == 0)
            {
                return FormatNumber((long)value, false);
            }

            float abs = Math.Abs(value);
            if ((int)((abs - (int)abs) * Math.Pow(10, precision)) > 0)
            {
                string format = "{0:n" + precision + "}";
                return string.Format(CurrentCultureInfo, format, value);
            }

            return FormatNumber((long)value, false);
        }

        /// <summary>
        /// Formats a TimeSpan as: [days]:[hours]:[minutes]:[seconds]
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="includeDays"></param>
        /// <param name="ceilingSeconds"></param>
        /// <returns>Formatted TimeSpan</returns>
        public static string FormatTimeSpan(TimeSpan timeSpan, bool includeDays = true, bool ceilingSeconds = false)
        {
            if (ceilingSeconds)
            {
                timeSpan = TimeSpan.FromSeconds(Math.Ceiling(timeSpan.TotalSeconds));
            }

            if (includeDays)
            {
                return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
        }

        /// <summary>
        /// Gets the culture info for the selected language, for the sake of formatting numbers, etc.
        /// </summary>
        public static System.Globalization.CultureInfo CurrentCultureInfo
        {
            get
            {
                return System.Globalization.CultureInfo.GetCultureInfo("en-US");
            }
        }
        #endregion
    }
}
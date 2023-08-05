using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLMatchResultConsole.Common
{
    internal static class Formatting
    {

        public static string FormatDateTimeFull(DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("ddd, dd/MM/yyyy HH:mm");
        }

        public static string FormatDateTimeHourMinute(DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString("HH:mm");
        }
    }
}

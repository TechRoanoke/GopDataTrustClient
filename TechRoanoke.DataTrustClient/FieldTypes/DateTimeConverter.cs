using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TechRoanoke.DataTrustClient
{
    public class DateTimeConverter
    {
        // Convert from DataTrust date format into C# format. 
        public static DateTime ConvertToDate(string val)
        {
            // YYYYMMDD or YYYYMM or YYYY
            if (val.Length == 8)
            {
                var date = DateTime.ParseExact(val, "yyyyMMdd", CultureInfo.InvariantCulture);
                return date;
            }
            if (val.Length == 6)
            {
                var date = DateTime.ParseExact(val, "yyyyMM", CultureInfo.InvariantCulture);
                return date;
            }
            if (val.Length == 4)
            {
                var date = DateTime.ParseExact(val, "yyyy", CultureInfo.InvariantCulture);
                return date;
            }
            return DateTime.Parse(val);
        }

        public static string ToDbValue(int year)
        {
            return string.Format("'{0}'", year);
        }

        public static string ToDbValue(int year, int month)
        {
            return string.Format("'{0}{1:00}'", year, month);
        }

        public static string ToDbValue(int year, int month, int day)
        {
            return string.Format("'{0}{1:00}{2:00}'", year, month, day);
        }

        public static string ToDbValue(DateTime date)
        {
            if (date.Day == 1)
            {
                if (date.Month == 1)
                {
                    return ToDbValue(date.Year);
                }
                return ToDbValue(date.Year, date.Month);
            }
            return ToDbValue(date.Year, date.Month, date.Day);
        }
    }
}
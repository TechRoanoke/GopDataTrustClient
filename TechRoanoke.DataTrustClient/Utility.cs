using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Globalization;

namespace TechRoanoke.DataTrustClient
{
    public static class Utility
    {
        // Get all the property names on type T.
        public static string[] GetPropertyNames(Type t)
        {
            var it = t.GetTypeInfo();
            var props = it.DeclaredProperties;
            var x = from prop in props select prop.Name;
            return x.ToArray();
        }

        // Convert a JToken into a strongly-typed object of type TSelect.
        public static TSelect Convert<TSelect>(JToken jt) where TSelect : new()
        {
            var dd = new TSelect();
            JObject jobj = (JObject)jt;

            var it = typeof(TSelect).GetTypeInfo();
            
            foreach (var prop in it.DeclaredProperties)
            {
                var name = prop.Name;
                var value = jobj[name];

                var v2 = Coerce(prop.PropertyType, value);
                prop.SetValue(dd, v2);
            }

            return dd;
        }

        // Coerce a JToken into the targetType. 
        private static object Coerce(Type targetType, JToken value)
        {
            var val = value.ToString();

            var it = targetType.GetTypeInfo();

            if (it.IsEnum)
            {
                // handles both integers and string values. 
                return Enum.Parse(targetType, val);
            }

            if (targetType == typeof(int))
            {                
                return int.Parse(val);
            }
            if (targetType == typeof(string))
            {
                return val;
            }
            if (targetType == typeof(DateTime))
            {
                return CoerceDate(val);
            }
            return val;
        }

        // Convert from DataTrust date format into C# format. 
        private static DateTime CoerceDate(string val)
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

        // Convert a JToken into an IDictionary.
        public static IDictionary<string, string> Convert(JToken jt)
        {
            var dd = new Dictionary<string, string>();

            JObject jobj = (JObject)jt;

            foreach (var prop in jobj.Properties())
            {
                var name = prop.Name;
                var value = prop.Value;
                dd[name] = value == null ? null : value.ToString();
            }
            return dd;
        }
    }
}
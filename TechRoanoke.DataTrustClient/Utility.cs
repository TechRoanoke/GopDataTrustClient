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

        // Get Select names from a type.
        // This can apply conventions / attributes:
        // int Count_X ==> count(x). This matches the convention that GDT returns count(x) in. 
        public static string[] GetSelectNamesFromType(Type t)
        {
            var it = t.GetTypeInfo();
            var props = it.DeclaredProperties.ToArray();

            int len = props.Length;
            string[] names = new string[len];
            for (int i = 0; i < len; i++)
            {
                string name = props[i].Name;
                if (name.StartsWith("count_"))
                {
                    if (props[i].PropertyType == typeof(int))
                    {
                        string realName = name.Substring(6);
                        name = string.Format("count({0})", realName);
                    }                    
                }
                names[i] = name;
            }

            return names;
        }

        // Convert a JObject into a strongly-typed object of type TSelect.
        public static TSelect Convert<TSelect>(JObject jobj) where TSelect : new()
        {
            var dd = new TSelect();
            
            var it = typeof(TSelect).GetTypeInfo();
            
            // Casing issue - jobj is the actual results from the database. TSelect may have wrong casing. 
            foreach (var prop in it.DeclaredProperties)
            {
                var name = prop.Name;
                JToken jt;
                if (jobj.TryGetValue(name, StringComparison.OrdinalIgnoreCase, out jt))
                {
                    var value = jt.ToString();
                    var v2 = Coerce(prop.PropertyType, value);
                    prop.SetValue(dd, v2);
                }
            }

            return dd;
        }

        // Coerce a JObject into the targetType. 
        internal static object Coerce(Type targetType, JToken value)
        {
            var val = value.ToString();
            return DbConverter.ConvertToObject(targetType, val);
        }

      
        // Convert a JObject into an IDictionary.
        public static IDictionary<string, string> Convert(JObject jobj)
        {
            var dd = new Dictionary<string, string>();
                        
            foreach (var prop in jobj.Properties())
            {
                var name = NormalizeKey(prop.Name);
                var value = prop.Value;
                dd[name] = value == null ? null : value.ToString();
            }
            return dd;
        }

        public static TSelect[] ConvertToObj<TSelect>(JObject[] results) where TSelect : new()
        {
            int len = results.Length;
            TSelect[] dd = new TSelect[len];
            for (int i = 0; i < len; i++)
            {
                dd[i] = Utility.Convert<TSelect>(results[i]);
            }

            return dd;
        }

        public static IDictionary<string, string>[] ConvertToDict(JObject[] results)
        {
            // Convert results back to dictionary 
            int len = results.Length;
            IDictionary<string, string>[] dd = new IDictionary<string, string>[len];
            for (int i = 0; i < len; i++)
            {
                dd[i] = Utility.Convert(results[i]);
            }

            return dd;
        }

        public static IDictionary<string, string[]> ConvertToTable(JObject[] results)
        {
            int len = results.Length;
            IDictionary<string, string[]> dd = new Dictionary<string, string[]>();

            for (int i = 0; i < len; i++)
            {
                var jobj = results[i];
                foreach (var prop in jobj.Properties())
                {
                    var name = NormalizeKey(prop.Name);
                    var value = prop.Value;

                    string[] column;
                    if (!dd.TryGetValue(name, out column))
                    {
                        column = new string[len];
                        dd[name] = column;
                    }

                    column[i] = (value == null) ? null : value.ToString(); 
                }
            }

            return dd;
        }

        public static string NormalizeKey(string key)
        {
            return key.ToLower();
        }
    }
}
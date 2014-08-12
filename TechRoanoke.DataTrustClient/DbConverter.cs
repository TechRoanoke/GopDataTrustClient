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
    // Convert C# types into DbValue objects. 
    public class DbConverter
    {
        public static readonly Dictionary<Type, ConvertEntry> Converters;

        static DbConverter()
        {
            Converters = new Dictionary<Type, ConvertEntry>();
            Init();
        }

        public static void Add<T>(Func<T, string> funcToDbValue, Func<string, T> funcToObj)
        {
            var t = typeof(T);
            Converters[t] = new ConvertEntry<T>(funcToDbValue, funcToObj);
        }

        public static string ConvertToDbValue<T>(T value)
        {
            var c = Converters[typeof(T)];
            var cc = (ConvertEntry<T>)c;
            return cc._funcToDbValue(value);
        }

        public static object ConvertToObject(Type targetType, string val)
        {
            ConvertEntry c;

            if (Converters.TryGetValue(targetType, out c))
            {
                var result = c.ConvertToObject(val);
                return result;
            }

            // Is it an enum?
            // There is an infinite number of enum types. 
            var it = targetType.GetTypeInfo();
            if (it.IsEnum)
            {
                // handles both integers and string values. 
                var result = Enum.Parse(targetType, val);
                return result;
            }

            string msg = string.Format("Can't convert '{0}' to type {1}", val, targetType.Name);
            throw new InvalidOperationException(msg);
        }

        public static T[] ConvertToObject<T>(string[] val)
        {
            int len = val.Length;
            T[] array = new T[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = ConvertToObject<T>(val[i]);
            }
            return array;
        }

        public static T ConvertToObject<T>(string val)
        {
            var result = ConvertToObject(typeof(T), val);
            return (T)result;
        }

        public static void Init()
        {
            // Built-ins
            Add<int>(x => x.ToString(), int.Parse);
            Add<string>(x => Quote(x), x => x);

            Field.AddCustomConverters();
        }

        // Convert when C# type is an integer, but DB Value is a string. 
        public static string IntToQuotedString(int i)
        {
            return Quote(i);
        }

        public static string Quote(object o)
        {
            return string.Format("'{0}'", o);
        }
    }

}
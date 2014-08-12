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
    public abstract class ConvertEntry
    {
        public abstract string ConvertToDbValue(object obj);
        public abstract object ConvertToObject(string val);
    }

    public class ConvertEntry<T> : ConvertEntry
    {
        public readonly Func<T, string> _funcToDbValue;
        public readonly Func<string, T> _funcToObj;

        public ConvertEntry(Func<T, string> funcToDbValue, Func<string, T> funcToObj)
        {
            _funcToDbValue = funcToDbValue;
            _funcToObj = funcToObj;
        }

        public override string ConvertToDbValue(object obj)
        {
            return _funcToDbValue((T)obj);
        }

        public override object ConvertToObject(string val)
        {
            return _funcToObj(val);
        }
    }
}
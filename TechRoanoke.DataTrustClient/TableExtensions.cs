using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechRoanoke.DataTrustClient
{
    // Helpers for treating a IDictionary<string,string[]> as a table. 
    public static class TableExtensions
    {
        public static int GetRowCount(this IDictionary<string, string[]> table)
        {
            return table.First().Value.Length;
        }

        public static string[] GetColumn(this IDictionary<string, string[]> table, string fieldName)
        {
            return table[fieldName];
        }

        // Used when table should only have 1 column, like with Select Distinct
        public static string[] GetSingleColumn(this IDictionary<string, string[]> table)
        {
            if (table.Count != 1)
            {
                string msg = string.Format("Table should only have 1 column. It has: {0}", string.Join(",", table.Keys.ToArray()));
                throw new InvalidOperationException(msg);
            }
            return table.First().Value;
        }

        // Used when a table should only have 1 value. Like Select count(). 
        public static string GetSingleValue(this IDictionary<string, string[]> table)
        {
            var column = table.GetSingleColumn();
            if (column.Length != 1)
            {
                string msg = string.Format("Table should only have 1 value. It has: {0}", column.Length);
                throw new InvalidOperationException(msg);
            }
            return column[0];
        }


        // Get a specific row from the dictionary 
        public static IDictionary<string, string> GetRow(this IDictionary<string, string[]> table, int rowIndex)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var kv in table)
            {
                dict[kv.Key] = kv.Value[rowIndex];
            }
            return dict;
        }

        public static TSelect[] ToObject<TSelect>(this IDictionary<string, string[]> table) where TSelect : new()
        {
            int len = table.GetRowCount();
            TSelect[] dd = new TSelect[len];
            for (int i = 0; i < len; i++)
            {
                dd[i] = new TSelect();
            }

            var it = typeof(TSelect).GetTypeInfo();

            foreach (var prop in it.DeclaredProperties)
            {
                // table keys may be a different case than property keys. 
                // normalized to be lowercase. 
                var name = Utility.NormalizeKey(prop.Name);

                var propType = prop.PropertyType;
                string[] column = table[name];


                for (int i = 0; i < len; i++)
                {
                    var val2 = Utility.Coerce(propType, column[i]);
                    prop.SetValue(dd[i], val2);
                }                
            }

            return dd;            
        }
    }
}
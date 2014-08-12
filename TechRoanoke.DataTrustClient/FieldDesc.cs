using System;

namespace TechRoanoke.DataTrustClient
{    
    // T is the C# type for the field and is a generic parameter so we can get strong 
    // typing in the client. 
    // When used in a Where clause, we need to convert to the DB Value. 
    public class FieldDesc<T>
    {
        // Avoid query operators needing overloads for FieldDesc vs. string
        public static implicit operator string(FieldDesc<T> f )
        {
            return f.FieldName;
        }

        public string FieldName;

        // If null, use default converted
        // This can be overridden when T is a different type than the DbType. 
        // IE, CongressionalDistrict is a string in the database, but we like to treat it as an int in C#. 
        public Func<T, string> _convertToDbValue;

        public FieldDesc(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public string ConvertToDbValue(T value)
        {
            if (_convertToDbValue != null)
            {
                return _convertToDbValue(value);
            }
            return DbConverter.ConvertToDbValue<T>(value);
        }
    }    
}
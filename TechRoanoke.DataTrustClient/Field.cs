using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TechRoanoke.DataTrustClient
{
    // Helper for known fields
    public class Field
    {
        public const string Party = "party";
        public const string RNCCalcParty = "RNCCalcParty";
        public const string StateAbbreviation = "stateabbreviation";
        public const string CongressionalDistrict = "CongressionalDistrict";
        public const string Sex = "sex";        
        public const string CountyName = "CountyName";

        public static string Count(string fieldName)
        {
            return "count(" + fieldName + ")";
        }
    }   
}
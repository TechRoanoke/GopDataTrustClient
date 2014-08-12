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
    // Centralize in 1 file where we register specific field types and custom converters.

    // List of known fields. 
    // - Provide compile-time typing
    // - standard spelling
    // - standard type conversion
    public class Field
    {
        public readonly static FieldDesc<State> StateAbbreviation = new FieldDesc<State>("stateabbreviation");

        public readonly static FieldDesc<int> CongressionalDistrict = new FieldDesc<int>("CongressionalDistrict")
        {
            _convertToDbValue = DbConverter.IntToQuotedString
        };

        public readonly static FieldDesc<Sex> Sex = new FieldDesc<Sex>("sex");
        public readonly static FieldDesc<string> CountyName = new FieldDesc<string>("CountyName");

        public readonly static FieldDesc<PartyKey> Party = new FieldDesc<PartyKey>("party");
        public readonly static FieldDesc<RncCalcParty> RNCCalcParty = new FieldDesc<RncCalcParty>("RNCCalcParty");

        public static string Count(string fieldName)
        {
            return "count(" + fieldName + ")";
        }


        // Add custom converters.
        public static void AddCustomConverters()
        {            
            // Custom types
            DbConverter.Add<State>(StateExtensions.ToDbValue, StateExtensions.Parse);
            DbConverter.Add<PartyKey>(x => x.DBValue, PartyKey.Parse);
            DbConverter.Add<Sex>(SexExtensions.ToDbValue, SexExtensions.Parse);
            DbConverter.Add<RncCalcParty>(RncCalcPartyExtensions.ToDbValue, RncCalcPartyExtensions.Parse);
            DbConverter.Add<DateTime>(null, DbConverter.CoerceDate);
        }
    }   
}
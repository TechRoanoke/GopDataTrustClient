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
    // var states = {"Illinois/Lincoln":"IL","Alabama":"AL","Alaska":"AK","Arizona":"AZ","Arkansas":"AR","California":"CA","Colorado":"CO","Connecticut":"CT","Delaware":"DE", "District Of Columbia":"DC","Florida":"FL","Georgia":"GA","Hawaii":"HI","Idaho":"ID","Indiana":"IN","Iowa":"IA","Kansas":"KS","Kentucky":"KY","Louisiana":"LA","Maine":"ME","Maryland":"MD","Massachusetts":"MA","Michigan":"MI","Minnesota":"MN","Mississippi":"MS","Missouri":"MO","Montana":"MT","Nebraska":"NE","Nevada ":"NV","New Hampshire":"NH","New Jersey":"NJ","New Mexico":"NM","New York":"NY","North Carolina":"NC","North Dakota":"ND","Ohio":"OH","Oklahoma ":"OK","Oregon":"OR","Pennsylvania":"PA","Rhode Island":"RI","South Carolina":"SC","South Dakota":"SD","Tennessee":"TN","Texas":"TX","Utah":"UT","Vermont":"VT","Virginia":"VA","Washington":"WA","West Virginia":"WV","Wisconsin":"WI","Wyoming":"WY"};

    // Convert to 2-letter code. 
    // Also have pretty display name. 
    public enum State
    {
        Illinois,
        Washington,
        // TODO - add rest of states. 
    }

    public static class StateExtensions
    {
        // Convert either 2-letter code or display name to a state object. 
        public static State Parse(string val)
        {
            val = val.ToUpper();
            for (int i = 0; i < _stateValues.Length; i++)
            {
                if (val == _stateValues[i])
                {
                    return (State)i;
                }
            }
            throw new InvalidOperationException("Unrecognized state :" + val);
        }

        // Return 2 -letter code for sending to database
        public static string ToDbValue(this State x)
        {
            return _stateValues[(int)x];
        }

        static string[] _stateValues = new string[] { 
            "IL", "WA"
        };
    }
}
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
    // Different types of party 
    // Since party keys get normalized to singletons, we can do comparison via object identity.
    public class PartyKey
    {
        internal string _dbValue;
        internal string _displayName;

        private PartyKey(char dbValue, string displayName)
        {
            _dbValue = dbValue.ToString();
            _displayName = displayName;
        }

        public string DBValue { get { return _dbValue; } }
        public string DisplayName { get { return _displayName; } }

        // Common definitions for easy lookup.
        public static readonly PartyKey Republican = new PartyKey('R', "Republican");
        public static readonly PartyKey Democrat = new PartyKey('D', "Democrat");
        public static readonly PartyKey Other = new PartyKey('O', "Other");
        public static readonly PartyKey None = new PartyKey('N', "None");

        // Optimized lookup by the character index of the party. 
        private static PartyKey[] _keys = Init();

        private static PartyKey[] Init()
        {
            PartyKey[] keys = new PartyKey[26];

            var all = new PartyKey[] { 
                Republican,
			    Democrat,
			    new PartyKey('I', "Independent"),
			    new PartyKey('U', "Unaffiliated"),
                new PartyKey('T', "Libertarian"),
			    new PartyKey('M', "Reform"),
			    new PartyKey('G', "Green"),
			    new PartyKey('C', "Conservative"),
			    new PartyKey('L', "Liberal"),
			    new PartyKey('X', "US Taxpayers"),
			    new PartyKey('F', "Right to Life"),
			    new PartyKey('P', "Peace and Freedom"),
			    new PartyKey('E', "American Independence"),
			    new PartyKey('S', "Socialist"),
			    new PartyKey('W', "Natural law"),
			    new PartyKey('A', "State specific"),
			    Other,
			    None,
            };

            foreach (var key in all)
            {
                int idx = (int)key._dbValue[0] - 'A';
                keys[idx] = key;
            }
            return keys;
        }

        public static PartyKey Parse(string x)
        {
            x = x.ToUpper(); // case doesn't matter.

            if (x.Length != 1)
            {
                string msg = string.Format("Party code should be 1 character: '{0}'", x);
                throw new InvalidOperationException(msg);
            }
            char ch = x[0];
            int idx = (int)ch - 'A';
            var key = _keys[idx];

            if (key == null)
            {
                string msg = string.Format("Unrecognized party code '{0}'", x);
                throw new InvalidOperationException(msg);
            }
            return key;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", _dbValue, _displayName);
        }
    }
}
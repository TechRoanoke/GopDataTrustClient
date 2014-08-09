using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechRoanoke.DataTrustClient;

namespace Driver
{
    // Sample app 
    class Program
    {
        static void Main(string[] args)
        {
            // REPLACE with your client token. 
            string token = "YOUR TOKEN HERE";

            Client c = new Client(token);
                    
            // Run a query
            var p2 = c.QuerySelect<PersonObj>().
                Where("stateabbreviation", "IL").Limit(3).ExecuteAsync().Result;

            var pkid = p2[0].personkey;

            // Do an update
            var dd = new Dictionary<string, string>();
            dd["nickname"] = "gwen";
            c.UpdateAsync(pkid, dd, reason: "this is a test from Mike.").Wait();
        }
    }

    // Strongly-typed object used in a query. 
    public class PersonObj
    {
        public PK_ID personkey { get; set; }
        public string firstname { get; set; }

        public string nickname { get; set; }
        public string lastname { get; set; }
        public DateTime dateofbirth { get; set; }
    }
}

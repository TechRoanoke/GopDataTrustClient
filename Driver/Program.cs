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
            string token = "YOUR KEY HERE";

            Client c = new Client(token);

            Report(c);
        }

        static void Report(Client c)
        {

            // Totally loosely typed query 
            // Get some unique names from the state
            string[] q8 = c.Execute("SELECT DISTINCT firstname WHERE stateabbreviation='IL' LIMIT 100").GetSingleColumn();

            // Get the list of congression districts in the state.
            var q1 = Query.SelectDistinct("congressionaldistrict").Limit(100);
            var r1 = c.Execute(q1.ToDql());
            string[] r1b = r1.GetSingleColumn();

            // Get a list of Person2 objects. This will bind the properties on Person2 to the 
            // select values in the database.
            // Query with strong binding. 
            var q6 = Query.Select<Person2>().
                Where(Field.Party, PartyKey.Republican).
                Where(Field.Sex, Sex.Male).
                Limit(100);
            var r6 = c.Execute(q6);

            // County # of people in the district
            var q2 = Query.SelectCount("firstname").
                    Where("stateabbreviation", "IL").
                    Where("CongressionalDistrict", "12").Limit(100);
            var r2 = c.Execute(q2.ToDql());
            var totalVoters = r2.GetSingleValue();

            // Very strongly typed query
            var q4 = Query.Select(Field.RNCCalcParty, Field.Count(Field.RNCCalcParty)).
                Where(Field.StateAbbreviation, State.Illinois).
                Where(Field.CongressionalDistrict, "12").
                GroupBy(Field.RNCCalcParty).
                Limit(200);
            var r4 = c.Execute(q4.ToDql()); // get results as loosely typed
            var r4b = r4.ToObject<RNCCalcPartyBreakdown>(); // then convert to strongly-typed obj.
            
            // Get a list of counties and number of voters in each county 
            var q3 = Query.Select<CountyBreakdown>().
                Where(Field.StateAbbreviation, State.Illinois).
                Where(Field.CongressionalDistrict, 12).
                GroupBy("CountyName").
                Limit(200);
            CountyBreakdown[] o3 = c.Execute(q3);

            
            // Another query of people in the district. 
            var q7 = Query.Select<PersonObj>().
                Where("stateabbreviation", "IL").
                Where("CongressionalDistrict", "12").
                Limit(10);
            var r7 = c.Execute(q7);
        }
    }

    // Some strong typing used in queries. 

    public class RNCCalcPartyBreakdown
    {
        public RncCalcParty RNCCalcParty { get; set; }
        public int count_RNCCalcParty { get; set; }
    }

    public class CountyBreakdown
    {
        public string countyname { get; set; }

        // Convention for: count(CountyName)
        // This matches the convention that GDT returns in. 
        public int count_countyname { get; set; }
    }

    class Person2
    {
        public string firstname { get; set; }
        public Sex sex { get; set; }
        public PartyKey Party { get; set; }
        public PersonKey personkey { get; set; }
    }

    // Strongly-typed object used in a query. 
    public class PersonObj
    {
        public PersonKey personkey { get; set; }
        public string firstname { get; set; }

        public string nickname { get; set; }
        public string lastname { get; set; }
        public DateTime dateofbirth { get; set; }
    }
}

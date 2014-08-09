using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TechRoanoke.DataTrustClient.Tests
{
    [TestClass]
    public class QueryBuilderTests
    {
        [TestMethod]
        public void Query1()
        {
            var q4 = Query.Select(Field.RNCCalcParty, Field.Count(Field.RNCCalcParty)).
               Where(Field.StateAbbreviation, State.Illinois).
               Where(Field.CongressionalDistrict, "12").
               GroupBy("RNCCalcParty").
               Limit(200);

            var dql = q4.ToDql();

            Assert.AreEqual(
                "SELECT RNCCalcParty,count(RNCCalcParty) WHERE stateabbreviation='IL' AND CongressionalDistrict='12' GROUP BY RNCCalcParty LIMIT 200",
                dql);                
        }        
    }
}

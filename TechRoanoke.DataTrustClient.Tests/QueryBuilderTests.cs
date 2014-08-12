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
               Where(Field.StateAbbreviation, State.Illinois). // Strong 
               Where(Field.CongressionalDistrict, 12). // 12 will get quoted as '12' since the DB type is actually a string
               GroupBy("RNCCalcParty").
               Limit(200);

            var dql = q4.ToDql();

            Assert.AreEqual(
                "SELECT RNCCalcParty,count(RNCCalcParty) WHERE stateabbreviation='IL' AND CongressionalDistrict='12' GROUP BY RNCCalcParty LIMIT 200",
                dql);                
        }

        [TestMethod]
        public void Query2()
        {
            var q4 = Query.SelectDistinct("f1").
               Where(Field.StateAbbreviation, "XX"). // mix strong & direct string
               Where("districtid", 12).  // not quoted
               Limit(5);

            var dql = q4.ToDql();

            Assert.AreEqual(
                "SELECT DISTINCT f1 WHERE stateabbreviation='XX' AND districtid=12 LIMIT 5",
                dql);
        }


        [TestMethod]
        public void Query3()
        {
            var q3 = Query.Select<CountyBreakdown>().
                        Where("state", "MI").                        
                        GroupBy("countyname").
                        Limit(200);

            var dql = q3.ToDql();

            Assert.AreEqual(
                "SELECT countyname,count(countyname) WHERE state='MI' GROUP BY countyname LIMIT 200",
                dql);
        }

        public class CountyBreakdown
        {
            public string countyname { get; set; }

            // Convention for: count(CountyName)
            // This matches the convention that GDT returns in. 
            public int count_countyname { get; set; }
        }

        [TestMethod]
        public void QueryWhereTypingPermutations()
        {
            var q4 = Query.Select("f1").               
               Where(Field.CongressionalDistrict, "12"). // quoted
               Where("cd", "12"). // quoted
               Where("cd", 12). // not quoted
               Where(Field.CongressionalDistrict, 12). // Quoted!!! Since we have strong-typing info
               Limit(10);

            var dql = q4.ToDql();
            Assert.AreEqual("SELECT f1 WHERE CongressionalDistrict='12' AND cd='12' AND cd=12 AND CongressionalDistrict='12' LIMIT 10",
                dql);
        }

        [TestMethod]
        public void QueryDates()
        {
            var q4 = Query.Select("firstname").
                Where(Field.DateOfBirth, "1981", CompareOp.Greater).
                Where(Field.DateOfBirth, new DateTime(1995, 1, 1), CompareOp.Less).
                Limit(10);               

            var dql = q4.ToDql();

            Assert.AreEqual(
                "SELECT firstname WHERE dateofbirth>'1981' AND dateofbirth<'1995' LIMIT 10",
                dql);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TechRoanoke.DataTrustClient.Tests
{
    [TestClass]
    public class DbConverterTests
    {
        [TestMethod]
        public void TestStandardConverters()
        {
            // String
            string stateObj = DbConverter.ConvertToObject<string>("IL");
            Assert.AreEqual("IL", stateObj);

            string state = DbConverter.ConvertToDbValue<string>("IL");
            Assert.AreEqual("'IL'", state); // gets quoted

            // Int
            int numObj = DbConverter.ConvertToObject<int>("12");
            Assert.AreEqual(12, numObj);

            string num = DbConverter.ConvertToDbValue<int>(12);
            Assert.AreEqual("12", num); // not quoted
        }

        [TestMethod]
        public void TestCustomConverters()
        {
            // State
            State stateObj = DbConverter.ConvertToObject<State>("WA");
            Assert.AreEqual(State.Washington, stateObj);

            string state = DbConverter.ConvertToDbValue<State>(State.Illinois);
            Assert.AreEqual("'IL'", state);

            // Party Key 
            PartyKey partyObj = DbConverter.ConvertToObject<PartyKey>("R");
            Assert.AreEqual(PartyKey.Republican, partyObj);

            string party = DbConverter.ConvertToDbValue<PartyKey>(partyObj);
            Assert.AreEqual("'R'", party);

            // Sex
            Sex sexObj = DbConverter.ConvertToObject<Sex>("F");
            Assert.AreEqual(Sex.Female, sexObj);

            string sex = DbConverter.ConvertToDbValue<Sex>(Sex.Male);
            Assert.AreEqual("'M'", sex);

            // RncCalcParty
            RncCalcParty rncObj = DbConverter.ConvertToObject<RncCalcParty>("1");
            Assert.AreEqual(RncCalcParty.HardGop, rncObj);

            string rnc = DbConverter.ConvertToDbValue<RncCalcParty>(RncCalcParty.WeakDem);
            Assert.AreEqual("'4'", rnc);
        }

        [TestMethod]
        public void TestConvertDates()
        {
            // YYYY
            DateTime dt1 = DbConverter.ConvertToObject<DateTime>("1981");
            Assert.AreEqual(1981, dt1.Year);

            var str1 = DbConverter.ConvertToDbValue<DateTime>(dt1);
            Assert.AreEqual("'1981'", str1);

            // YYYY MM
            DateTime dt2 = DbConverter.ConvertToObject<DateTime>("198106");
            Assert.AreEqual(1981, dt2.Year);
            Assert.AreEqual(6, dt2.Month);

            var str2 = DbConverter.ConvertToDbValue<DateTime>(dt2);
            Assert.AreEqual("'198106'", str2);

            // YYYY MM DD
            DateTime dt3 = DbConverter.ConvertToObject<DateTime>("19810615");
            Assert.AreEqual(1981, dt3.Year);
            Assert.AreEqual(6, dt3.Month);
            Assert.AreEqual(15, dt3.Day);

            var str3 = DbConverter.ConvertToDbValue<DateTime>(dt3);
            Assert.AreEqual("'19810615'", str3);

            // From date time 

            var dt4 = new DateTime(1985, 12, 3);
            var str4 = DbConverter.ConvertToDbValue<DateTime>(dt4);
            Assert.AreEqual("'19851203'", str4);
        }            
    }
}
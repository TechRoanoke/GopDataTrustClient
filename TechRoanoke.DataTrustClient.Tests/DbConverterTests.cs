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
            // DateTime
            DateTime dt = DbConverter.ConvertToObject<DateTime>("1981");
            Assert.AreEqual(1981, dt.Year);

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
    }
}
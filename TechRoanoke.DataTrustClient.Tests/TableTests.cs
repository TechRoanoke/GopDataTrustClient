using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TechRoanoke.DataTrustClient.Tests
{
    [TestClass]
    public class TableTests
    {
        [TestMethod]
        public void StrongConversion()
        {
            var table = NewTable(3,
                "RNCCalcParty", "count_RNCCalcParty", "state",
                "1", "1000", "IL",            
                "5", "1600", "WA");

            RNCCalcPartyBreakdown[] strong = table.ToObject<RNCCalcPartyBreakdown>();

            Assert.AreEqual(2, strong.Length);

            Assert.AreEqual(RncCalcParty.HardGop, strong[0].RNCCalcParty);
            Assert.AreEqual(1000, strong[0].count_RNCCalcParty);
            Assert.AreEqual(State.Illinois, strong[0].state);

            Assert.AreEqual(RncCalcParty.HardDem, strong[1].RNCCalcParty);
            Assert.AreEqual(1600, strong[1].count_RNCCalcParty);
            Assert.AreEqual(State.Washington, strong[1].state);

            // column access
            var states = table.GetColumn<State>("state");
            Assert.AreEqual(2, states.Length);
            Assert.AreEqual(State.Illinois, states[0]);
            Assert.AreEqual(State.Washington, states[1]);
        }

        public class RNCCalcPartyBreakdown
        {
            public RncCalcParty RNCCalcParty { get; set; }
            public int count_RNCCalcParty { get; set; }

            public State state { get; set; }
        }


        [TestMethod]
        public void Basic()
        {
            var table = NewTable(2,
                "RNCCalcParty", "count_RNCCalcParty",
                "1", "1000",
                "2", "1500",
                "3", "8000",
                "4", "2000",
                "5", "1600");

            var count = table.GetRowCount();
            Assert.AreEqual(5, count);

            IDictionary<string, string> row3 = table.GetRow(2);
            Assert.AreEqual("3", row3["rnccalcparty"]);
            Assert.AreEqual("8000", row3["count_rnccalcparty"]);

            string[] column = table.GetColumn("count_rnccalcparty");
            Assert.AreEqual(5, column.Length);
            Assert.AreEqual("1600", column[4]);
        }

        // Helper for easily creating a new table.
        // data is the row-major (ie, read-order) list of all values. 
        // numColumns is how many columns, which we use to convert a 1-d array into a 2-d table.
        public static IDictionary<string, string[]> NewTable(
           int numColumns, params string[] data)
        {
            int numRows = (data.Length / numColumns) - 1;

            Dictionary<string, string[]> dd = new Dictionary<string, string[]>();

            string[][] columns = new string[numColumns][];
            for (int i = 0; i < numColumns; i++)
            {
                string header = Utility.NormalizeKey(data[i]);
                string[] column = new string[numRows];
                dd[header] = column;
                columns[i] = column;
            }

            int idx = numColumns;
            for (int row = 0; row < numRows; row++)
            {
                for (int iColumn = 0; iColumn < numColumns; iColumn++)
                {
                    string[] column = columns[iColumn];
                    column[row] = data[idx];
                    idx++;
                }
            }
            return dd;
        }
    }
}
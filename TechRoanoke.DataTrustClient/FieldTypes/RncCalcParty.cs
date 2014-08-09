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
    // Database value is an integer. 1...5. 
    public enum RncCalcParty
    {
        Unknown = 0,
        HardGop = 1,
        WeakGop = 2,
        Swing = 3,
        WeakDem = 4,
        HardDem = 5
    }

    public static class RncCalcPartyExtensions
    {
        public static bool IsGop(this RncCalcParty id)
        {
            return id == RncCalcParty.HardGop || id == RncCalcParty.WeakGop;
        }
    }
}
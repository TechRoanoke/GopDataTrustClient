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
    // Has various overloads for Queries.
    // - strongly typed vs. loosely typed 
    //      - input: QueryBuilder<TSelect> vs string 
    //      - return: TSelect[] vs. IDict[]
    // - Sync vs. Async 
    public static class ClientExtensions
    {
        public static TSelect[] Execute<TSelect>(this Client client, QueryBuilder<TSelect> query) where TSelect : new()
        {
            string dql = query.ToDql();
            return client.Execute<TSelect>(dql);
        }

        public static TSelect[] Execute<TSelect>(this Client client, string dql) where TSelect : new()
        {
            JObject[] jt = client.RunQueryInternalAsync(dql).Result;

            var results = Utility.ConvertToObj<TSelect>(jt);
            return results;
        }

        // The most loosely typed query. Take any DQL string, return as a Dictionary.
        // Mapping of selected Fields --> array of values. 
        public static IDictionary<string, string[]> Execute(this Client client, string dql)
        {
            JObject[] jt = client.RunQueryInternalAsync(dql).Result;

            var results = Utility.ConvertToTable(jt);
            return results;
        }

        // The most loosely typed query. Take any DQL string, return as a Dictionary.
        // Mapping of selected Fields --> array of values. 
        public static async Task<IDictionary<string, string[]>> ExecuteAsync(this Client client, string dql)
        {
            JObject[] jt = await client.RunQueryInternalAsync(dql);

            var results = Utility.ConvertToTable(jt);
            return results;
        }
    }
}

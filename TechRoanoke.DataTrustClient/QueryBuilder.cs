using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TechRoanoke.DataTrustClient
{
    // Helper for building up a query 
    public class QueryBuilder<TSelect> where TSelect : new()
    {
        internal string[] _fieldNames;
        internal List<Tuple<string, string>> _where = new List<Tuple<string,string>>();
        internal int _limit = 1;
        internal Client _client;

        // Where clause gets evaluated on server, so fields don't need to match selection
        public QueryBuilder<TSelect> Where(string fieldName, string value)
        {
            _where.Add(Tuple.Create(fieldName, value));
            return this;
        }

        public QueryBuilder<TSelect> Limit(int n)
        {
            _limit = n;
            return this;
        }

        public string ToDql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(string.Join(",", _fieldNames));
            sb.Append(" ");

            if (_where.Count > 0)
            {
                sb.Append("WHERE");
                int count = 0;
                foreach (var kv in _where)
                {
                    if (count > 0)
                    {
                        sb.Append(" AND");
                    }
                    sb.AppendFormat(" {0}='{1}'", kv.Item1, kv.Item2);
                    count++;
                }
            }

            if (_limit > 0)
            {
                sb.AppendFormat(" LIMIT {0}", _limit);
            }

            return sb.ToString();
        }

        public async Task<TSelect[]> ExecuteAsync()
        {            
            string dql = this.ToDql();
            var result = await _client.ExecuteQuery<TSelect>(dql);
            return result;
        }
    }
}
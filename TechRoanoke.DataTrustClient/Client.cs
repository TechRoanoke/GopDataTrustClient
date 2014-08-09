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
    public class Client
    {
        private readonly string _token;

        private readonly string _queryPrefix;
        private readonly string _updatePrefix;        

        // Token is the secret key that grants access to this account. 
        // serverUrl is the url prefix for the server.  
        public Client(string token, string serverUrl = "https://lincoln.gopdatatrust.com")
        {
            string prefix = serverUrl + "/v2/api/";
            
            _token = token;
            _queryPrefix = prefix + "query.php?ClientToken=" + token;
            _updatePrefix = prefix + "direct_write.php?ClientToken=" + token;
        }
                
        internal async Task<JObject[]> RunQueryInternalAsync(string dql)
        {
            // DQL will get encoded, so spaces are ok. 
            string uri = _queryPrefix + "&q=" + dql;
            string body = await Send(uri);

            Result result = JsonConvert.DeserializeObject<Result>(body);
            if (!result.Success)
            {
                throw new DqlException(result.Error, dql);
            }
            return result.Results;
        }

        private static async Task<TResult> Send<TResult>(string uri) where TResult : ResultBase
        {
            var body = await Send(uri);
            TResult result = JsonConvert.DeserializeObject<TResult>(body);
            if (!result.Success)
            {
                throw new InvalidOperationException(result.Error);
            }

            return result;
        }

        private static async Task<string> Send(string uri)
        {
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            HttpResponseMessage response = await client.SendAsync(request);

            string body = await response.Content.ReadAsStringAsync();
            return body;
        }

        public async Task UpdateAsync(PersonKey pkid, IDictionary<string, string> values, string reason)
        {
            // $$$ This is failing. Appears to be a bug in their server. 
            StringBuilder sb = new StringBuilder();
            sb.Append(_updatePrefix);
            sb.AppendFormat("&Action=update&PK_Field={0}&PK_ID={1}&Values=", "personkey", (int)pkid);
            sb.Append('{');

            int count = 0;
            foreach (var kv in values)
            {
                if (count > 0)
                {
                    sb.Append(',');
                }
                sb.AppendFormat("'{0}':'{1}'", kv.Key, kv.Value);
                count++;
            }
            sb.Append('}');
            sb.AppendFormat("&Rationale={0}", reason);

            string uri = sb.ToString();
            UpdateResult body = await Send<UpdateResult>(uri);
        }
    }

    class ResultBase
    {
        public string Call_ID { get; set; }
        public bool Success { get; set; }

        // If !Success, provides a human readable error message from the server. 
        public string Error { get; set; }
    }
    
    class UpdateResult : ResultBase
    {
        public string PK_ID { get; set; }
    }

    class Result : ResultBase
    {
        public JObject[] Results { get; set; }

        public int Results_Count { get; set; }
        public bool More_Results { get; set; }
    }
}

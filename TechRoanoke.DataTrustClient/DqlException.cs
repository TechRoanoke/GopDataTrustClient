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
    public class DqlException : Exception
    {
        // The original query that triggered the server response. 
        public string Query { get; set; }

        public DqlException(string msg, string query)
            : base(msg)
        {
            this.Query = query;
        }
    }
}
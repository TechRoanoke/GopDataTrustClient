﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TechRoanoke.DataTrustClient
{
    // Comparison operations supported in a where-clause. 
    public enum CompareOp
    {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        Like, // allows wildcard matching using '%'.  "ABC" ~ "%B%. 
        NotLike,
    }

    // Static builder class for starting a query. 
    public static class Query
    {
        public static QueryBuilder<object> Select(params string[] fieldNames)
        {
            var qb = new QueryBuilder<object> { _selectFields = fieldNames };
            return qb;
        }

        public static QueryBuilder<object> SelectDistinct(string fieldName)
        {
            var qb = new QueryBuilder<object> { _selectDistinct = fieldName };
            return qb;        
        }

        public static QueryBuilder<object> SelectCount(string fieldName)
        {
            var qb = new QueryBuilder<object> { _selectFields = new string[] 
                {
                    string.Format("count({0})", fieldName) 
                }};
            return qb;
        }

        public static QueryBuilder<TSelect> Select<TSelect>() where TSelect : new()
        {
            string[] fieldNames = Utility.GetSelectNamesFromType(typeof(TSelect));

            var qb = new QueryBuilder<TSelect> { _selectFields = fieldNames };
            return qb;
        }
    }

    // Helper for building up a query 
    // DISTINCT, COUNT(), COUNT(DISTINCT), and GROUP BY 
    public class QueryBuilder<TSelect> where TSelect : new()
    {
        internal string[] _selectFields;
        internal string _selectDistinct;
        internal List<Tuple<string, CompareOp, string>> _where = new List<Tuple<string, CompareOp, string>>();
        internal int _limit = 1;

        internal string _groupByField;

        // Where clause gets evaluated on server, so fields don't need to match selection
        // Allowed operators in a WHERE statement include =, !=, >, >=, <, <=, ~, and !~. 
        // The wildcard character for a string comparison using ~ or !~ is %.
        public QueryBuilder<TSelect> Where(string fieldName, string value, CompareOp op = CompareOp.Equal)
        {
            string dbValue = DbConverter.Quote(value);
            return WhereInner(fieldName, dbValue, op);
        }

        public QueryBuilder<TSelect> Where(string fieldName, int value, CompareOp op = CompareOp.Equal)
        {
            string dbValue = value.ToString(); // Don't quote integers
            return WhereInner(fieldName, dbValue, op);
        }

        public QueryBuilder<TSelect> Where<T>(FieldDesc<T> field, T value, CompareOp op = CompareOp.Equal)
        {
            string dbValue = field.ConvertToDbValue(value);
            return WhereInner(field, dbValue, op);
        }

        // Caller converts the Value to a dbValue
        private QueryBuilder<TSelect> WhereInner(string fieldName, string dbValue, CompareOp op)
        {
            _where.Add(Tuple.Create(fieldName, op, dbValue));
            return this;
        }

        public QueryBuilder<TSelect> Limit(int n)
        {
            _limit = n;
            return this;
        }

        public QueryBuilder<TSelect> GroupBy(string fieldName)
        {
            _groupByField = fieldName;
            return this;
        }

        private static string CompareOpToString(CompareOp op)
        {
            switch (op)
            {
                case CompareOp.Equal: return "=";
                case CompareOp.NotEqual: return "!=";
                case CompareOp.Greater: return ">";
                case CompareOp.GreaterOrEqual: return ">=";
                case CompareOp.Less: return "<";
                case CompareOp.LessOrEqual: return "<=";
                case CompareOp.Like: return "~";
                case CompareOp.NotLike: return "!~";
                default:
                    string msg = string.Format("Unknown Where operation: {0}", op);
                    throw new InvalidOperationException(msg);
            }
        }

        public override string ToString()
        {
            return ToDql();
        }
        public string ToDql()
        {
            StringBuilder sb = new StringBuilder();

            if (_selectFields != null)
            {
                sb.Append("SELECT ");
                sb.Append(string.Join(",", _selectFields));                
            }
            else if (_selectDistinct != null)
            {
                sb.AppendFormat("SELECT DISTINCT {0}", _selectDistinct);
            }
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
                    // Where requires the rhs be quoted for strings, not quoted for ints. 
                    string fieldName = kv.Item1;
                    string opName = CompareOpToString(kv.Item2);
                    string valueName = kv.Item3;
                    sb.AppendFormat(" {0}{1}{2}", fieldName, opName, valueName);
                    count++;
                }
            }

            if (_groupByField != null)
            {
                // Don't quote fieldname for groupby and select, but we do quote it for Where
                sb.AppendFormat(" GROUP BY {0}", _groupByField);
            }

            if (_limit > 0)
            {
                sb.AppendFormat(" LIMIT {0}", _limit);
            }

            return sb.ToString();
        }
    }
}
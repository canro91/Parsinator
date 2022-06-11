using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Parsinator
{
    public static class DictionaryExtensions
    {
        // HACK: Details are returned as <KeyName[index], <String, String>>
        public static DataSet ToDataSet(this Dictionary<string, Dictionary<string, string>> dict, DataSet dataSet)
        {
            foreach (var item in dict.Where(t => !t.Key.Contains('[')))
            {
                var sectionName = item.Key;
                var parsed = item.Value;

                if (!dataSet.Tables.Contains(sectionName))
                    throw new ArgumentNullException($"DataSet [{dataSet.DataSetName}] doesn't contain a table: [{sectionName}]");

                DataRow row = dataSet.Tables[sectionName].NewRow();
                foreach (var p in parsed)
                {
                    row[p.Key] = p.Value;
                }
                dataSet.Tables[sectionName].Rows.Add(row);
            }

            var keys = dict.Where(t => t.Key.Contains('['))
                           .Select(t => t.Key.Substring(0, t.Key.IndexOf('[')))
                           .Distinct();

            foreach (var item in dict.Where(t => t.Key.Contains('[')))
            {
                var sectionName = keys.FirstOrDefault(t => item.Key.StartsWith(t));
                var parsed = item.Value;

                DataRow row = dataSet.Tables[sectionName].NewRow();
                foreach (var p in parsed)
                {
                    row[p.Key] = p.Value;
                }
                dataSet.Tables[sectionName].Rows.Add(row);
            }

            return dataSet;
        }

        internal static Dictionary<string, Dictionary<string, string>> AddOrMerge(this Dictionary<string, Dictionary<string, string>> self, String key, Dictionary<string, string> dict)
        {
            if (!self.ContainsKey(key))
                self[key] = dict;
            else
            {
                foreach (var item in dict)
                {
                    self[key].Add(item.Key, item.Value);
                }
            }
            return self;
        }

        internal static IDictionary<string, string> Merge(this IDictionary<string, string> self, IDictionary<string, string> dict)
        {
            foreach (var item in dict)
            {
                if (!self.ContainsKey(item.Key))
                    self.Add(item.Key, item.Value);
            }
            return self;
        }
    }
}

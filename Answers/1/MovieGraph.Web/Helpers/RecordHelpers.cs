using System;
using System.Linq;
using Neo4j.Driver.V1;

namespace MovieGraph.Web
{
    public static class RecordHelpers
    {
        public static T GetOrDefault<T>(this IRecord record, string key, T defaultValue)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (record.Keys.Contains(key) == false)
            {
                return defaultValue;
            }

            return record.Values.GetOrDefault(key, defaultValue);
        }
    }
}
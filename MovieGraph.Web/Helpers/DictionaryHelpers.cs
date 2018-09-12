using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace MovieGraph.Web
{
    public static class DictionaryHelpers
    {
        public static object GetOrDefault(this IReadOnlyDictionary<string, object> dict, string key,
            object defaultValue)
        {
            if (dict.TryGetValue(key, out var value))
            {
                if (value == null)
                {
                    return defaultValue;
                }

                return value;
            }

            return defaultValue;
        }

        public static T GetOrDefault<T>(this IReadOnlyDictionary<string, object> dict, string key, T defaultValue)
            where T : IConvertible
        {
            if (dict.TryGetValue(key, out var value))
            {
                if (value == null)
                {
                    return defaultValue;
                }

                var actualType = typeof(T);
                if (actualType.GetTypeInfo().IsEnum)
                {
                    return (T) Enum.Parse(actualType, Convert.ToString(value), true);
                }

                return (T) Convert.ChangeType(value, typeof(T));
            }

            return defaultValue;
        }

        public static object GetOrDefault(this IDictionary<string, object> dict, string key, object defaultValue)
        {
            if (dict.TryGetValue(key, out var value))
            {
                if (value == null)
                {
                    return defaultValue;
                }

                return value;
            }

            return defaultValue;
        }

        public static T GetOrDefault<T>(this IDictionary<string, object> dict, string key, T defaultValue)
            where T : IConvertible
        {
            if (dict.TryGetValue(key, out var value))
            {
                if (value == null)
                {
                    return defaultValue;
                }

                var actualType = typeof(T);
                if (actualType.GetTypeInfo().IsEnum)
                {
                    return (T) Enum.Parse(actualType, Convert.ToString(value), true);
                }

                return (T) Convert.ChangeType(value, typeof(T));
            }

            return defaultValue;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace MovieGraph.Web
{
    public static class DictionaryHelpers
    {
        public static T GetOrDefault<T>(this IReadOnlyDictionary<string, object> dict, string key, T defaultValue)
        {
            if (dict.TryGetValue(key, out var value))
            {
                if (value == null)
                {
                    return defaultValue;
                }

                var actualType = typeof(T);
                var underlyingType = Nullable.GetUnderlyingType(actualType);
                if (underlyingType != null)
                {
                    actualType = underlyingType;
                }

                if (actualType.GetTypeInfo().IsEnum)
                {
                    return (T) Enum.Parse(actualType, Convert.ToString(value), true);
                }

                if (actualType.IsInstanceOfType(value))
                {
                    return (T) value;
                }

                if (typeof(IComparable).IsAssignableFrom(actualType))
                {
                    return (T) Convert.ChangeType(value, actualType);
                }

                throw new InvalidCastException($"Unable to cast {value.GetType().FullName} to {actualType.FullName}");
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
                var underlyingType = Nullable.GetUnderlyingType(actualType);
                if (underlyingType != null)
                {
                    actualType = underlyingType;
                }

                if (actualType.GetTypeInfo().IsEnum)
                {
                    return (T) Enum.Parse(actualType, Convert.ToString(value), true);
                }

                if (actualType.IsInstanceOfType(value))
                {
                    return (T) value;
                }

                if (typeof(IComparable).IsAssignableFrom(actualType))
                {
                    return (T) Convert.ChangeType(value, typeof(T));
                }

                throw new InvalidCastException($"Unable to cast {value.GetType().FullName} to {actualType.FullName}");
            }

            return defaultValue;
        }
    }
}
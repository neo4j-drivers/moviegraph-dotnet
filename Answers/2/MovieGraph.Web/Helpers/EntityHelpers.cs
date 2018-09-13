using System;
using Neo4j.Driver.V1;

namespace MovieGraph.Web
{
    public static class EntityHelpers
    {
        public static T GetOrDefault<T>(this IEntity entity, string key, T defaultValue)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return entity.Properties.GetOrDefault(key, defaultValue);
        }
    }
}
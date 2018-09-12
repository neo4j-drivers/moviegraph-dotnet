using System;
using Neo4j.Driver.V1;

namespace MovieGraph.Web
{
    public static class EntityHelpers
    {

        public static object GetOrDefault(this IEntity entity, string key, object defaultValue)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            return entity.Properties.GetOrDefault(key, defaultValue);
        }

        public static T GetOrDefault<T>(this IEntity entity, string key, T defaultValue)
            where T : IConvertible
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            return entity.Properties.GetOrDefault(key, defaultValue);
        }

    }
}
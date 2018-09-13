using System;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Model
{
    public class PersonModel
    {
        public const string NameKey = "name";

        public PersonModel(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Name = node.GetOrDefault<string>(NameKey, null);
        }

        public string Name { get; }
    }
}
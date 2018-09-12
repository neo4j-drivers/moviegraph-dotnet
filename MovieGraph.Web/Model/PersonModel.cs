using System;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Model
{
    public class PersonModel
    {
        public PersonModel(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Name = node["name"].As<string>();
        }

        public string Name { get; }
    }
}
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Model
{
    public class MovieModel
    {
        public MovieModel(INode node)
        {
            Title = node["title"].As<string>();
            Tagline = node.GetOrDefault("tagline", string.Empty);
            Released = node.GetOrDefault("released", 0);
        }

        public MovieModel(IRecord record) : this((INode) record["movie"])
        {
            if (record.Keys.Contains("actors"))
            {
                var actors = (List<object>) record["actors"];
                if (actors != null)
                {
                    Actors = actors.Select(actor => new PersonModel(actor.As<INode>())).OrderBy(p => p.Name);
                }
            }
        }

        public string Title { get; }

        public string Tagline { get; }

        public int Released { get; }

        public IEnumerable<PersonModel> Actors { get; }
    }
}
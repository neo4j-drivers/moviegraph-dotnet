using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Born = node.GetOrDefault("born", string.Empty);
        }

        public PersonModel(IRecord record) : this(record["person"].As<INode>())
        {
            if (record.Keys.Contains("movies"))
            {
                var movies = (List<object>) record["movies"];
                if (movies != null)
                {
                    Movies = movies.Select(movie => new MovieModel(movie.As<INode>())).OrderBy(p => p.Released);
                }
            }
        }

        public string Name { get; }

        public string Born { get; }

        public IEnumerable<MovieModel> Movies { get; }
    }
}
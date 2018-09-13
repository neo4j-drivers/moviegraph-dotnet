using System;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Model
{
    public class PersonModel
    {
        public const string PersonKey = "person";
        public const string NameKey = "name";
        public const string BornKey = "born";
        public const string MoviesKey = "movies";

        public PersonModel(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Name = node.GetOrDefault<string>(NameKey, null);
            Born = node.GetOrDefault<int?>(BornKey, null);
        }

        public PersonModel(IRecord record)
            : this((record ?? throw new ArgumentNullException(nameof(record))).GetOrDefault(PersonKey, (INode) null))
        {
            var movies = record.GetOrDefault(MoviesKey, (List<object>) null);
            if (movies != null)
            {
                Movies = movies.Select(movie => new MovieModel((INode) movie)).OrderBy(p => p.Released);
            }
        }

        public string Name { get; }

        public int? Born { get; }

        public IEnumerable<MovieModel> Movies { get; }
    }
}
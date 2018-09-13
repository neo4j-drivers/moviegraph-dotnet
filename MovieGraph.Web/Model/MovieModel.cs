using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Model
{
    public class MovieModel
    {
        public const string MovieKey = "movie";
        public const string TitleKey = "title";
        public const string TaglineKey = "tagline";
        public const string ReleasedKey = "released";
        public const string ActorsKey = "actors";

        public MovieModel(IRecord record)
        {
            var movie = record.GetOrDefault(MovieKey, (INode) null);
            if (movie != null)
            {
                Title = movie.GetOrDefault<string>(TitleKey, null);
                Tagline = movie.GetOrDefault<string>(TaglineKey, null);
                Released = movie.GetOrDefault<int?>(ReleasedKey, null);
            }

            var actors = record.GetOrDefault(ActorsKey, (List<object>) null);
            if (actors != null)
            {
                Actors = actors.Select(actor => new PersonModel((INode) actor)).OrderBy(p => p.Name);
            }
        }

        public string Title { get; }

        public string Tagline { get; }

        public int? Released { get; }

        public IEnumerable<PersonModel> Actors { get; }
    }
}
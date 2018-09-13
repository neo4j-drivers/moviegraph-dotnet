using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieGraph.Web.Model;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IDriver driver;

        public HomeController(IDriver driver)
        {
            this.driver = driver;
        }

        [Route("")]
        public async Task<IActionResult> Index(string q, Order order)
        {
            return View("Index", await MatchMovies(q, order));
        }

        private async Task<IEnumerable<MovieModel>> MatchMovies(string term, Order order)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Enumerable.Empty<MovieModel>();
            }

            string query;
            switch (order)
            {
                case Order.MostRecentFirst:
                    query =
                        "MATCH (movie:Movie) WHERE toLower(movie.title) " +
                        "CONTAINS toLower($term) " +
                        "RETURN movie ORDER BY movie.released DESCENDING";
                    break;
                case Order.MostPopularFirst:
                    query =
                        "MATCH (movie:Movie) WHERE toLower(movie.title) " +
                        "CONTAINS toLower($term) " +
                        "RETURN movie ORDER BY coalesce(movie.stars, 0) DESCENDING";
                    break;
                case Order.Alphabetically:
                    query =
                        "MATCH (movie:Movie) WHERE toLower(movie.title) " +
                        "CONTAINS toLower($term) " +
                        "RETURN movie ORDER BY movie.title ASCENDING";

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(order), order, null);
            }

            var session = driver.Session();
            try
            {
                return await session.ReadTransactionAsync(async tx =>
                {
                    var cursor =
                        await tx.RunAsync(
                            query,
                            new {term});

                    return await cursor.ToListAsync(record => new MovieModel(record));
                });
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }
    }
}

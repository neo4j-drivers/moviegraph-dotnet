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
        public async Task<IActionResult> Index(string q)
        {
            return View("Index", await MatchMovies(q));
        }

        private async Task<IEnumerable<MovieModel>> MatchMovies(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Enumerable.Empty<MovieModel>();
            }

            var session = driver.Session();
            try
            {
                return await session.ReadTransactionAsync(async tx =>
                {
                    var cursor =
                        await tx.RunAsync(
                            "MATCH (movie:Movie) WHERE toLower(movie.title) CONTAINS toLower($term) RETURN movie",
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

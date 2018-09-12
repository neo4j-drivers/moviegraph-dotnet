using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieGraph.Web.Model;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Controllers
{
    [Route("movie")]
    public class MovieController : Controller
    {
        private readonly IDriver driver;

        public MovieController(IDriver driver)
        {
            this.driver = driver;
        }

        [Route("{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var session = driver.Session(AccessMode.Read);
            try
            {
                var movie = await session.ReadTransactionAsync(tx => MatchMovie(tx, id));
                if (movie == null)
                {
                    return StatusCode(404);
                }

                return View("Index", movie);
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        [Route("{id}")]
        [HttpPost]
        public async Task<IActionResult> SetStars(string id, int stars)
        {
            var session = driver.Session();
            try
            {
                await session.WriteTransactionAsync(tx => SetMovieStars(tx, id, stars));

                return RedirectToAction("Index");
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        private async Task<MovieModel> MatchMovie(ITransaction tx, string title)
        {
            var cursor =
                await tx.RunAsync(
                    "MATCH (movie:Movie) WHERE movie.title = $title OPTIONAL MATCH (person)-[:ACTED_IN]->(movie) RETURN movie, collect(person) AS actors",
                    new {title});

            var found = await cursor.ToListAsync(record => new MovieModel(record));

            return found.SingleOrDefault();
        }

        private Task SetMovieStars(ITransaction tx, string title, int stars)
        {
            return tx.RunAsync("MATCH (movie:Movie) WHERE movie.title = $title SET movie.stars = $stars",
                new {title, stars});
        }
    }
}
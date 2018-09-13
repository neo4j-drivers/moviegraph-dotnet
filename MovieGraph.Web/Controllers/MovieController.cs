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
            var movie = await MatchMovie(id);
            if (movie == null)
            {
                return StatusCode(404);
            }

            return View("Index", movie);
        }

        private async Task<MovieModel> MatchMovie(string title)
        {
            var session = driver.Session();
            try
            {
                return await session.ReadTransactionAsync(async tx =>
                {
                    var cursor =
                        await tx.RunAsync(
                            "MATCH (movie:Movie) WHERE movie.title = $title " +
                            "OPTIONAL MATCH(person) -[:ACTED_IN]->(movie) " +
                            "RETURN movie, collect(person) AS actors",
                            new {title});

                    return new MovieModel(await cursor.SingleAsync());
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

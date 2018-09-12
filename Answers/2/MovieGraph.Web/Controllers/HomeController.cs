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
        public Task<IActionResult> Index()
        {
            return Search(null);
        }

        [Route("search/{q?}")]
        public async Task<IActionResult> Search(string q)
        {
            ViewData["q"] = q;

            var session = driver.Session(AccessMode.Read);
            try
            {
                var results = await session.ReadTransactionAsync(tx => MatchMovies(tx, q));

                return View("Index", results);
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        private async Task<IEnumerable<MovieModel>> MatchMovies(ITransaction tx, string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Enumerable.Empty<MovieModel>();
            }

            var cursor =
                await tx.RunAsync("MATCH (movie:Movie) WHERE toLower(movie.title) CONTAINS toLower($term) RETURN movie",
                    new {term});

            return await cursor.ToListAsync(record => new MovieModel(record));
        }
    }
}
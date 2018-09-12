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
            return Search(null, null);
        }

        [Route("search")]
        public async Task<IActionResult> Search(string q, string order)
        {
            ViewData["q"] = q;
            ViewData["order"] = order;

            var session = driver.Session(AccessMode.Read);
            try
            {
                var results = await session.ReadTransactionAsync(tx => MatchMovies(tx, q, order));

                return View("Index", results);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        private async Task<IEnumerable<MovieModel>> MatchMovies(ITransaction tx, string term, string order)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Enumerable.Empty<MovieModel>();
            }

            var query = string.Empty;
            if ("r".Equals(order))
            {
                query =
                    "MATCH (movie:Movie) WHERE toLower(movie.title) CONTAINS toLower($term) RETURN movie ORDER BY movie.released DESCENDING";
            }
            else if ("p".Equals(order))
            {
                query =
                    "MATCH (movie:Movie) WHERE toLower(movie.title) CONTAINS toLower($term)  RETURN movie ORDER BY coalesce(movie.stars, 0) DESCENDING";
            }
            else if ("a".Equals(order))
            {
                query =
                    "MATCH (movie:Movie) WHERE toLower(movie.title) CONTAINS toLower($term) RETURN movie ORDER BY movie.title ASCENDING";
            }
            else
            {
                throw new ArgumentException("Bad order parameter", nameof(order));
            }


            var cursor =
                await tx.RunAsync(query, new {term});

            return await cursor.ToListAsync(record => new MovieModel(record));
        }
    }
}
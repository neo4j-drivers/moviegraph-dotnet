using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieGraph.Web.Model;
using Neo4j.Driver.V1;

namespace MovieGraph.Web.Controllers
{
    [Route("person")]
    public class PersonController : Controller
    {
        private readonly IDriver driver;

        public PersonController(IDriver driver)
        {
            this.driver = driver;
        }

        [Route("{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var session = driver.Session(AccessMode.Read);
            try
            {
                var person = await session.ReadTransactionAsync(tx => MatchPerson(tx, id));
                if (person == null)
                {
                    return StatusCode(404);
                }

                return View("Index", person);
            }
            finally
            {
                if (session != null)
                {
                    await session.CloseAsync();
                }
            }
        }

        private async Task<PersonModel> MatchPerson(ITransaction tx, string name)
        {
            var cursor =
                await tx.RunAsync(
                    "MATCH (person:Person) WHERE person.name = $name OPTIONAL MATCH (person)-[:ACTED_IN]->(movie) RETURN person, collect(movie) AS movies",
                    new {name});

            var found = await cursor.ToListAsync(record => new PersonModel(record));

            return found.SingleOrDefault();
        }
    }
}
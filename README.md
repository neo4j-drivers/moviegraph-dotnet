# Moviegraph

Moviegraph is a demo and training application for ASP.NET Core and Neo4j.


## Setup 1: Install Neo4j
```
wget http://dist.neo4j.org/neo4j-community-3.4.7-unix.tar.gz
tar xf neo4j-community-3.4.7-unix.tar.gz
cd neo4j-community-3.4.7
bin/neo4j-admin set-initial-password password
bin/neo4j start|console
```


## Setup 2: Install a data set
```
Browse to
http://localhost:7474/
:play movies
call db.schema
(explore a bit)
```


## Setup 3: Install application skeleton
```
git clone ...
export ASPNETCORE_ENVIRONMENT=Development
cd MovieGraph.Web && dotnet run
```
If you want to use a different language, feel free to convert the code.


## Setup 4: Open the application
```
http://127.0.0.1:5000
```


## Code walk 1: Project structure

- `Answers/` - all the answers!!
- `MovieGraph.sln` - the solution file that includes the project `MovieGraph.Web`
- `MovieGraph.Web/` - the project itself
- `MovieGraph.Web/Controllers/` - the MVC controllers
- `MovieGraph.Web/Views/` - the MVC views (razor)
- `MovieGraph.Web/Model/` - the MVC models
- `MovieGraph.Web/Helpers/` - couple of extension methods for Neo4j
- `MovieGraph.Web/wwwroot/` - the static files (css)


## Code walk 2: Neo4j driver
The driver is added as a service - `Startup.cs`
```csharp
// Add Neo4j Driver As A Singleton Service
services.AddSingleton<IDriver>(provider => GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "password")));
```

## Code walk 3: MVC Controllers
MVC controllers accept `IDriver` in their constructors, ASP.NET MVC passes our already registered singleton instance. 
```csharp
private readonly IDriver driver;

public HomeController(IDriver driver)
{
    this.driver = driver;
}
```

## Code walk 4: Home page

You can have a look at the home page view templates at `Views/Home`.

```csharp
[Route("")]
public async Task<IActionResult> Index(string q)
{
    return View("Index", await MatchMovies(q));
}
```

## Code walk 5: `MatchMovies` function
```csharp
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
                    "MATCH (movie:Movie) WHERE toLower(movie.title) " +
                    "CONTAINS toLower($term) RETURN movie",
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
```

## Code walk 6: Movie page

You can have a look at the home page view templates at `Views/Movie`.

```csharp
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
```

## Code walk 7: `MatchMovie` function
```csharp
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
```

## Code walk 8: `MovieModel` model

`GetOrDefault` functions used are defined as extension methods by the static classes in `Helpers` folder.

```csharp
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
```

## Code walk 9: `PersonModel` model

`GetOrDefault` functions used are defined as extension methods by the static classes in `Helpers` folder.

```csharp
public class PersonModel
{
    public const string NameKey = "name";

    public PersonModel(INode node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        Name = node.GetOrDefault<string>(NameKey, null);
    }

    public string Name { get; }
}
```

## Exercise 1: Add `Person` pages
- Add links behind the movie cast list (movies are rendered by templates in `Views/Movie`)
- Add a new `Person` controller (in `Controllers`)
- Modify `PersonModel` to include year of birth and all films played in (in `Model`)
- Add a new view template for `Person` (save new view file(s) in `Views/Person`)

```bash
cd Answers/1/MovieGraph.Web && dotnet run
```

## Exercise 2: Add star ratings
- Add clickable star rating to each movie (movies are rendered by templates in `Views/Movie`)
- Add handler for POST requests to `/movie/<title>` (in `Controllers/MovieController`)
- Modify `MovieModel` to include stored star rating (in `Model`)

```bash
cd Answers/2/MovieGraph.Web && dotnet run
```
 
## Exercise 3: Improve search page
- Add stars to search results (search results are rendered by templates in `Views/Home`)
- Add _order by_ box (search box is rendered in `Views/Shared/_Layout.cshtml`)

```bash
cd Answers/3/MovieGraph.Web && dotnet run
```

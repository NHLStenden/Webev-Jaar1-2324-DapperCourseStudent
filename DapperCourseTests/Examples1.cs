using Dapper;
using FluentAssertions;
using MySqlConnector;

namespace DapperCourseTests;

public class Examples1
{
    private static readonly string ConnectionString;
    static Examples1()
    {
        ConnectionString = ConnectionStrings.GetConnectionStringMovies();
    }
    
    // Create the movies database and tables using the movies-db/CreateMoviesDb.sql in the SQL folder.
    // Load the data into the database, by executing the movies-db/InsertMovies.sql script in the SQL folder.
    // This is an Example to make sure that you can connect to the database.
    // The Dapper package is already installed in the project. Otherwise you can install it using the NuGet Package Manager or the Package Manager Console.
    // You can use the NuGet Package Manager or the Package Manager Console to install MySqlConnector (Database provider for mysql, sometimes called connector or driver)
    // https://www.learndapper.com/database-providers (go to MySql section)
    // In the Package Manager Console, type: Install-Package MySqlConnector
    // If you are using Rider, you can use the NuGet tool window.
    // If you are using Visual Studio, you can use the NuGet Package Manager.
    // You can find the connection string for mysql here: https://www.connectionstrings.com/mysql/ .
    // Make a database connection to the Movies database. The password that I always use for development is Test@1234!
    // (maybe you need to change this, if you have another password)
    // You can test if you can connect to the database by executing the Example0() test in DatabaseTests/Examples1.cs.
    public bool Example0()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        connection.Open();
        bool result = connection.QuerySingle<bool>("SELECT 1 = 1");
        return result;
    }
    
    [Test]
    public void TestExample0()
    {
        bool result = Example0();
        result.Should().BeTrue();
    }
    
    
    //First we will start with some simple queries, that only return one single value (1 row with 1 column).
    //This is called a scalar value.
    //https://www.learndapper.com/dapper-query/selecting-scalar-values

    
    // Write a SQL query to find the number of movies in the database.
    // use the ExecuteScalar() method, that	Returns the first column of the first row as a dynamic type.
    public object? ExampleScalar1()
    {
        string sql = "SELECT COUNT(*) FROM Movies";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        object? count = connection.ExecuteScalar(sql);
        return count;
    }
    
    [Test]
    public void TestExampleScalar1()
    {
        object? count = ExampleScalar1();
        count.Should().Be(28);
    }

    // Write a SQL query to count the number of female actors in the database
    // Use the ExecuteScalar<T> method, that returns the first column of the first row as the specified T type parameter.
    // A method that returns a strongly typed value is always better than a method that returns a dynamic type!
    public int ExampleScalar2()
    {
        string sql = "SELECT COUNT(*) FROM Actors WHERE Gender = 'M' ";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        int count = connection.ExecuteScalar<int>(sql);
        return count;
    }
    
    [Test]
    public void TestExampleScalar2()
    {
        int count = ExampleScalar2();
        count.Should().Be(17);
    }
    
    // Of course we can select a different kind of scalar value, like a string, boolean, etc.
    // Are there any reviews that don't have a name (NULL) or an empty string.
    // Write a SQL query to find out.
    public bool ExampleScalar3()
    {
        string sql = "SELECT COUNT(*) FROM Reviewers WHERE Name IS NULL OR Name = ''";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        bool count = connection.ExecuteScalar<bool>(sql);
        return count;
    }
    
    [Test]
    public void TestExampleScalar3()
    {
        bool count = ExampleScalar3();
        count.Should().BeTrue();
    }
    
    // Write a SQL query to find the number of movies that have a rating higher (stars) than 8.5.
    public int ExampleScalar4()
    {
        string sql = "SELECT COUNT(*) FROM Ratings WHERE Stars > 8.5";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        int count = connection.ExecuteScalar<int>(sql);
        return count;
    }
    
    [Test]
    public void TestExampleScalar4()
    {
        int count = ExampleScalar4();
        count.Should().Be(1);
    }
    
    // Write a SQL query to find the actor full name (FirstName + ' ' + LastName) which preforms in the most movies.
    // To concatenate strings in MySql use the CONCAT function.
    public string? ExampleScalar5()
    {
        string sql = @"SELECT CONCAT(FirstName, ' ', LastName) FROM Actors WHERE ActorId = 
                            (SELECT ActorId FROM MovieCasts GROUP BY ActorId ORDER BY COUNT(*) LIMIT 1)";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string? name = connection.ExecuteScalar<string>(sql);
        return name;
    }
    
    [Test]
    public void TestExampleScalar5()
    {
        string? name = ExampleScalar5();
        name.Should().NotBeNull();
        name.Should().Be("James Stewart");
    }
    
    // One more example with scalar values.
    // Which movie (return title) has the highest average rating?
    public string? ExampleScalar6()
    {
        string sql = @"SELECT Title FROM Movies WHERE MovieId = 
                            (SELECT MovieId FROM Ratings GROUP BY MovieId ORDER BY AVG(Stars) DESC LIMIT 1)";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string? title = connection.ExecuteScalar<string>(sql);
        return title;
    }
    
    [Test]
    public void TestExampleScalar6()
    {
        string? title = ExampleScalar6();
        title.Should().NotBeNull();
        title.Should().Be("The Usual Suspects");
    }
    
    // Now we will start with some simple queries, that return one row with multiple columns.
    // https://www.learndapper.com/dapper-query/selecting-single-rows
    // There are many methods that can be used to return a single row with multiple columns. The are subtle differences between them!
    // We will not look at the methods that return a dynamic type, because they are not type safe!
    // We will look at the methods that return a strongly typed object.
    
    // Write a SQL query to find the first and last name of the actor with ActorId = 1.
    // Use the query method QuerySingle<T> that returns a single row with multiple columns.
    public class QuerySingleResult1
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
    }
    
    public QuerySingleResult1 ExampleQuerySingle()
    {
        string sql = "SELECT FirstName, LastName FROM Actors WHERE ActorId = 101";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        QuerySingleResult1 actor = connection.QuerySingle<QuerySingleResult1>(sql);
        return actor;
    }
    
    [Test]
    public void TestExampleQuerySingle()
    {
        QuerySingleResult1 actor = ExampleQuerySingle();
        actor.Firstname.Should().Be("James");
        actor.Lastname.Should().Be("Stewart");
    }
    
    // Write a SQL query to find the first and last name of the director and the year and duration
    // from movie with name 'American Beauty'.
    // Use the query method QuerySingle<T> that returns a single row with multiple columns.
    // Create a class QuerySingleRowResult2 with the properties that are required and of the correct type.
    public class QuerySingleResult2
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Year { get; set; }
        public int Duration { get; set; }
    }
    
    public QuerySingleResult2 ExampleQuerySingle2()
    {
        string sql = @"SELECT d.FirstName, d.LastName, m.Year, m.Duration FROM Directors d JOIN DirectorMovie dm ON d.DirectorId = dm.DirectorsDirectorId JOIN Movies m ON dm.MoviesMovieId = m.MovieId WHERE m.Title = 'American Beauty'";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        QuerySingleResult2 movie = connection.QuerySingle<QuerySingleResult2>(sql);
        return movie;
    }
    
    [Test]
    public void TestExampleQuerySingle2()
    {
        QuerySingleResult2 movie = ExampleQuerySingle2();
        movie.FirstName.Should().Be("Sam");
        movie.LastName.Should().Be("Mendes");
        movie.Year.Should().Be(1999);
        movie.Duration.Should().Be(122);
    }
    
    // QuerySingle<T> throws an exception if the query returns zero or more than one row.
    // Let write a query that returns Title, ReleaseDate  for a movie that doesn't exist. The movie with title = 'Does not exist', doesn't exist.
    public class QuerySingleResult3
    {
        public string Title { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
    }
    
    public QuerySingleResult3 ExampleQuerySingle3()
    {
        string sql = "SELECT Title FROM Movies WHERE Title = 'Does not exist'";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        QuerySingleResult3 result = connection.QuerySingle<QuerySingleResult3>(sql);
        return result;
    }
    
    [Test]
    public void TestExampleQuerySingle3()
    {
        Action act = () => ExampleQuerySingle3();
        act.Should().Throw<InvalidOperationException>();
    }
    
    // QuerySingle<T> throws an exception if the query returns zero or more than one row.
    // Let write a query that return all movies with the language 'English'. (this is a query that returns multiple rows)
    // Select the Title, Language
    public class QuerySingleResult4
    {
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;
    }
    public QuerySingleResult4 ExampleQuerySingle4()
    {
        string sql = "SELECT Title, Language FROM Movies WHERE Language = 'English'";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        QuerySingleResult4 result4 = connection.QuerySingle<QuerySingleResult4>(sql);
        return result4;
    }
    
    [Test]
    public void TestExampleQuerySingle4()
    {
        Action act = () => ExampleQuerySingle4();
        act.Should().Throw<InvalidOperationException>();
    }
    
    // QuerySingleOrDefault<T> throws an exception if the query returns more than one row.
    // Use when zero or one row is expected to be returned. Returns an instance of the type specified by the T type parameter or null
    // Let write a query for a movie that doesn't exist. The movie with title = 'Does not exist', doesn't exist.
    // Select the Title, Language
    // This returns a null!
    public class QuerySingleOrDefaultResult1
    {
        public string Title { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
    }
    public QuerySingleOrDefaultResult1? ExampleQuerySingleOrDefault1()
    {
        string sql = "SELECT Title FROM Movies WHERE Title = 'Does not exist'";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        QuerySingleOrDefaultResult1? result = connection.QuerySingleOrDefault<QuerySingleOrDefaultResult1>(sql);
        return result;
    }
    
    [Test]
    public void TestExampleQuerySingleOrDefault1()
    {
        QuerySingleOrDefaultResult1? result = ExampleQuerySingleOrDefault1();
        result.Should().BeNull();
    }
    
    // Sometimes the query returns multiple rows and we only need one row.
    // QueryFirst<T> returns the first row of the query result. If the query returns no rows, an exception is thrown.
    // Let write a query that return all movies with the language 'English'. (this is a query that returns multiple rows)
    // Select the Title, Language
    // Order the result by Title alphabetically
    public class ExampleQueryFirst1
    {
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;   
    }
    
    public ExampleQueryFirst1 ExampleQueryFirst()
    {
        string sql = "SELECT Title, Language FROM Movies WHERE Language = 'English' ORDER BY Title";
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        ExampleQueryFirst1 result = connection.QueryFirst<ExampleQueryFirst1>(sql);
        return result;
    }
    
    [Test]
    public void TestExampleQueryFirst()
    {
        ExampleQueryFirst1 result = ExampleQueryFirst();
        result.Title.Should().Be("Aliens");
        result.Language.Should().Be("English");
    }
    
    
    // Write a SQL query to find the name and year of the movies.
    // Use the correct Dapper method and return a IEnumerable<dynamic> object.
    // Don't worry about the return type, we will fix that in the next example.
    public IEnumerable<dynamic> Example1()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<dynamic> movies = connection.Query("SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10");
        return movies;
    }
    
    [Test]
    public void TestExample1()
    {
        IEnumerable<dynamic> movies = Example1();
        
        // Take a look at the warning, this indicates that there is a possibility of multiple enumeration.
        // This is a good reason why not to use IEnumerable<X> as a return type.
        movies.Should().HaveCount(10);
        
        //notice the casting to string or int (this is a also a good example why you should not use dynamic)!
        ((string)movies.First().Title).Should().Be("Aliens");
        ((int)movies.First().Year).Should().Be(1986);
    }
    
    // It's not a good practice to use an IEnumerable<dynamic> as in the previous example. Let's fix that.
    // Write a SQL query to find the name and year of the movies. This time return a list of Movie objects.
    // Use the correct Dapper method and create a class named ResultExample1_1 with the properties Title and Year.
    // Use to correct type for the properties.
    public class ResultExample1BAndC
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
    }
    
    public IEnumerable<ResultExample1BAndC> Example1B()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<ResultExample1BAndC> movies = connection.Query<ResultExample1BAndC>("SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10");
        return movies;
    }
    
    [Test]
    public void TestExample1B()
    {
        IEnumerable<ResultExample1BAndC> movies = Example1B();
        movies.Should().HaveCount(10);
        movies.First().Title.Should().Be("Aliens");
        movies.First().Year.Should().Be(1986);
    }
    
    // In the previous example the return type IEnumerable<ResultExample1BAndC> is not a good practice, because it's not type safe!
    // Change this to a List<ResultExample1BAndC> instead. Use the same Dapper method as in the previous example.
    // The is a method To....() that can be used to convert an IEnumerable<T> to a List<T>.
    // 'Dot into' the IEnumerable<ResultExample1BAndC> and press Ctrl + . to see the available methods.
    // If you look at the return types should be able to find the correct method.
    public List<ResultExample1BAndC> Example1C()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<ResultExample1BAndC> movies = connection.Query<ResultExample1BAndC>("SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10");
        return movies.ToList();
    }
    
    [Test]
    public void TestExample1C()
    {
        List<ResultExample1BAndC> movies = Example1C();
        movies.Should().HaveCount(10);
        movies.First().Title.Should().Be("Aliens");
        movies.First().Year.Should().Be(1986);
    }
    
    
    // Write a SQL query to find when the movie 'American Beauty' was released. Return the movie release year, as an object?
    // Use the correct Dapper method and return a object?. Don't worry about the return type, we will fix that in the next example.
    public object? Example2()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        object? year = connection.ExecuteScalar("SELECT Year FROM Movies WHERE Title = 'American Beauty'");
        
        return year;
    }
    
    [Test]
    public void TestExample2()
    {
        object? year = Example2();
        year.Should().Be(1999);
    }
    
    // In the previous example the return type object?, is not a good practice, because it's not type safe!
    // Write a SQL query to find when the movie 'American Beauty' was released. Return the movie release year.
    // This time, use the correct return type. Don't cast the result to int, use the correct Dapper method instead.
    public int Example2_1()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        int year = connection.ExecuteScalar<int>("SELECT Year FROM Movies WHERE Title = 'American Beauty'");
        
        return year;
    }
    
    [Test]
    public void TestExample2_1()
    {
        int year = Example2_1();
        year.Should().Be(1999);
    }
    
    // Write a SQL query to find all the years when a Japanese (Language) movie was released. Return movie release years.
    // Order the result by year.
    public List<int> Example3()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<int> years = connection.Query<int>("SELECT Year FROM Movies WHERE Language = 'Japanese' ORDER BY Year");
        return years.ToList();
    }
    
    [Test]
    public void TestExample3()
    {
        List<int> years = Example3();
        years.Should().HaveCount(3);
        years.Should().ContainInOrder(1954, 1997, 2001);
    }
    
    // //Which actors played in the movie with the highest average rating?
    // // Write a SQL query to find all the actors that played in the movie with the highest average rating.
    // public List<string> Example4()
    // {
    //     string sql = @"SELECT a.FirstName, a.LastName FROM Actors a JOIN Ac am ON a.ActorId = am.ActorsActorId JOIN Movies m ON am.MoviesMovieId = m.MovieId JOIN Ratings r ON m.MovieId = r.MovieId
    //                     WHERE m.MovieId = (SELECT m.MovieId FROM Movies m JOIN Ratings r ON m.MovieId = r.MovieId GROUP BY m.MovieId ORDER BY AVG(r.Stars) DESC LIMIT 1)";
    //     using var connection = new MySqlConnection(_connectionString);
    //     var actors = connection.Query<string>(sql);
    //     return actors.ToList();
    // }
    
    
    // Write a SQL query to find all the titles of the movies directed by 'Kevin Spacy'. Return movie titles.
    // Order the result alphabetically. It's always a good idea to return a List<T> instead of IEnumerable<T> when using Dapper.
    public List<string> Example4()
    {
        string sql = @"   SELECT Title 
                    FROM 
                        Movies m JOIN 
                            DirectorMovie dm ON m.MovieId = dm.MoviesMovieId JOIN    
                                Directors d ON dm.DirectorsDirectorId = d.DirectorId                        
                    WHERE d.FirstName = 'Kevin' AND d.LastName = 'Spacey' ORDER BY Title";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        List<string> titles = connection.Query<string>(sql).ToList();
        return titles;
    }
    
    [Test]
    public void TestExample4()
    {
        List<string> titles = Example4();
        titles.Should().HaveCount(1);
        titles.First().Should().Be("Beyond the Sea");
    }


    public class Movie
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public string ReleaseCountryCode { get; set; } = null!;
    }


    public Movie? GetMovieById(int movieId )
    {
        string sql = @"SELECT Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode
                        FROM Movies WHERE MovieId = @movieId";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        var movie = connection.QuerySingleOrDefault<Movie>(sql, new { movieId });
        return movie;
    }
    
    [Test]
    public void TestGetMovieById()
    {
        Movie? movie = GetMovieById(922);
        movie.Should().NotBeNull();
        movie!.Title.Should().Be("Aliens");
        movie.Year.Should().Be(1986);
        movie.Duration.Should().Be(137);
        movie.Language.Should().Be("English");
        movie.ReleaseDate.Should().Be(new DateTime(1986, 8, 29));
        movie.ReleaseCountryCode.Should().Be("UK");
    }
}
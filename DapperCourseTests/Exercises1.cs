using Dapper;
using DapperCourse;
using FluentAssertions;
using MySql.Data.MySqlClient;

namespace DapperCourseTests;


public class Exercises1
{
    // Create the movies database and tables using the createMovies.sql in the SQL folder.
    // Load the data into the database, by executing the insertMovies.sql script in the SQL folder.
    // This is aa exercises to make sure that you can connect to the database.
    // You can use the NuGet Package Manager or the Package Manager Console to install MySql.Data. (Database provider for mysql, sometimes called connector or driver)
    // https://www.learndapper.com/database-providers (go to MySql section)
    // In the Package Manager Console, type: Install-Package MySql.Data
    // If you are using Rider, you can use the NuGet tool window.
    // If you are using Visual Studio, you can use the NuGet Package Manager.
    // You can find the connection string for mysql here: https://www.connectionstrings.com/mysql/ .
    // Make a database connection to the Movies database. The password that I always use for development is Test@1234!
    // (maybe you need to change this, if you have another password)
    // You can test if you can connect to the database by executing the Exercise0Test() test.
    
    // Important note: the name of the database can be case sensitive.
    // This depends on the operating system on which you are running mysql.
    // This is also true for the table names and column names.
    
    // If you are using Windows, the database name, table names are not case sensitive.
    // If you are using Linux, the database name, table names are case sensitive.
    // If you are using Mac, the database name, table names are case sensitive.
    // The problem is make sure the case is correct in the SQL because the deployment can be done on a different operating system.
    
    // Rider can help you with this. If you type the name of a table or column, it will show you the correct case.
    // Rider can inspect the database schema (structure of database, such as tables, columns, views, etc.)
    // and assist you with writing correct SQL and also the case sensitivity of the table and column names.
    private string GetConnectionString()
    {
        return "server=localhost;port=3306;database=Movies;user=root;password=Test@1234!";
    }
    
    public bool Exercise0()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        connection.Open();
        bool result = connection.QuerySingle<bool>("SELECT 1 = 1");
        return result;
    }
    
    [Test]
    public void Exercise0Test()
    {
        // Arrange®
        var sut = new Exercises1();
        
        // Act
        bool result = sut.Exercise0();
        
        // Assert
        result.Should().BeTrue();
    }
    
    //First we will start with some simple queries, that only return one single value (1 row with 1 column).
    //This is called a scalar value.
    //https://www.learndapper.com/dapper-query/selecting-scalar-values

    
    // Write a SQL query to find the number of movies in the database.
    // use the ExecuteScalar() method, that	Returns the first column of the first row as a dynamic type.
    // cast it to the correct type. There is a small problem with this method,
    // it returns an object? (nullable object).
    // This is because the ExecuteScalar() method can return null.
    // We know that the query will always return a value, so we can cast it to the correct type.
    // In the next exercise we will use the ExecuteScalar<T> method, this is a better method,
    // because it returns a strongly typed value!
    public int ExerciseScalar1()
    {
        string sql = "SELECT COUNT(*) FROM Movies";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var count = connection.ExecuteScalar(sql);
        return Convert.ToInt32(count);
    }
    
    [Test]
    public void ExerciseScalar1Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        int count = sut.ExerciseScalar1();
        
        // Assert
        count.Should().Be(28);
    }
    
    // Write a SQL query to count the number of female actors in the database
    // Use the ExecuteScalar<T> method, that returns the first column of the first row as the specified T type parameter
    // A method that returns a strongly typed value is always better than a method that returns a dynamic type!
    public int ExerciseScalar2()
    {
        string sql = "SELECT COUNT(*) FROM Actors WHERE Gender = 'M' ";
        using var connection = new MySqlConnection(GetConnectionString());
        var count = connection.ExecuteScalar<int>(sql);
        return count;
    }
    
    [Test]
    public void ExerciseScalar2Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        int count = sut.ExerciseScalar2();
        
        // Assert
        count.Should().Be(10);
    }
    
    // Of course we an select a different kind of scalar value, like a string, boolean, etc.
    // Are there any reviews that don't have a name (NULL) or an empty string.
    // Write a SQL query to find out.
    public bool ExerciseScalar3()
    {
        string sql = "SELECT COUNT(*) > 0 FROM Reviewers WHERE Name IS NULL OR Name = ''";
        using var connection = new MySqlConnection(GetConnectionString());
        var count = connection.ExecuteScalar<bool>(sql);
        return count;
    }
    
    [Test]
    public void ExerciseScalar3Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        bool result = sut.ExerciseScalar3();
        
        // Assert
        result.Should().BeTrue();
    }
    
    // Write a SQL query to find the number of movies that have a rating higher (stars) than 8.5.
    public int ExerciseScalar4()
    {
        string sql = "SELECT COUNT(*) FROM Ratings WHERE Stars > 8.5";
        using var connection = new MySqlConnection(GetConnectionString());
        var count = connection.ExecuteScalar<int>(sql);
        return count;
    }
    
    [Test]
    public void ExerciseScalar4Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        int count = sut.ExerciseScalar4();
        
        // Assert
        count.Should().Be(1);
    }
    
    // Write a SQL query to find the actor full name (FirstName + ' ' + LastName) which preforms in the most movies.
    // To concatenate strings in MySql use the CONCAT function.
    public string ExerciseScalar5()
    {
        string sql = @"SELECT CONCAT(FirstName, ' ', LastName) FROM Actors WHERE ActorId = 
                            (SELECT ActorId FROM MovieCasts GROUP BY ActorId ORDER BY COUNT(*) LIMIT 1)";
        using var connection = new MySqlConnection(GetConnectionString());
        var name = connection.ExecuteScalar<string>(sql);
        return name;
    }
    
    [Test]
    public void ExerciseScalar5Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        string name = sut.ExerciseScalar5();
        
        // Assert
        name.Should().Be("James Stewart");
    }
    
    // One more exercise with scalar values. Which movie (return title) has the highest average rating?
    public string ExerciseScalar6()
    {
        string sql = @"SELECT Title FROM Movies m JOIN Ratings r ON m.MovieId = r.MovieId
                        GROUP BY Title
                        ORDER BY AVG(Stars) DESC
                        LIMIT 1";
        using var connection = new MySqlConnection(GetConnectionString());
        var title = connection.ExecuteScalar<string>(sql);
        return title;
    }
    
    [Test]
    public void ExerciseScalar6Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        string title = sut.ExerciseScalar6();
        
        // Assert
        title.Should().Be("The Usual Suspects");
    }
    
    // Now we will start with some simple queries, that return one row with multiple columns.
    // https://www.learndapper.com/dapper-query/selecting-single-rows
    // There are many methods that can be used to return a single row with multiple columns. The are subtle differences between them!
    // We will not look at the methods that return a dynamic type, because they are not type safe!
    // We will look at the methods that return a strongly typed object.
    
    // Write a SQL query to find the first and last name of the actor with ActorId = 101.
    // Use the query method QuerySingle<T> that returns a single row with multiple columns.
    public class QuerySingleResult1
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
    }
    
    public QuerySingleResult1 ExercisesQuerySingle()
    {
        string sql = "SELECT FirstName, LastName FROM Actors WHERE ActorId = 101";
        using var connection = new MySqlConnection(GetConnectionString());
        var actor = connection.QuerySingle<QuerySingleResult1>(sql);
        return actor;
    }
    
    [Test]
    public void ExercisesQuerySingleTest()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var actor = sut.ExercisesQuerySingle();
        
        // Assert
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
    
    public QuerySingleResult2 ExercisesQuerySingle2()
    {
        string sql = @"SELECT d.FirstName, d.LastName, m.Year, m.Duration FROM Directors d JOIN DirectorMovie dm ON d.DirectorId = dm.DirectorsDirectorId JOIN Movies m ON dm.MoviesMovieId = m.MovieId WHERE m.Title = 'American Beauty'";
        using var connection = new MySqlConnection(GetConnectionString());
        var movie = connection.QuerySingle<QuerySingleResult2>(sql);
        return movie;
    }
    
    [Test]
    public void ExercisesQuerySingle2Test()
    {
        // Arrange
        var exercises1 = new Exercises1();
        
        // Act
        var movie = exercises1.ExercisesQuerySingle2();
        
        // Assert
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
    
    public QuerySingleResult3 ExerciseQuerySingle3()
    {
        string sql = "SELECT Title FROM Movies WHERE Title = 'Does not exist'";
        using var connection = new MySqlConnection(GetConnectionString());
        var result = connection.QuerySingle<QuerySingleResult3>(sql);
        return result;
    }
    
    [Test]
    public void ExerciseQuerySingle3Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act & Assert
        Assert.Catch(() => sut.ExerciseQuerySingle3());
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
    public QuerySingleOrDefaultResult1 ExerciseQuerySingleOrDefault1()
    {
        string sql = "SELECT Title FROM Movies WHERE Title = 'Does not exist'";
        using var connection = new MySqlConnection(GetConnectionString());
        var result = connection.QuerySingleOrDefault<QuerySingleOrDefaultResult1>(sql);
        return result;
    }
    
    [Test]
    public void ExerciseQuerySingleOrDefault1Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var result = sut.ExerciseQuerySingleOrDefault1();
        
        // Assert
        result.Should().BeNull();
    }
    
    // Sometimes the query returns multiple rows and we only need one row.
    // QueryFirst<T> returns the first row of the query result. If the query returns no rows, an exception is thrown.
    // Let write a query that return all movies with the language 'English'. (this is a query that returns multiple rows)
    // Select the Title, Language
    // Order the result by Title alphabetically
    public class ExerciseQueryFirst1
    {
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;   
    }
    
    public ExerciseQueryFirst1 ExerciseQueryFirst()
    {
        string sql = "SELECT Title, Language FROM Movies WHERE Language = 'English' ORDER BY Title";
        using var connection = new MySqlConnection(GetConnectionString());
        var result = connection.QueryFirst<ExerciseQueryFirst1>(sql);
        return result;
    }
    
    [Test]
    public void ExerciseQueryFirstTest()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var result = sut.ExerciseQueryFirst();
        
        // Assert
        result.Title.Should().Be("Aliens"); // good movie :)
        result.Language.Should().Be("English");
    }
    
    // Write a SQL query to find the name and year of the movies.
    // Use the correct Dapper method and return a IEnumerable<dynamic> object.
    // Don't worry about the return type, we will fix that in the next exercise.
    public IEnumerable<dynamic> ExerciseQueryDynamic()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.Query("SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10");
        return movies;
    }
    
    [Test]
    public Task ExerciseQueryDynamicTest()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var movies = sut.ExerciseQueryDynamic();
        
        // Assert
        movies.Should().HaveCount(10);

        return Verify(movies);
    }
    
    // It's not a good practice to use an IEnumerable<dynamic> as in the previous exercise. Let's fix that.
    // Write a SQL query to find the name and year of the movies. This time return a list of Movie objects.
    // Use the correct Dapper method and create a class named ResultExerciseQuery with the properties Title and Year.
    // Use the correct types for the properties.
    public class ResultExerciseQuery
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
    }
    
    public IEnumerable<ResultExerciseQuery> ExerciseQuery()
    {
        var sql = """
                      SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10
                  """;
            
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.Query<ResultExerciseQuery>(sql);
        return movies;
    }
    
    [Test]
    public Task ExerciseQueryTest()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var movies = sut.ExerciseQuery();
        
        // Assert
        movies.Should().HaveCount(10);

        return Verify(movies);
    }
    
    
    // Write a SQL query to find all the titles of the movies directed by 'Kevin Spacy'. Return movie titles.
    // Order the result alphabetically. It's always a good idea to return a List<T> instead of IEnumerable<T> when using Dapper.
    public List<string> ExerciseQuery2()
    {
        string sql = 
            """
                 SELECT Title
                 FROM
                     Movies m JOIN
                     DirectorMovie dm ON m.MovieId = dm.MoviesMovieId JOIN
                     Directors d ON dm.DirectorsDirectorId = d.DirectorId
                 WHERE d.FirstName = 'Kevin' AND d.LastName = 'Spacey' ORDER BY Title
            """;
        
        using var connection = new MySqlConnection(GetConnectionString());
        var titles = connection.Query<string>(sql).ToList();
        return titles;
    }
    
    [Test]
    public void ExerciseQuery2Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var titles = sut.ExerciseQuery2();
        
        // Assert
        titles.Should().ContainInOrder("Beyond the Sea");
    }
    
    
    // Write a SQL query to get a movie By movieId, return an Movie Object that contains the Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode.
    // Use the correct Dapper method. This method should return a single row with multiple columns and throws an exception if the query returns zero or more than one row.
    // Create a class named Movie with the properties Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode.
    public class Movie
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public string ReleaseCountryCode { get; set; } = null!;
    }


    public Movie GetMovieById(int movieId )
    {
        string sql = @"SELECT Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode
                        FROM Movies WHERE MovieId = @movieId";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.QuerySingle<Movie>(sql, new { movieId });
        return movies;
    }
    
    [Test]
    public void GetMovieByIdTest()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var movie = sut.GetMovieById(922);
        
        // Assert
        movie.Title.Should().Be("Aliens");
        movie.Year.Should().Be(1986);
        movie.Duration.Should().Be(137);
        movie.Language.Should().Be("English");
        movie.ReleaseDate.Should().Be(new DateTime(1986, 8, 29));
        
        //Act & Assert
        Assert.Catch(() => sut.GetMovieById(999999));
    }
    
    // Write a SQL query to get a movie By movieId, return an Movie Object that contains the Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode.
    // Use the correct Dapper method. This time GetMovieById2() returns a nullable Movie object.
    // If the movie doesn't exist, return null.
    public Movie? GetMovieById2(int movieId )
    {
        string sql = @"SELECT Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode
                        FROM Movies WHERE MovieId = @movieId";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.QuerySingleOrDefault<Movie>(sql, new { movieId });
        return movies;
    }
    
    [Test]
    public void GetMovieById2Test()
    {
        // Arrange
        var sut = new Exercises1();
        
        // Act
        var movie = sut.GetMovieById2(922);
        
        // Assert
        movie.Should().NotBeNull();
        movie!.Title.Should().Be("Aliens");
        movie.Year.Should().Be(1986);
        movie.Duration.Should().Be(137);
        movie.Language.Should().Be("English");
        movie.ReleaseDate.Should().Be(new DateTime(1986, 8, 29));
        
        var movie2 = sut.GetMovieById2(999999);
        movie2.Should().BeNull();
    }
}
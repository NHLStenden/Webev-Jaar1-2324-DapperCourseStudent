using Dapper;
using FluentAssertions;
using MySqlConnector;

namespace DapperCourseTests;

public class Exercises1
{
    // Create the movies database and tables using the createMovies.sql in the SQL folder.
    // Load the data into the database, by executing the insertMovies.sql script in the SQL folder.
    // This is an exercises to make sure that you can connect to the database.
   
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
    
    // Rider can help you with this!!! If you type the name of a table or column, it will show you the correct case.
    // Rider can inspect the database schema (structure of database, such as tables, columns, views, etc.)
    // and assist you with writing correct SQL and also the case sensitivity of the table and column names.
    private readonly string _connectionString;
    public Exercises1() 
    {
        _connectionString = ConnectionStrings.GetConnectionStringMovies();
    }

    public bool Exercise0()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void Exercise0Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        bool result = sut.Exercise0();
        
        // Assert
        result.Should().BeTrue();
        // Assert.AreEqual(true, result); 
    }
    
    // First we will start with some simple queries, that only return one single value (1 row with 1 column).
    // This is called a scalar value.
    // https://www.learndapper.com/dapper-query/selecting-scalar-values
    // 
    // Write a SQL query to find the number of movies in the database.
    // Use the ExecuteScalar() method, this	returns the first column of the first row as a dynamic type.
    // Cast it to the correct type. There is a small problem with this method,
    // it returns an object? (nullable object). Object? means an object or null type.
    // This is because the ExecuteScalar() method can return null. 
    // We know that the query will always returns a value, so we can cast it to the correct type (Convert.ToInt32(...)).
    // In the next exercise we will use the ExecuteScalar<T>() method, this is a better method,
    // because it returns a strongly typed value!
    public int ExerciseScalar1()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseScalar1Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        int count = sut.ExerciseScalar1();
        
        // Assert
        count.Should().Be(28);
    }
    
    // Write a SQL query to count the number of female actors in the database.
    // Use the ExecuteScalar<T>(sql) method, that returns the first column of the first row as the specified T type parameter.
    // A method that returns a strongly typed value is always better than a method that returns a dynamic type!
    public int ExerciseScalar2()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void ExerciseScalar2Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        int count = sut.ExerciseScalar2();
        
        // Assert
        count.Should().Be(7);
    }
    
    // Of course we can select a different type of scalar value, like a string, boolean, etc.
    // Are there any reviewers with an empty name or no name (null)?
    // Try to use the ExecuteScalar<bool>() method to return a boolean.
    // Write a SQL query to find out.
    public bool ExerciseScalar3()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseScalar3Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        bool result = sut.ExerciseScalar3();
        
        // Assert
        result.Should().BeTrue();
    }
    
    // Write a SQL query to find the number of movies that have a rating higher (stars) than 8.5.
    public int ExerciseScalar4()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseScalar4Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        int count = sut.ExerciseScalar4();
        
        // Assert
        count.Should().Be(1);
    }
    
    // Write a SQL query to find the actor full name (FirstName , ' ' , LastName) who performs in the most movies.
    // To concatenate strings in MySql use the CONCAT function.
    public string ExerciseScalar5()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseScalar5Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        string name = sut.ExerciseScalar5();
        
        // Assert
        name.Should().Be("Kevin Spacey");
    }
    
    // One more exercise with scalar values.
    // Which movie (return title) has the highest average rating?
    public string ExerciseScalar6()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseScalar6Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        string title = sut.ExerciseScalar6();
        
        // Assert
        title.Should().Be("The Usual Suspects");
    }
    
    // Now we will start with some simple queries, that return one row with multiple columns.
    // https://www.learndapper.com/dapper-query/selecting-single-rows
    // There are many methods that can be used to return a single row with multiple columns.
    // The are subtle differences between them!
    // We will not look at the methods that return a dynamic type, because they are not type safe!
    // We will look at the methods that return a strongly typed object, i.e. use a generic type parameter <T>.
    
    // Write a SQL query to find the first and last name of the actor with ActorId = 101.
    // Use the query method QuerySingle<T> that returns a single row with multiple columns.
    // In this exercise T is QuerySingleResult1, see below.  
    public class QuerySingleResult1
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
    
    public QuerySingleResult1 ExercisesQuerySingle()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExercisesQuerySingleTest()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        QuerySingleResult1 actor = sut.ExercisesQuerySingle();
        
        // Assert
        actor.FirstName.Should().Be("James");
        actor.LastName.Should().Be("Stewart");
    }
    
    // Write a SQL query to find the first and last name of the director and the year and duration
    // from the movie with the name 'American Beauty'.
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
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExercisesQuerySingle2Test()
    {
        // Arrange
        Exercises1 exercises1 = new Exercises1();
        
        // Act
        QuerySingleResult2 movie = exercises1.ExercisesQuerySingle2();
        
        // Assert
        movie.FirstName.Should().Be("Sam");
        movie.LastName.Should().Be("Mendes");
        movie.Year.Should().Be(1999);
        movie.Duration.Should().Be(122);
    }
    
    // QuerySingle<T>() throws an exception if the query returns zero or more than one row.
    // Let's write a query that returns Title, ReleaseDate for a movie that doesn't exist.
    // The movie with title = 'Does not exist', doesn't exist.
    public class QuerySingleResult3
    {
        public string Title { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
    }
    
    public QuerySingleResult3 ExerciseQuerySingle3()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseQuerySingle3Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        // Act & Assert
        Assert.Catch<InvalidOperationException>(() => sut.ExerciseQuerySingle3());
    }
    
    // QuerySingleOrDefault<T>() throws an exception if the query returns more than one row.
    // Use when zero or one row is expected to be returned.
    // Returns an instance of the type specified by the T type parameter or null
    // Let's write a query for a movie that doesn't exist. The movie with title = 'Does not exist', doesn't exist.
    // Select the Title, Language
    // This returns a null!
    public class QuerySingleOrDefaultResult1
    {
        public string Title { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
    }
    public QuerySingleOrDefaultResult1 ExerciseQuerySingleOrDefault1()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseQuerySingleOrDefault1Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        QuerySingleOrDefaultResult1 result = sut.ExerciseQuerySingleOrDefault1();
        
        // Assert
        result.Should().BeNull();
    }
    
    // Sometimes the query returns multiple rows and we only need one row.
    // QueryFirst<T> returns the first row of the query result.
    // If the query returns no rows, an exception is thrown.
    // Let's write a query that returns all movies with the language 'English'.
    // (this is a query that returns multiple rows)
    // Select the Title, Language
    // Order the result by Title alphabetically
    public class ExerciseQueryFirst1
    {
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;   
    }
    
    public ExerciseQueryFirst1 ExerciseQueryFirst()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseQueryFirstTest()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        ExerciseQueryFirst1 result = sut.ExerciseQueryFirst();
        
        // Assert
        result.Title.Should().Be("Aliens"); // good movie :-)
        result.Language.Should().Be("English");
    }
    
    // Write a SQL query that returns the Title and Year of the first 10 movies ordered by Title.
    // Use the correct Dapper method and return a IEnumerable<dynamic> object.
    // Don't worry about the return type, we will fix that in the next exercise.
    public IEnumerable<dynamic> ExerciseQueryDynamic()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public Task ExerciseQueryDynamicTest()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        IEnumerable<dynamic> movies = sut.ExerciseQueryDynamic();
        
        // Assert
        movies.Should().HaveCount(10);

        return Verify(movies);
    }
    
    // It's not a good practice to use an IEnumerable<dynamic> as in the previous exercise. Let's fix that.
    // Write a SQL query to return the name and year of the first 10 movies Ordered by Title. This time return a list of Movie objects.
    // Use the correct Dapper method (Query<T>(sql)) and create a class named ResultExerciseQuery with the properties Title and Year.
    // Use the correct types for the properties.
    public class ResultExerciseQuery
    {
        public string Title { get; set; } = null!; //remove this as well in the student version
        public int Year { get; set; }
    }
    
    public IEnumerable<ResultExerciseQuery> ExerciseQuery()
    { 
        throw new NotImplementedException();
    }
    
    [Test]
    public Task ExerciseQueryTest()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        IEnumerable<ResultExerciseQuery> movies = sut.ExerciseQuery();
        
        // Assert
        movies.Should().HaveCount(10);

        return Verify(movies);
    }
    
    // Write a SQL query to find all the titles of the movies directed by 'Kevin Spacey'. Return movie titles.
    // Order the result alphabetically. It's always a good idea to return a List<T> instead of IEnumerable<T> when using Dapper.
    public List<string> ExerciseQuery2()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void ExerciseQuery2Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        List<string> titles = sut.ExerciseQuery2();
        
        // Assert
        titles.Should().ContainInOrder("Beyond the Sea");
    }
    
    
    // Write a SQL query to get a movie by movieId, return a Movie Object that contains the Title,
    // Year, Duration, Language, ReleaseDate, ReleaseCountryCode.
    // Use the correct Dapper method. This method should return a single row with multiple columns
    // and throws an exception if the query returns zero or more than one row.
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


    public Movie GetMovieById(int movieId)
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void GetMovieByIdTest()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        Movie movie = sut.GetMovieById(922);
        
        // Assert
        movie.Title.Should().Be("Aliens");
        movie.Year.Should().Be(1986);
        movie.Duration.Should().Be(137);
        movie.Language.Should().Be("English");
        movie.ReleaseDate.Should().Be(new DateTime(1986, 8, 29));
        
        //Act & Assert
        Assert.Catch(() => sut.GetMovieById(999999));
    }
    
    // Write a SQL query to get a movie by movieId, return a Movie Object that contains the Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode.
    // Use the correct Dapper method. This time GetMovieById2() returns a nullable Movie (Movie?) object.
    // If the movie doesn't exist, return null.
    public Movie? GetMovieById2(int movieId )
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public void GetMovieById2Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        Movie? movie = sut.GetMovieById2(922);
        
        // Assert
        movie.Should().NotBeNull();
        movie!.Title.Should().Be("Aliens");
        movie.Year.Should().Be(1986);
        movie.Duration.Should().Be(137);
        movie.Language.Should().Be("English");
        movie.ReleaseDate.Should().Be(new DateTime(1986, 8, 29));
        
        Movie? movie2 = sut.GetMovieById2(999999);
        movie2.Should().BeNull();
    }
}
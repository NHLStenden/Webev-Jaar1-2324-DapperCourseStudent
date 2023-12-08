using Dapper;
using MySql.Data.MySqlClient;

namespace DapperCourse;

public class SimpleQueries
{
    // Create the database movies database and tables using the createMovies.sql in the SQL folder.
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
    // You can test if you can connect to the database by executing the Exercise0() test in DatabaseTests/Exercises1.cs.
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
    
    
    //First we will start with some simple queries, that only return one single value (1 row with 1 column).
    //This is called a scalar value.
    //https://www.learndapper.com/dapper-query/selecting-scalar-values

    
    // Write a SQL query to find the number of movies in the database.
    // use the ExecuteScalar() method, that	Returns the first column of the first row as a dynamic type.
    public object? ExerciseScalar1()
    {
        string sql = "SELECT COUNT(*) FROM Movies";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var count = connection.ExecuteScalar(sql);
        return count;
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
    
    // Of course we an select a different kind of scalar value, like a string, boolean, etc.
    // Are there any reviews that don't have a name (NULL) or an empty string.
    // Write a SQL query to find out.
    public bool ExerciseScalar3()
    {
        string sql = "SELECT COUNT(*) FROM Reviewers WHERE Name IS NULL OR Name = ''";
        using var connection = new MySqlConnection(GetConnectionString());
        var count = connection.ExecuteScalar<bool>(sql);
        return count;
    }
    
    // Write a SQL query to find the number of movies that have a rating higher (stars) than 8.5.
    public int ExerciseScalar4()
    {
        string sql = "SELECT COUNT(*) FROM Ratings WHERE Stars > 8.5";
        using var connection = new MySqlConnection(GetConnectionString());
        var count = connection.ExecuteScalar<int>(sql);
        return count;
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
    
    public QuerySingleResult1 ExercisesQuerySingle()
    {
        string sql = "SELECT FirstName, LastName FROM Actors WHERE ActorId = 1";
        using var connection = new MySqlConnection(GetConnectionString());
        var actor = connection.QuerySingle<QuerySingleResult1>(sql);
        return actor;
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
    
    // QuerySingle<T> throws an exception if the query returns zero or more than one row.
    // Let write a query that return all movies with the language 'English'. (this is a query that returns multiple rows)
    // Select the Title, Language
    public class QuerySingleResult4
    {
        public string Title { get; set; } = null!;
        public string Language { get; set; } = null!;
    }
    public QuerySingleResult4 ExerciseQuerySingle4()
    {
        string sql = "SELECT Title, Language FROM Movies WHERE Language = 'English'";
        using var connection = new MySqlConnection(GetConnectionString());
        var result4 = connection.QuerySingle<QuerySingleResult4>(sql);
        return result4;
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
    
    
    
    // Write a SQL query to find the name and year of the movies.
    // Use the correct Dapper method and return a IEnumerable<dynamic> object.
    // Don't worry about the return type, we will fix that in the next exercise.
    public IEnumerable<dynamic> Exercise1()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.Query("SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10");
        return movies;
    }
    
    // It's not a good practice to use an IEnumerable<dynamic> as in the previous exercise. Let's fix that.
    // Write a SQL query to find the name and year of the movies. This time return a list of Movie objects.
    // Use the correct Dapper method and create a class named ResultExercise1_1 with the properties Title and Year.
    // Use to correct type for the properties.
    public class ResultExercise1BAndC
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
    }
    
    public IEnumerable<ResultExercise1BAndC> Exercise1B()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.Query<ResultExercise1BAndC>("SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10");
        return movies;
    }
    
    // In the previous exercise the return type IEnumerable<ResultExercise1BAndC> is not a good practice, because it's not type safe!
    // Change this to a List<ResultExercise1BAndC> instead. Use the same Dapper method as in the previous exercise.
    // The is a method To....() that can be used to convert an IEnumerable<T> to a List<T>.
    // 'Dot into' the IEnumerable<ResultExercise1BAndC> and press Ctrl + . to see the available methods.
    // If you look at the return types should be able to find the correct method.
    public List<ResultExercise1BAndC> Exercise1C()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.Query<ResultExercise1BAndC>("SELECT Title, Year FROM Movies ORDER BY Title LIMIT 10");
        return movies.ToList();
    }
    
    
    // Write a SQL query to find when the movie 'American Beauty' was released. Return the movie release year, as an object?
    // Use the correct Dapper method and return a object?. Don't worry about the return type, we will fix that in the next exercise.
    public object? Exercise2()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var year = connection.ExecuteScalar("SELECT Year FROM Movies WHERE Title = 'American Beauty'");
        
        return year;
    }
    
    // In the previous exercises the return type object?, is not a good practice, because it's not type safe!
    // Write a SQL query to find when the movie 'American Beauty' was released. Return the movie release year.
    // This time, use the correct return type. Don't cast the result to int, use the correct Dapper method instead.
    public int Exercise2_1()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var year = connection.ExecuteScalar<int>("SELECT Year FROM Movies WHERE Title = 'American Beauty'");
        
        return year;
    }
    
    // Write a SQL query to find all the years when a Japanese (Language) movie was released. Return movie release years.
    // Order the result by year.
    public List<int> Exercise3()
    {
        using var connection = new MySqlConnection("server=localhost;port=3306;database=Movies;user=root;password=Test@1234!");
        var years = connection.Query<int>("SELECT Year FROM Movies WHERE Language = 'Japanese' ORDER BY Year");
        return years.ToList();
    }
    //
    // //Which actors played in the movie with the highest average rating?
    // // Write a SQL query to find all the actors that played in the movie with the highest average rating.
    // public List<string> Exercise4()
    // {
    //     string sql = @"SELECT a.FirstName, a.LastName FROM Actors a JOIN Ac am ON a.ActorId = am.ActorsActorId JOIN Movies m ON am.MoviesMovieId = m.MovieId JOIN Ratings r ON m.MovieId = r.MovieId
    //                     WHERE m.MovieId = (SELECT m.MovieId FROM Movies m JOIN Ratings r ON m.MovieId = r.MovieId GROUP BY m.MovieId ORDER BY AVG(r.Stars) DESC LIMIT 1)";
    //     using var connection = new MySqlConnection(GetConnectionString());
    //     var actors = connection.Query<string>(sql);
    //     return actors.ToList();
    // }

    
    

    
    
    // Write a SQL query to find all the titles of the movies directed by 'Kevin Spacy'. Return movie titles.
    // Order the result alphabetically. It's always a good idea to return a List<T> instead of IEnumerable<T> when using Dapper.
    public List<string> Exercise4()
    {
        string sql = @"   SELECT Title 
                    FROM 
                        Movies m JOIN 
                            DirectorMovie dm ON m.MovieId = dm.MoviesMovieId JOIN    
                                Directors d ON dm.DirectorsDirectorId = d.DirectorId                        
                    WHERE d.FirstName = 'Kevin' AND d.LastName = 'Spacy' ORDER BY Title";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var titles = connection.Query<string>(sql).ToList();
        return titles;
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


    public List<Movie> GetMovieById(int movieId )
    {
        string sql = @"SELECT Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode
                        FROM Movies WHERE MovieId = @movieId";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.Query<Movie>(sql, new { movieId });
        
        
        return movies.ToList();
    }
    
    public class MovieAndDirector
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public string ReleaseCountryCode { get; set; } = null!;

        public List<Director> Directors { get; set; } = new ();
    }

    public class Director
    {
        public int DirectorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    public MovieAndDirector GetMovieWithDirectoryById(int movieId)
    {
        string sql = @"SELECT m.Title, m.Year, m.Duration, m.Language, m.ReleaseDate, m.ReleaseCountryCode, d.DirectorId, d.FirstName, d.LastName
                        FROM Movies m 
                            JOIN DirectorMovie dm ON m.MovieId = dm.MoviesMovieId 
                                JOIN Directors d ON dm.DirectorsDirectorId = d.DirectorId
                        WHERE m.MovieId = @movieId";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var movies = connection.Query<MovieAndDirector, Director, MovieAndDirector>(sql, (movie, director) =>
        {
            movie.Directors.Add(director);
            return movie;
        }, new { movieId });
        return movies.First();
    }

    public MovieAndDirector GetMovieWithDirectorByMultipleResultSet(int movieId)
    {
        string sql = @"SELECT m.Title, m.Year, m.Duration, m.Language, m.ReleaseDate, m.ReleaseCountryCode
                        FROM Movies m 
                        WHERE m.MovieId = @movieId;
                        
                        SELECT d.DirectorId, d.FirstName, d.LastName
                        FROM DirectorMovie dm 
                            JOIN Directors d ON dm.DirectorsDirectorId = d.DirectorId
                        WHERE dm.MoviesMovieId = @movieId";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var gridReader = connection.QueryMultiple(sql, new { movieId });
        var movie = gridReader.Read<MovieAndDirector>().Single();
        var directors = gridReader.Read<Director>().ToList();
        movie.Directors.AddRange(directors);
        return movie;
    }
}
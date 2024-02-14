# Exercises1.cs Summary


The [`Exercises1.cs`](exercises1.cs) file is a C# test suite that uses Dapper, a micro ORM for .NET, to interact with a MySQL database. Here's a brief summary of the methods used:


- `Exercise0()`: Tests the database connection by executing a simple SQL query.
- `ExerciseScalar1()`: Executes a SQL query to find the number of movies in the database.
- `ExerciseScalar2()`: Executes a SQL query to count the number of female actors in the database.
- `ExerciseScalar3()`: Executes a SQL query to check if there are any reviewers with an empty name or no name.
- `ExerciseScalar4()`: Executes a SQL query to find the number of movies that have a rating higher than 8.5.
- `ExerciseScalar5()`: Executes a SQL query to find the actor full name who performs in the most movies.
- `ExerciseScalar6()`: Executes a SQL query to find the movie with the highest average rating.
- `ExercisesQuerySingle()`: Executes a SQL query to find the first and last name of the actor with ActorId = 101.
- `ExercisesQuerySingle2()`: Executes a SQL query to find the first and last name of the director and the year and duration from the movie with the name 'American Beauty'.
- `ExerciseQuerySingle3()`: Executes a SQL query that returns Title, ReleaseDate for a movie that doesn't exist.
- `ExerciseQuerySingleOrDefault1()`: Executes a SQL query for a movie that doesn't exist and returns Title, Language.
- `ExerciseQueryFirst()`: Executes a SQL query that returns all movies with the language 'English'.
- `ExerciseQueryDynamic()`: Executes a SQL query to find the name and year of the movies and returns a `IEnumerable<dynamic>` object.
- `ExerciseQuery()`: Executes a SQL query to find the name and year of the movies and returns a list of `ResultExerciseQuery` objects.
- `ExerciseQuery2()`: Executes a SQL query to find all the titles of the movies directed by 'Kevin Spacey'.
- `GetMovieById()`: Executes a SQL query to get a movie by movieId and returns a `Movie` object.
- `GetMovieById2()`: Executes a SQL query to get a movie by movieId and returns a nullable `Movie` object.

The methods are tested using the [FluentAssertions](https://fluentassertions.com/) library. The context of the exercises is a movie database, with operations such as counting the number of movies, finding the number of female actors, checking if there are reviewers with an empty name, finding the actor who performs in the most movies, and finding the movie with the highest average rating.

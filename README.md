# Summary of Exercises in `Exercises1.cs`

The `Exercises1.cs` file contains a series of exercises designed to practice using Dapper, a micro ORM (Object-Relational Mapping) library for .NET. The exercises are focused on connecting to a MySQL database, executing various SQL queries, and handling the results using Dapper's methods.

## Exercises

1. **Exercise0:** Tests the ability to connect to a MySQL database using a connection string. Returns a boolean indicating the success of the connection.

2. **ExerciseScalar1:** Counts the number of movies in the database. Returns the count as an integer.

3. **ExerciseScalar2:** Counts the number of female actors in the database. Returns the count as an integer.

4. **ExerciseScalar3:** Checks if there are any reviewers with an empty name or no name (null). Returns a boolean indicating the result.

5. **ExerciseScalar4:** Finds the number of movies that have a rating higher (stars) than 8.5. Returns the count as an integer.

6. **ExerciseScalar5:** Finds the actor full name (FirstName , ' ' , LastName) who performs in the most movies. Returns the full name as a string.

7. **ExerciseScalar6:** Finds which movie (return title) has the highest average rating. Returns the movie title as a string.

8. **ExercisesQuerySingle:** Finds the first and last name of the actor with ActorId = 101. Returns the names as a string.

9. **ExercisesQuerySingle2:** Finds the first and last name of the director and the year and duration from the movie with the name 'American Beauty'. Returns the details as a string.

10. **ExerciseQuerySingle3:** Tries to find a movie that doesn't exist. Returns null if the movie doesn't exist.

11. **ExerciseQuerySingleOrDefault1:** Tries to find a movie that doesn't exist and returns null if the movie doesn't exist.

12. **ExerciseQueryFirst:** Returns the first movie with the language 'English'. Returns the movie details as a string.

13. **ExerciseQueryDynamic:** Returns the name and year of the movies. Returns the details as a dynamic object.

14. **ExerciseQuery:** Returns the name and year of the movies as a list of `ResultExerciseQuery` objects.

15. **ExerciseQuery2:** Finds all the titles of the movies directed by 'Kevin Spacey'. Returns the movie titles as a list of strings.

16. **GetMovieById:** Gets a movie by movieId, returns a `Movie` Object that contains the Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode.

17. **GetMovieById2:** Similar to `GetMovieById`, but returns a nullable `Movie` object. If the movie doesn't exist, it returns null.

Each exercise is accompanied by a unit test to verify the correctness of the implemented method. The exercises cover a range of Dapper's capabilities, from executing simple scalar queries to handling complex queries that return multiple rows and columns.
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

13. **ExerciseQueryDynamic:** Returns the title and year of the first 10 movies ordered by title as a list of dynamic objects.

14. **ExerciseQuery:** Returns the title and year of the first 10 movies ordered by title as a list of `ResultExerciseQuery` objects.

15. **ExerciseQuery2:** Finds all the titles of the movies directed by 'Kevin Spacey'. Returns the movie titles as a list of strings.

16. **GetMovieById:** Gets a movie by movieId, returns a `Movie` Object that contains the Title, Year, Duration, Language, ReleaseDate, ReleaseCountryCode.

17. **GetMovieById2:** Similar to `GetMovieById`, but returns a nullable `Movie` object. If the movie doesn't exist, it returns null.

Each exercise is accompanied by a unit test to verify the correctness of the implemented method. The exercises cover a range of Dapper's capabilities, from executing simple scalar queries to handling complex queries that return multiple rows and columns.

# Summary of Exercises in `Exercises2.cs`

The `Exercises2.cs` file contains a series of exercises designed to practice using Dapper, a micro ORM (Object-Relational Mapping) library for .NET. The exercises are focused on connecting to a MySQL database, executing various SQL queries, and handling the results using Dapper's methods.

## Exercises

1. **GetCustomerByEmail:** Retrieves a list of customers by their email. The SQL return method used here is `Query<CustomerForSqlInjection>`, which executes a query and maps the result to a list of `CustomerForSqlInjection` objects.

2. **GetCustomerByEmail2:** Similar to `GetCustomerByEmail`, but with SQL injection prevention. The SQL return method used here is `Query<CustomerForSqlInjection>`, which executes a query and maps the result to a list of `CustomerForSqlInjection` objects.

3. **ViewExercises1:** Retrieves a list of rental views. The SQL return method used here is `Query<RentalView>`, which executes a query and maps the result to a list of `RentalView` objects.

4. **ViewExercises2:** Retrieves a list of top rental customers. The SQL return method used here is `Query<TopRentalCustomers>`, which executes a query and maps the result to a list of `TopRentalCustomers` objects.

5. **ViewExercises3:** Retrieves a list of rentals by country. The SQL return method used here is `Query<RentalByCountry>`, which executes a query and maps the result to a list of `RentalByCountry` objects.

6. **ViewExercises4:** Similar to `ViewExercises3`, but with a different implementation. The SQL return method used here is `Query<RentalByCountry>`, which executes a query and maps the result to a list of `RentalByCountry` objects.

7. **ExerciseParameter1:** Retrieves a list of customer searches based on the city. The SQL return method used here is `Query<CustomerSearch>`, which executes a query and maps the result to a list of `CustomerSearch` objects.

8. **ExerciseParameter2:** Similar to `ExerciseParameter1`, but with an optional city parameter. The SQL return method used here is `Query<CustomerSearch>`, which executes a query and maps the result to a list of `CustomerSearch` objects.

9. **ExerciseParameter3:** Retrieves a list of customer searches based on the country and city. The SQL return method used here is `Query<CustomerSearch>`, which executes a query and maps the result to a list of `CustomerSearch` objects.

10. **ExerciseParameter4:** Retrieves a list of customer searches based on a `CustomerSearchParameters` object. The SQL return method used here is `Query<CustomerSearch>`, which executes a query and maps the result to a list of `CustomerSearch` objects.

11. **InsertCustomerCopy:** Inserts a copy of a customer into the database. The SQL return method used here is `Execute`, which executes a command that does not return rows.

12. **InsertCityExercises:** Inserts a city into the database. The SQL return method used here is `Execute`, which executes a command that does not return rows.

Each exercise is accompanied by a unit test to verify the correctness of the implemented method. The exercises cover a range of Dapper's capabilities, from executing simple queries to handling complex queries that return multiple rows and columns.


# Summary of Exercises in `Exercises3.cs`

The `Exercises3.cs` file contains a series of exercises designed to practice using Dapper, a micro ORM (Object-Relational Mapping) library for .NET. The exercises are focused on connecting to a MySQL database, executing various SQL queries, and handling the results using Dapper's methods.

## Exercises

1. **ExerciseOneToOne:** This exercise is about creating a query that returns all payments with the customer information. The SQL return method used here would be `Query<Payment, Customer, Payment>`, which executes a query and maps the result to a list of `Payment` objects, each containing a `Customer` object.

2. **ExerciseOneToOneTwoJoins:** Similar to `ExerciseOneToOne`, but now with an additional join to the address table. The SQL return method used here would be `Query<Payment, Customer, Address, Payment>`, which executes a query and maps the result to a list of `Payment` objects, each containing a `Customer` object, which in turn contains an `Address` object.

3. **ExerciseOneToManyWithDictionaryMethod:** This exercise is about creating a query that returns all stores with their customers. The SQL return method used here would be `Query<Store, Customer, Store>`, which executes a query and maps the result to a list of `Store` objects, each containing a list of `Customer` objects.

4. **ExerciseOneToManyWithJsonMethod:** Similar to `ExerciseOneToManyWithDictionaryMethod`, but now using the JSON method. The SQL return method used here would be `Query<Store, Customer, Store>`, which executes a query and maps the result to a list of `Store` objects, each containing a list of `Customer` objects.

5. **GetFilmsIncludeActorsDictionaryMethod:** This exercise is about creating a query that returns a list of films with their actors. The SQL return method used here would be `Query<Film, Actor, Film>`, which executes a query and maps the result to a list of `Film` objects, each containing a list of `Actor` objects.

6. **GetFilmsIncludeActorsJsonMethod:** Similar to `GetFilmsIncludeActorsDictionaryMethod`, but now using the JSON method. The SQL return method used here would be `Query<Film, Actor, Film>`, which executes a query and maps the result to a list of `Film` objects, each containing a list of `Actor` objects.

Each exercise is accompanied by a unit test to verify the correctness of the implemented method. The exercises cover a range of Dapper's capabilities, from executing simple queries to handling complex queries that return multiple rows and columns.

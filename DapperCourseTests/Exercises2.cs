using Dapper;
using FluentAssertions;
using MySqlConnector;

namespace DapperCourseTests;

public class Exercises2
{
    // Database installation
    // For the exercises below, use the sakila database, this database can be downloaded from:
    // https://dev.mysql.com/doc/index-other.html
    // unzip the sakila-db.zip file and import the sakila-schema.sql and sakila-data.sql files into your mysql database
    // https://dev.mysql.com/doc/sakila/en/sakila-installation.html
    // take a look a the database schema, you can use a tool like MySQL Workbench for this (or Rider)
    // it's a movie rental database, it contains tables like:
    // actor, address, category, city, country, customer, film, film_actor, film_category, inventory, language, payment, rental, staff, store
    // the structure (database diagram) can be found here:
    // https://dev.mysql.com/doc/sakila/en/sakila-structure.html (the direction of the arrows is incorrect!)
    private static string GetConnectionString()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    
    [Test]
    public void TestDatabaseIsCreatedAndFilled()
    {
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        string sql = """SELECT COUNT(*) FROM nicer_but_slower_film_list""";
        int count = connection.ExecuteScalar<int>(sql);
        count.Should().Be(1000);
    }
    
    // NEVER EVER DO THIS AGAIN, THIS IS AN SQL INJECTION EXAMPLE!
    // Write a query that returns one customer by its email.
    // Use string interpolation ($" ... WHERE email = '{email}'") or concatenation to create the query string
    // and pass it to the Query method.
    // Use the Customer class below to map the result
    // Use the Query() method to execute the query, (it was better to use QuerySingle() method,
    // but this is harder to get all the data with SQL injection)
    // NEVER EVER DO THIS AGAIN, THIS IS AN SQL INJECTION EXAMPLE!
    public class CustomerForSqlInjection
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
    
    public List<CustomerForSqlInjection> GetCustomerByEmail(string email)
    {
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        string sql = $"""
                       SELECT customer_id as CustomerId, first_name as FirstName, last_name as LastName, email as Email
                       FROM customer WHERE email = '{email}'
                      """;
        IEnumerable<CustomerForSqlInjection> customer = connection.Query<CustomerForSqlInjection>(sql);
        return customer.ToList();
    }
    
    [Test]
    public void SqlInjectionTest()
    {
        string email = "PATRICIA.JOHNSON@sakilacustomer.org";
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        
        GetCustomerByEmail(email).Should().HaveCount(1);

        List<CustomerForSqlInjection> allCustomers = GetCustomerByEmail("PATRICIA.JOHNSON@sakilacustomer.org' OR 1 = 1 -- ");
        allCustomers.Should().HaveCount(599); //this demonstrates SQL injection, we get all the customers instead of the one we want
    }
    
    //Fix the previous exercise, use SQL Parameter Placeholders (WHERE email = @Email).
    //Always use SQL Parameter Placeholders!!!!!
    public List<CustomerForSqlInjection> GetCustomerByEmail2(string email)
    {
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        string sql = $"""
                       SELECT customer_id as CustomerId, first_name as FirstName, last_name as LastName, email as Email
                       FROM customer WHERE email = @email
                      """;
        IEnumerable<CustomerForSqlInjection> customer = connection.Query<CustomerForSqlInjection>(sql, new {email});
        return customer.ToList();
    }
    
    [Test]
    public void SqlInjectionPreventionTest()
    {
        string email = "PATRICIA.JOHNSON@sakilacustomer.org";
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        
        GetCustomerByEmail(email).Should().HaveCount(1);

        List<CustomerForSqlInjection> allCustomers = GetCustomerByEmail2("PATRICIA.JOHNSON@sakilacustomer.org' OR 1 = 1 -- ");
        allCustomers.Should().HaveCount(0); //this demonstrates SQL injection, we get all the customers instead of the one we want
    }

    
    //Create a view that retrieves all the columns from the tables: rental, customer, inventory, film
    //In other words join the tables rental, customer, inventory, film
    //Create a view called rental_view

    public class RentalView
    {
        public int RentalId { get; set; }
        public DateTime RentalDate { get; set; }
        public int InventoryId { get; set; }
        public int CustomerId { get; set; }
        public DateTime ReturnDate { get; set; }
        public int StaffId { get; set; }
        public DateTime RentalLastUpdate { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int AddressId { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public int FilmId { get; set; }
        public int StoreId { get; set; }
        public DateTime InventoryLastUpdate { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ReleaseYear { get; set; }
        public int LanguageId { get; set; }
        public int OriginalLanguageId { get; set; }
        public int RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
        public int Length { get; set; }
        public decimal ReplacementCost { get; set; }
        public string Rating { get; set; } = null!;
        public string SpecialFeatures { get; set; } = null!;
        public DateTime FilmLastUpdate { get; set; }
    }
    
    public List<RentalView> ViewExercises1()
    {
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        string sql = """
                      DROP VIEW IF EXISTS rental_view;
                      CREATE VIEW rental_view AS
                      SELECT r.rental_id as RentalId, r.rental_date as RentalDate, r.inventory_id as InventoryId,
                      r.customer_id as CustomerId, r.return_date as ReturnDate,
                          r.staff_id as StaffId, r.last_update as RentalLastUpdate,
                             c.first_name as FirstName, c.last_name as LastName, c.email as Email, c.address_id as AddressId,
                             c.active as Active, c.create_date as CreateDate,
                             i.film_id as FilmId, i.store_id as StoreId, i.last_update as InventoryLastUpdate,
                             f.title as Title, f.description as Description, f.release_year as ReleaseYear, f.language_id as LanguageId,
                             f.original_language_id as OriginalLanguageId, f.rental_duration as RentalDuration,
                             f.rental_rate as RentalRate,
                             f.length as Length , f.replacement_cost as ReplacementCost,
                             f.rating as Rating, f.special_features as SpecialFeatures,
                             f.last_update as FilmLastUpdate
                      FROM rental r
                          JOIN customer c ON r.customer_id = c.customer_id
                              JOIN inventory i ON r.inventory_id = i.inventory_id
                                  JOIN film f ON i.film_id = f.film_id
                     """;
        connection.Execute(sql);


        return connection.Query<RentalView>("SELECT * FROM rental_view").ToList();
    }
    
    [Test]
    public void ViewExercises1Test()
    {
        // Arrange
        Exercises2 sut = new Exercises2();
        
        // Act
        List<RentalView> result = sut.ViewExercises1();
        
        // Assert
        result.Should().HaveCount(16044);
    }

    //In the previous exercise we created a view, this one has a lot (maybe too much) columns.
    //We can query the view with only the columns that we need and we can rename the columns with the AS keyword (or space).
    //Create a query based on the view from the previous exercise, but only select the following columns:
    //CustomerId, Lastname, Firstname, AmountOfRentals
    //Order Descending (DESC) by AmountOfRentals
    //Select only the first 10 rows
    //This query select the top 10 customers with the most rentals

    public class TopRentalCustomers
    {
        public int CustomerId { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public int AmountOfRentals { get; set; }
    }
    
    public List<TopRentalCustomers> ViewExercises2()
    {
        string sql = """
                     SELECT CustomerId, LastName, FirstName, COUNT(*) as AmountOfRentals
                     FROM rental_view GROUP BY CustomerId ORDER BY AmountOfRentals DESC LIMIT 10
                     """;
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        IEnumerable<TopRentalCustomers> result = connection.Query<TopRentalCustomers>(sql);
        return result.ToList();
    }
    
    [Test]
    public void ViewExercises2Test()
    {
        // Arrange
        Exercises2 sut = new Exercises2();
        
        // Act
        List<TopRentalCustomers> result = sut.ViewExercises2();
        
        // Assert
        result.Should().HaveCount(10);
        
        result.First().CustomerId.Should().Be(148);
        result.First().LastName.Should().Be("HUNT");
        result.First().FirstName.Should().Be("ELEANOR");
        result.First().AmountOfRentals.Should().Be(46);
    }
    
    //It's also possible to combine views with other tables (or views). 
    //From a query's perspective a view is just a table.
    //Create a query that returns the following columns:
    //CountryName, AmountOfRentalsByCountry
    //in other words, you should join the rental_view with the country table, this could not be done directly,
    //so multiple joins are needed.
    //Order descending by AmountOfRentalsByCountry
    //Select only the first 10 rows
    
    public class RentalByCountry
    {
        public string CountryName { get; set; } = null!;
        public int AmountOfRentalsByCountry { get; set; }
    }

    public List<RentalByCountry> ViewExercises3()
    {
        string sql = """
                        SELECT c.country as CountryName, COUNT(*) as AmountOfRentalsByCountry  
                        FROM rental_view
                            JOIN address a ON rental_view.AddressId = a.address_id
                                JOIN city ci ON a.city_id = ci.city_id
                                    JOIN country c ON ci.country_id = c.country_id
                            GROUP BY c.country
                            ORDER BY COUNT(*) DESC
                            LIMIT 10
                     """;
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        List<RentalByCountry> result = connection.Query<RentalByCountry>(sql).ToList();
        return result;
    }
    
    [Test]
    public void ViewExercises3Test()
    {
        // Arrange
        Exercises2 sut = new Exercises2();
        
        // Act
        List<RentalByCountry> result = sut.ViewExercises3();
        
        // Assert
        result.Should().HaveCount(10);
        
        result.First().CountryName.Should().Be("India");
        result.First().AmountOfRentalsByCountry.Should().Be(1572);
        
        result.Last().CountryName.Should().Be("Indonesia");
        result.Last().AmountOfRentalsByCountry.Should().Be(367);
    }
    
    //In the previous exercise we joined the rental_view with many other tables, this is not very efficient.
    //We can create a view that already contains the joined tables.
    //So let's create a view that joins all the customer related tables (customer, address, city, country)
    //call this view customer_view
    //Use this view to create the same query as in the previous exercise, but now use the customer_view instead and the rental_view
    //instead of joining all the tables again.
    //The idea with views is that we can abstract the complexity of the database schema (structure) and complex queries and create views that are easier to query.
    //Actually views have more advantages, but this is out of scope for this course.
    //Abstraction is a very important concept in software development, it's a way to hide complexity.
    //In this case we hide the complexity of the database schema.
    //In my opinion, views are not used enough, they are a very powerful tool to abstract complexity!
    //Also I find abstraction a very important concept in software development, it's a way to hide complexity.
    //Abstraction are everywhere in software development, OO programming is a way to abstract complexity (methods, classes, interfaces, ...),
    //Enough about abstraction, let's create the customer_view and use it to reproduce the query from the previous exercise.
    public List<RentalByCountry> ViewExercises4()
    {
        string createCustomerView =
            """
                DROP VIEW IF EXISTS customer_view;

                CREATE VIEW customer_view AS
                SELECT c.customer_id, c.first_name, c.last_name, c.email, a.address, a.address2, 
                        c.address_id,
                        a.district, a.postal_code, a.phone, ci.city, co.country
                FROM customer c
                     JOIN address a ON c.address_id = a.address_id
                        JOIN city ci ON a.city_id = ci.city_id
                            JOIN country co ON ci.country_id = co.country_id;
            """;
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        connection.Execute(createCustomerView);
        
        string sql = """
                        SELECT c.country as CountryName, COUNT(*) as AmountOfRentalsByCountry
                        FROM rental_view
                              JOIN customer_view c ON rental_view.CustomerId = c.customer_id
                        GROUP BY c.country
                        ORDER BY AmountOfRentalsByCountry DESC
                        LIMIT 10
                     """;
        List<RentalByCountry> result = connection.Query<RentalByCountry>(sql).ToList();
        return result;
    }
    
    [Test]
    public void ViewExercises4Test()
    {
        // Arrange
        Exercises2 sut = new Exercises2();
        
        // Act
        List<RentalByCountry> result = sut.ViewExercises4();
        
        // Assert
        result.Should().HaveCount(10);
        
        result.First().CountryName.Should().Be("India");
        result.First().AmountOfRentalsByCountry.Should().Be(1572);
        
        result.Last().CountryName.Should().Be("Indonesia");
        result.Last().AmountOfRentalsByCountry.Should().Be(367);
    }    
    
    //Let's shift our attention to parameters.
    //For our rental business we want to create a queries that return information about customers.
    //In other words, we want to create a query that returns a list of customers based on different criteria.
    //Create a query that returns all the customers that live in a certain city (city is a parameter).
    //Use parameter placeholders (@city) and pass the parameter to the Query method.
    //You could use a view (maybe you already created one) or join the tables directly, it's up to you.
    //Use the Customer class (CustomerSearch) below to map the result, it has the following properties:
    //CustomerId, FirstName, LastName, Email
    //Order by LastName, FirstName
    //Limit the result to 10 rows

    public class CustomerSearch
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
    
    public List<CustomerSearch> ExerciseParameter1(string city)
    {
        string sql =
            """
                SELECT c.customer_id AS CustomerId, c.last_name AS LastName, c.first_name AS FirstName, c.email AS Email
                FROM customer c
                    JOIN address a on c.address_id = a.address_id
                        JOIN city ci on a.city_id = ci.city_id WHERE ci.city = @city
                ORDER BY c.last_name, c.first_name
                LIMIT 10
            """;
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        List<CustomerSearch> result = connection.Query<CustomerSearch>(sql, 
            new {city}).ToList();

        return result;
    }

    [Test]
    public void ExerciseParameter1Test()
    {
        // Arrange
        Exercises2 sut = new Exercises2();

        // Act
        List<CustomerSearch> result = sut.ExerciseParameter1("London");

        // Assert
        result.Should().HaveCount(2);

        result.First().CustomerId.Should().Be(252);
        result.First().LastName.Should().Be("HOFFMAN");

        result.Last().CustomerId.Should().Be(512);
        result.Last().Email.Should().Be("CECIL.VINES@sakilacustomer.org");
    }
    
    //Let's create a query that returns all the customers that live in a certain city (city is a parameter).
    //The difference with the previous exercise is that we want the parameter to be optional.
    //So don't forget to LIMIT the result to 10 rows, and order by LastName, FirstName
    //If the parameter is not passed, we want to return all the customers.
    //Use parameter placeholders (@city) and pass the parameter to the Query method.
    //I always use the NULL trick to make a parameter optional, see the examples
    //So the query is the same as the previous exercise, but the parameter is optional
    public List<CustomerSearch> ExerciseParameter2(string? city = null)
    {
        string sql =
            """
                SELECT c.customer_id as CustomerId, c.last_name as LastName, c.first_name as FirstName, c.email as Email
                FROM customer c
                    JOIN address a on c.address_id = a.address_id
                        JOIN city ci on a.city_id = ci.city_id 
                WHERE (@City IS NULL OR ci.city = @City)
                ORDER BY c.last_name, c.first_name
                LIMIT 10
            """;
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        List<CustomerSearch> result = connection.Query<CustomerSearch>(sql, 
            new {City = city}).ToList();

        return result;
    }
    
    [Test]
    public void ExerciseParameter2Test()
    {
        // Arrange
        Exercises2 sut = new Exercises2();

        // Act -- optional parameter
        List<CustomerSearch> result = sut.ExerciseParameter2("London");

        // Assert
        result.Should().HaveCount(2);

        result.First().CustomerId.Should().Be(252);
        result.First().LastName.Should().Be("HOFFMAN");

        result.Last().CustomerId.Should().Be(512);
        result.Last().Email.Should().Be("CECIL.VINES@sakilacustomer.org");
        
        // Act -- no optional parameter
        List<CustomerSearch> result2 = sut.ExerciseParameter2();

        result2.Should().HaveCount(10);
    }

    //Let's create a query that returns all the customers that live in a certain city (city is a parameter).
    //Or all the customers that live in a certain country (country is a parameter).
    //Both parameters are optional.
    //If both parameters are empty (null), we want to return all the customers.
    //Use parameter placeholders (@city, @country) and pass the parameters to the Query method.
    //LIMIT the result to 10 rows, and order by LastName, FirstName
    
    public List<CustomerSearch> ExerciseParameter3(string? country = null, string? city = null)
    {
        string sql =
            """
                SELECT c.customer_id as CustomerId, c.last_name as LastName, c.first_name as FirstName, c.email as Email
                FROM customer c
                    JOIN sakila.address a on c.address_id = a.address_id
                        JOIN sakila.city ci on a.city_id = ci.city_id
                            JOIN sakila.country co on ci.country_id = co.country_id
                WHERE (@City IS NULL OR ci.city = @City) 
                  AND (@Country IS NULL OR co.country = @Country)
                ORDER BY c.last_name, c.first_name
                LIMIT 10
            """;
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        List<CustomerSearch> result = connection.Query<CustomerSearch>(sql, 
            new {City = city, Country = country}).ToList();

        return result;
    }

    [Test]
    public void ExerciseParameter3()
    {
        // Arrange
        Exercises2 sut = new Exercises2();

        // Act one parameter (country)
        List<CustomerSearch> result = sut.ExerciseParameter3(city: "London");

        // Assert
        result.Should().HaveCount(2);

        result.First().CustomerId.Should().Be(252);
        result.First().LastName.Should().Be("HOFFMAN");

        result.Last().CustomerId.Should().Be(512);
        result.Last().Email.Should().Be("CECIL.VINES@sakilacustomer.org");
        
        
        // Act one parameter (country)
        result = sut.ExerciseParameter3(country: "Netherlands");

        // Assert
        result.Should().HaveCount(5);

        result.First().CustomerId.Should().Be(474);
        result.First().LastName.Should().Be("GILLETTE");

        result.Last().CustomerId.Should().Be(184);
        result.Last().Email.Should().Be("VIVIAN.RUIZ@sakilacustomer.org");
        
        // Act -- optional parameter
        List<CustomerSearch> result2 = sut.ExerciseParameter2();

        result2.Should().HaveCount(10);
        result2.First().Email.Should().Be("RAFAEL.ABNEY@sakilacustomer.org");
        result2.Last().Email.Should().Be("IDA.ANDREWS@sakilacustomer.org");
    }
    
    //Use an object for parameters
    //In the previous exercises we used anonymous objects to pass parameters to the Query method.
    //This is not very convenient, because we have to remember the names of the properties.
    //It's better to use an object for the parameters. Also we can use the same object for multiple queries.
    //Create a class called CustomerSearchParameters with the following properties:
    //Country, City, PageNumber and PageSize
    //The default value for PageNumber is 1 and for PageSize is 10
    //The default value for Country and City is null
    //Use the same query as in the previous exercise, but now use the CustomerSearchParameters object
    //to pass the parameters to the Query method.
    //In the query use the Offset and PageSize properties to limit the result to the correct page.
    public class CustomerSearchParameters
    {
        public string? Country { get; set; } = null;
        public string? City { get; set; } = null;
        public int PageNumber { get; set; } = 1;

        public int Offset => (PageNumber - 1) * PageSize;
        public int PageSize { get; set; } = 10;
    }

    
    public List<CustomerSearch> ExerciseParameter4(CustomerSearchParameters customerSearchParameters)
    {
        string sql =
            """
                SELECT c.customer_id as CustomerId, c.last_name as LastName, c.first_name as FirstName, c.email as Email
                FROM customer c
                    JOIN address a on c.address_id = a.address_id
                        JOIN city ci on a.city_id = ci.city_id
                            JOIN country co on ci.country_id = co.country_id
                WHERE (@City IS NULL OR ci.city = @City) AND (@Country IS NULL OR co.country = @Country)
                ORDER BY c.last_name, c.first_name
                LIMIT @PageSize OFFSET @Offset
            """;
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        List<CustomerSearch> result = connection.Query<CustomerSearch>(sql, 
                customerSearchParameters
            ).ToList();

        return result;
    }

    [Test]
    public void ExerciseParameter4Test()
    {
        // Arrange
        Exercises2 sut = new Exercises2();
        CustomerSearchParameters customerSearchParameters = new CustomerSearchParameters()
        {
            Country = "India",
            
        };

        // Act
        List<CustomerSearch> result = sut.ExerciseParameter4(customerSearchParameters);

        // Assert
        result.Should().HaveCount(10);

        result.First().CustomerId.Should().Be(252);
        result.First().LastName.Should().Be("HOFFMAN");

        result.Last().CustomerId.Should().Be(512);

        CustomerSearchParameters customerSearchParameters2 = new CustomerSearchParameters()
        {
            PageNumber = 3, PageSize = 10
        };
        
        // Act
        List<CustomerSearch> result2 = sut.ExerciseParameter4(customerSearchParameters2);
        
        //TODO: fix test
    }
    
    
    
    //Let's practice with parameters a little bit more. This time in the context of inserts and updates.
    //First we need to create a new table called customer_copy.
    //This table we are going to use to store the old data (we pretend).
    //A trick to create a table from an existing table is to use the CREATE TABLE ... SELECT statement.
    //DROP TABLE IF EXISTS customer_copy;
    //CREATE TABLE customer_copy as SELECT * FROM customer LIMIT 0; 
    //If we remove the LIMIT 0, we copy all the data from the customer table to the customer_copy table.
    //This can be useful if we want to create a copy of a table. But not for this exercise.
    //The problem is that the customer_copy table has no constraints such as primary keys, foreign keys, ...
    //In order to insert we need ta add at least a primary key. Make sure also add the auto increment property
    //to the primary key.
    //To do this
    //alter table customer_copy
    // add constraint customer_copy_pk
    //     primary key (customer_id);
    //alter table customer_copy MODIFY COLUMN customer_id INT AUTO_INCREMENT
    //
    //Also create a table called address_copy with the same trick, make sure it has a primary key.
    //
    //For inserts we can use the Execute() method, this method returns the number of rows affected.
    //Also use an object for the parameters.
    //Actually this is not what we want, we want to return the last inserted id.
    //We would like to return the inserted customerId (generated by the primary key in the database),
    //we can do this with the LAST_INSERT_ID() function.
    //This function returns the last inserted id.
    // INSERT INTO customer_copy (store_id, first_name, last_name, email, address_id, active, create_date, last_update)
    // VALUES (@StoreId, @FirstName, @LastName, @Email, @AddressId, @Active, @CreateDate, @LastUpdate);
    // SELECT LAST_INSERT_ID();
    
    //So we can use the ExecuteScalar<int>() method to return the last inserted id.
    public class InsertCustomerParameters
    {
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int AddressId { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
    
    public int InsertCustomerCopy(InsertCustomerParameters insertCustomerParameters)
    {
        string sql =
            """
                DROP TABLE IF EXISTS customer_copy;
                CREATE TABLE customer_copy as SELECT * FROM customer LIMIT 0;
                alter table customer_copy
                    add constraint customer_copy_pk
                        primary key (customer_id);
                alter table customer_copy MODIFY COLUMN customer_id INT AUTO_INCREMENT; 
            """;
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        connection.Execute(sql);

        string insertSql =
            """
                INSERT INTO customer_copy (store_id, first_name, last_name, email, address_id, active, create_date, last_update)
                VALUES (@StoreId, @FirstName, @LastName, @Email, @AddressId, @Active, @CreateDate, @LastUpdate);
                SELECT LAST_INSERT_ID();
            """;
        int lastInsertedId = connection.ExecuteScalar<int>(insertSql, insertCustomerParameters);
        return lastInsertedId;
    }

    [Test]
    public void InsertCustomerCopyTest()
    {
        // Arrange
        Exercises2 sut = new Exercises2();
        InsertCustomerParameters insertCustomerParameters = new InsertCustomerParameters()
        {
            StoreId = 1,
            FirstName = "Test123",
            LastName = "123Test",
            Email = "test@test.com",
            AddressId = 1,
            CustomerId = 2,
            Active = true,
            CreateDate = DateTime.Parse("24-02-2006 22:04:36"),
            LastUpdate = DateTime.Parse("25-02-2006 22:04:36"),
        };

        // Act
        int result = sut.InsertCustomerCopy(insertCustomerParameters);

        // Assert
        result.Should().Be(1);
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        string sql = """
                     SELECT customer_id as CustomerId, store_id as StoreId, first_name as FirstName, last_name as LastName,
                       email as Email, address_id as AddressId, active as Active, create_date as CreateDate, last_update as LastUpdate
                         FROM customer_copy
                     """;
        InsertCustomerParameters customerCopy = connection.Query<InsertCustomerParameters>(sql).First();
        customerCopy.Should().BeEquivalentTo(insertCustomerParameters, options => options.Excluding(x => x.CustomerId));
    }
    
    //Optional Question, it's a lot of work.
    //
    //Let's create a query that inserts a new city in the city table.
    //A city belongs to a country, so we need to pass the country_id as a parameter.
    //The first step is to make a copy with the same trick as in the previous exercise from the city table and country table.
    //Make sure that the tables have a primary key.
    //Optionally you can also add the foreign key constraint (not necessary for this exercise).
    //Create a method that inserts a new city in the city_copy table.
    //but before you can insert the city, you need to check if the country name exists in the country_copy table.
    //If the country_id does not exist, you need to insert the country first. Otherwise you can use the country_id to insert the country_id into the city_copy table.
    //Use the ExecuteScalar<int>() method to return the last inserted id.
    //
    //It's hard to test this method if the country_id is reused in the city table.
    //Because we need to create a copy of the city and country table in the method InsertCityExercises.
    public class InsertCity
    {
        public string City { get; set; } = null!; 
        public DateTime LastUpdate { get; set; }
    }

    public class InsertCountry
    {
        public string Country { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }

    public int InsertCityExercises(InsertCountry country, InsertCity insertCity)
    {
        string sqlCreateCopyTables = """
                                     DROP TABLE IF EXISTS city_copy;
                                        DROP TABLE IF EXISTS country_copy;
                                        CREATE TABLE country_copy as SELECT * FROM country LIMIT 0;
                                        alter table country_copy
                                            add constraint country_copy_pk
                                                primary key (country_id);
                                        alter table country_copy MODIFY COLUMN country_id INT AUTO_INCREMENT;
                                        CREATE TABLE city_copy as SELECT * FROM city LIMIT 0;
                                        alter table city_copy
                                            add constraint city_copy_pk
                                                primary key (city_id);
                                        alter table city_copy MODIFY COLUMN city_id INT AUTO_INCREMENT;
                                     """;
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        connection.Execute(sqlCreateCopyTables);
        
        
        string countryExistsSql = "SELECT country_id FROM country_copy WHERE country = @Country";
        
        int? countryId = connection.QuerySingleOrDefault<int?>(countryExistsSql, country);
        if (!countryId.HasValue)
        {
            string sqlInsertCountry = """
                                      INSERT INTO country_copy (country, last_update) VALUES (@Country, @LastUpdate);
                                        SELECT LAST_INSERT_ID();
                                      """;
            
            countryId = connection.ExecuteScalar<int>(sqlInsertCountry, country);
        }
        
        string sqlInsertCity = """
                                INSERT INTO city_copy (city, country_id, last_update) VALUES (@City, @CountryId, @LastUpdate);
                                SELECT LAST_INSERT_ID();
                               """;
        int cityId = connection.ExecuteScalar<int>(sqlInsertCity, new {City = insertCity.City, CountryId = countryId, insertCity.LastUpdate});
        return cityId;
    }

    [Test]
    public void InsertCityExercisesTest()
    {
        // Arrange
        Exercises2 sut = new Exercises2();


        DateTime lastUpdated = DateTime.Now;
        InsertCountry country = new InsertCountry()
        {
            Country = "Belgium",
            LastUpdate = lastUpdated
        };
        
        InsertCity city = new InsertCity()
        {
            City = "Antwerp",
            LastUpdate = lastUpdated
        };
        
        // Act
        int result = sut.InsertCityExercises(country, city);
        
        // Assert
        result.Should().Be(1);
        
        using MySqlConnection connection = new MySqlConnection(GetConnectionString());
        string sql = """
                     SELECT city_id as CityId, city as City, country_id as CountryId, last_update as LastUpdate
                         FROM city_copy
                     """;
        
        CityForTest cityCopy = connection.Query<CityForTest>(sql).First();
        cityCopy.Should().BeEquivalentTo(new CityForTest()
        {
            CityId = 1, CountryId = 1
            //LastUpdate = lastUpdated -- database applies rounding the seconds :-(
        }, options => options.Excluding(x => x.LastUpdate));
        
        // Arrange
        InsertCity city2 = new InsertCity()
        {
            City = "Brussels",
            LastUpdate = lastUpdated
        };
    }
    
    public class CityForTest
    {
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
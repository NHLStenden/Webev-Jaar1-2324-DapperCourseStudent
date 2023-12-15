using System.Data;
using Dapper;
using FluentAssertions;
using MySql.Data.MySqlClient;

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
    // https://dev.mysql.com/doc/sakila/en/sakila-structure.html
    private static string GetConnectionString()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    
    [Test]
    public void TestDatabaseIsCreatedAndFilled()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var sql = """SELECT COUNT(*) FROM nicer_but_slower_film_list""";
        var count = connection.ExecuteScalar<int>(sql);
        count.Should().Be(1000);
    }
    
    // Write a query that return the one customer by its id
    // Use string interpolation or concatenation to create the query string and pass it to the Query method
    // Use the Customer class below to map the result
    // Use the Query() method to execute the query, (it was better to use QuerySingle() method, but this is harder to get all the data with SQL injection)
    public class CustomerForSqlInjection
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
    
    public List<CustomerForSqlInjection> GetCustomerByEmail(string email)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var sql = $"""
                    SELECT customer_id as CustomerId, first_name as FirstName, last_name as LastName, email as Email 
                    FROM customer WHERE email = '{email}'
                   """;
        var customer = connection.Query<CustomerForSqlInjection>(sql);
        return customer.ToList();
    }
    
    [Test]
    public void SqlInjectionTest()
    {
        var email = "PATRICIA.JOHNSON@sakilacustomer.org";
        using var connection = new MySqlConnection(GetConnectionString());
        
        GetCustomerByEmail(email).Should().HaveCount(1);

        var allCustomers = GetCustomerByEmail("PATRICIA.JOHNSON@sakilacustomer.org' OR 1 = 1 -- ");
        allCustomers.Should().HaveCount(599); //this demonstrates SQL injection, we get all the customers instead of the one we want
    }
    
    


}
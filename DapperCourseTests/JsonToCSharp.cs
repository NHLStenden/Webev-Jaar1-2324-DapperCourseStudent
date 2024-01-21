using System.Text.Json;
using Dapper;
// using MySql.Data.MySqlClient;
using MySqlConnector;

namespace DapperCourseTests;

public class JsonToCSharp
{
    public class Actor
    {
        public int ActorId { get; set; }
        public string FistName { get; set; }
        public string LastName { get; set; }
    }

    public class Address
    {
        public string adress { get; set; }
        public string address2 { get; set; }
        public string district { get; set; }
        public int cityId { get; set; }
        public City city { get; set; }
        public string postalCode { get; set; }
        public string phone { get; set; }
    }

    public class City
    {
        public int cityId { get; set; }
        public string city { get; set; }
        public int countryId { get; set; }
        public Country country { get; set; }
    }

    public class Country
    {
        public int countryId { get; set; }
        public string country { get; set; }
        public string lastUpdate { get; set; }
    }

    public class Movie
    {
        public int FilmId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Actor> Actors { get; set; }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }
        public int StoreId { get; set; }
        public int Active { get; set; }
        public string CreateDate { get; set; }
        public string LastUpdate { get; set; }
        public Address Address { get; set; }
        public List<Movie> movies { get; set; }
    }
    
    [Test]
    public void TestGetCustomersWithRentedMovies()
    {
        var customers = GetCustomersWithRentedMovies();
        Assert.AreEqual(19, customers.Count);
    }

    public static List<Customer> GetCustomersWithRentedMovies()
    {
        var sql = """
                  SELECT JSON_MERGE_PATCH(c.CutsomerObject,
                                          JSON_OBJECT('movies',
                                                      JSON_ARRAYAGG(DISTINCT f.FilmActorAsJson ORDER BY JSON_EXTRACT(f.FilmActorAsJson, '$.Title')) )) as CustomerObject
                  FROM rental r
                           JOIN customersAsJson c on r.customer_id = c.customer_id
                           JOIN inventory  i ON r.inventory_id = i.inventory_id
                           JOIN filmAndActorAsJson f ON i.film_id = f.film_Id
                  -- WHERE c.customer_id = 19
                  GROUP BY c.customer_id;
                  """;
        
        using var connection =
            new MySqlConnection("host=localhost;port=3307;user id=root;password=Test@1234!;database=sakila;");

        var jsonCustomers = connection.Query<string>(sql)
            .Select(jsonCustomer => JsonSerializer.Deserialize<Customer>(jsonCustomer)!)
            .ToList();
        
        return jsonCustomers;
    }
    
}
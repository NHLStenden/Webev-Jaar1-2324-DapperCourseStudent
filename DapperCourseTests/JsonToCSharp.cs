using System.Text.Json;
using Dapper;
// using MySql.Data.MySqlClient;
using MySqlConnector;

namespace DapperCourseTests;

public class JsonToCSharp
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Actor
    {
        public int ActorId { get; set; }
        public string FistName { get; set; }
        public string LastName { get; set; }
    }

    public class Address
    {
        public City City { get; set; }
        public string Phone { get; set; }
        public int CityId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
    }

    public class City
    {
        public string Name { get; set; }
        public int CityId { get; set; }
        public Country Country { get; set; }
        public int CountryId { get; set; }
    }

    public class Country
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
        public string LastUpdate { get; set; }
    }

    public class Movie
    {
        public string Title { get; set; }
        public List<Actor> Actors { get; set; }
        public int FilmId { get; set; }
        public string Description { get; set; }
    }

    public class Customer
    {
        public string Email { get; set; }
        public int Active { get; set; }
        public List<Movie> Movies { get; set; }
        public Address Address { get; set; }
        public int StoreId { get; set; }
        public string LastName { get; set; }
        public int AddressId { get; set; }
        public string FirstName { get; set; }
        public string CreateDate { get; set; }
        public int CustomerId { get; set; }
        public string LastUpdate { get; set; }
    }


    
    [Test]
    public async Task TestGetCustomersWithRentedMovies()
    {
        List<Customer> customers = GetCustomersWithRentedMovies();

        await Verify(customers);
    }

    public static List<Customer> GetCustomersWithRentedMovies()
    {
        // DISTINCT, ORDER BY, LIMIT only work in MariaDB in the JSON_ARRAYAGG() function. NOT IN MYSQL :-(. 
        // SELECT JSON_MERGE_PATCH(c.CutsomerObject,
        //     JSON_OBJECT('Movies',
        //     JSON_ARRAYAGG(DISTINCT f.FilmActorAsJson ORDER BY JSON_EXTRACT(f.FilmActorAsJson, '$.Title')) )) as CustomerObject
        
        string sql = """
                     SELECT JSON_MERGE_PATCH(c.CutsomerObject,
                                             JSON_OBJECT('Movies',
                                                         JSON_ARRAYAGG(f.FilmActorAsJson ))) as CustomerObject
                     FROM rental r
                              JOIN customersAsJson c on r.customer_id = c.customer_id
                              JOIN inventory  i ON r.inventory_id = i.inventory_id
                              JOIN filmAndActorAsJson f ON i.film_id = f.film_Id

                     WHERE c.customer_id = 19
                     GROUP BY c.customer_id;
                     """;
        
        using MySqlConnection connection =
            new MySqlConnection(ConnectionStrings.GetConnectionStringSakila());

        List<Customer> jsonCustomers = connection
            .Query<string>(sql)
            .Select(jsonCustomer => JsonSerializer.Deserialize<Customer>(jsonCustomer)!)
            .ToList();
        
        return jsonCustomers;
    }
    
}
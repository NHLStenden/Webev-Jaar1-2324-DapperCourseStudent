using System.Text.Json;
using Dapper;
using MySqlConnector;

namespace DapperCourseTests;

public class Examples3JsonViews
{
    private static readonly string ConnectionString;
    static Examples3JsonViews()
    {
        ConnectionString = ConnectionStrings.GetConnectionStringSakila();
    }
    
    public List<Customer> GetCustomerWithRentalsAndActors()
    {
        // The query below only works in MariaDB because DISTINCT, ORDER BY and LIMIT are not supported in JSON_ARRAYAGG() in MySQL
        // string mariaDbSQL =
        //     """
        //          SELECT JSON_MERGE_PATCH(c.CutsomerObject,
        //              JSON_OBJECT('movies',
        //              JSON_ARRAYAGG(DISTINCT f.FilmActorAsJson ORDER BY JSON_EXTRACT(f.FilmActorAsJson, '$.Title')) )) as CustomerObject
        //              FROM rental r
        //          JOIN customersAsJson c on r.customer_id = c.customer_id
        //          JOIN inventory  i ON r.inventory_id = i.inventory_id
        //          JOIN filmAndActorAsJson f ON i.film_id = f.film_Id
        //          WHERE c.customer_id = 19
        //          GROUP BY c.customer_id;
        //     """;

        // The views that are used in this example are created in the file: DapperCourseTests/SQL/json-views.sql
        // It's amazing that we can use views in this way to create JSON objects and hides the complexity of the query
        // Even if you don't use Dapper, you can use this technique to create JSON objects
        // This query can of course also be used as a view
        string sql =
            """
            SELECT JSON_MERGE_PATCH(c.CutsomerObject,
                                    JSON_OBJECT('Movies',
                                                JSON_ARRAYAGG(f.FilmActorAsJson) 
                                                )
                                    ) as CustomerObject
            FROM rental r
                 JOIN customersAsJson c on r.customer_id = c.customer_id
                     JOIN inventory  i ON r.inventory_id = i.inventory_id
                        JOIN filmAndActorAsJson f ON i.film_id = f.film_Id
            WHERE c.customer_id = 19
            GROUP BY c.customer_id;
            """;

        //To create the datamodel from the query I use the website: https://json2csharp.com/
        //This way I don't need to create the datamodel by hand :-). 
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        return 
            connection
                .Query<string>(sql)
                .Select(x => JsonSerializer.Deserialize<Customer>(x)!)
                .ToList();
    }
    
    [Test]
    public async Task TestCustomerWithRentalsAndActors()
    {
        List<Customer> customers = GetCustomerWithRentalsAndActors();
        customers.ForEach(c =>
        {
            c.Movies = c.Movies.OrderBy(m => m.FilmId).ToList();
            c.Movies.ForEach(m =>
            {
                m.Actors = m.Actors.OrderBy(a => a.ActorId).ToList();
            });
        });

        await Verify(customers);
    }
    
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

}
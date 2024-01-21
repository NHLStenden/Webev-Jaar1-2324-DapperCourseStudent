using Dapper;
using MySqlConnector;

namespace DapperCourse;

public class RelationshipsSakila
{
    // https://medium.com/dapper-net/multiple-mapping-d36c637d14fa
    // https://dapper-tutorial.net/querymultiple
    public class Actor
    {
        public int ActorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LastUpdate { get; set; }
    }
    
    private static string GetConnectionStringForShop()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    
    public List<Actor> GetActors()
    {
        string sql =
            $"""
             SELECT actor_id AS {nameof(Actor.ActorId)}, first_name AS {nameof(Actor.FirstName)}, 
             last_name as {nameof(Actor.LastName)}, last_update AS {nameof(Actor.LastUpdate)} FROM actor
             """;
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var actors = connection.Query<Actor>(sql);
        return actors.ToList();
    }
    
    
    public class Customer
    {
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int AddressId { get; set; }
        
        public Address Address { get; set; } = null!;
        
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class Address
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; } = null!;
        public string Address2 { get; set; } = null!;
        public string District { get; set; } = null!;
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Phone { get; set; } = null!;
        // public byte[] Location { get; set; }
        //
        // public MySqlGeometry LocationAsGeometry
        // {
        //     get
        //     {
        //         var geo = new MySqlGeometry(MySqlDbType.Geometry, Location);
        //         return geo;
        //     }
        // }
        public DateTime LastUpdate { get; set; }
    }

    public List<Customer> GetCustomerIncludeAddress()
    {
        string sql = 
            """
                SELECT c.customer_id AS CustomerId, c.store_id AS StoreId, c.first_name AS FirstName,
                c.last_name AS LastName, c.email AS Email, c.address_id AS AddressId, c.active AS Active,
                c.create_date AS CreateDate, c.last_update AS LastUpdate,
                
                c.address_id AS AddressIdSplit,
                
                a.address_id AS AddressId, a.address AS Address1, a.address2 AS Address2, a.district AS District,
                a.city_id AS CityId, a.postal_code AS PostalCode, a.phone AS Phone, 
                a.location AS Location, 
                a.last_update AS LastUpdate
                FROM customer c JOIN address a on a.address_id = c.address_id
            """;
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var customers = connection.Query<Customer, Address, Customer>(sql, 
            (customer, address) =>
            {
                customer.Address = address;
                return customer;
            }, splitOn: "AddressIdSplit");
        return customers.ToList();
    }

    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
    
    public List<Customer> GetCustomerIncludeAddressIncludeCity()
    {
        string sql = 
            """
                SELECT c.customer_id AS CustomerId, c.store_id AS StoreId, c.first_name AS FirstName,
                c.last_name AS LastName, c.email AS Email, c.active AS Active,
                c.create_date AS CreateDate, c.last_update AS LastUpdate, 
                c.address_id AS AddressId,
                
                a.address_id AS AddressIdSplit,
                
                a.address_id AS AddressId, a.address AS Address1, a.address2 AS Address2, a.district AS District,
                a.postal_code AS PostalCode, a.phone AS Phone,
                a.location AS Location, a.last_update AS LastUpdate,
                
                ci.city_id AS CityIdSplit,
                
                ci.city_id AS CityId,
                ci.city as Name, ci.country_id AS CountryId, ci.last_update as LastUpdate
                FROM customer c 
                    JOIN address a on a.address_id = c.address_id
                        JOIN city ci on ci.city_id = a.city_id
            """;
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var customers = connection.Query<Customer, Address, City, Customer>(sql, 
            (customer, address, city) =>
            {
                customer.Address = address;
                customer.Address.City = city;
                return customer;
            }, splitOn: "AddressIdSplit, CityIdSplit");
        return customers.ToList();
    }

    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
    
    //The name of table is Country and it has a column named Country (same name as the table),
    //in C# we can't have a property with the same name as the class! So we need to give it a different name, such as Name.
    //Don't forget to use the AS keyword in the SQL query to give country (column name) a different name.
    //This AS keyword is used to give a column a different name which officially is called an alias.
    
    //Take a look at the result, the Id's are zero for the foreign keys,
    //this is because Dapper is a simple mapper and doesn't know which column belongs to which property
    //(customer.address_id AS AddressId is the same as address.customerId AS AddressId).
    //We can solve this by manually mapping the properties to the columns in the map function!
    //This problem is also explained in: https://dapper-tutorial.net/querymultiple
    
    //The splitting can be problematic when you have multiple columns with the same name in different tables.
    //A trick that I use is the following: I add a special column in the query that I use to split the result
    //(c.customer_id as CustomerIdSplit, a.address_id AS AddressIdSplit).
    public List<Customer> GetCustomerIncludeAddressIncludeCityIncludeCountry()
    {
        string sql = 
            """
                SELECT c.customer_id AS CustomerId, c.store_id AS StoreId, c.first_name AS FirstName,
                c.last_name AS LastName, c.email AS Email, c.active AS Active,
                c.address_id AS AddressId,  
                c.create_date AS CreateDate, c.last_update AS LastUpdate,
                c.customer_id as CustomerIdSplit,
                
                a.address_id AS AddressId,
                a.address AS Address1, 
                a.address2 AS Address2, a.district AS District,
                a.postal_code AS PostalCode, a.phone AS Phone,
                a.location AS Location, a.last_update AS LastUpdate,
                a.address_id AS AddressIdSplit,
                
                a.city_id AS CityId,
                ci.city as Name, ci.last_update as LastUpdate,
                ci.city_id AS CityIdSplit,
                
                ci.country_id AS CountryId,
                co.country AS Name, co.last_update AS LastUpdate
                FROM customer c
                    JOIN address a on a.address_id = c.address_id
                        JOIN city ci on ci.city_id = a.city_id
                            JOIN country co on co.country_id = ci.country_id
            """;
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var customers = connection.Query<Customer, Address, City, Country, Customer>(sql, 
            (customer, address, city, country) =>
            {
                customer.Address = address;
                // customer.AddressId = address.AddressId;
                customer.Address.City = city;
                // customer.Address.CityId = city.CityId;
                customer.Address.City.Country = country;
                // customer.Address.City.CountryId = country.CountryId;
                return customer;
            }, splitOn: "CustomerIdSplit, AddressIdSplit, CityIdSplit");
        return customers.ToList();
    }
    


    
}
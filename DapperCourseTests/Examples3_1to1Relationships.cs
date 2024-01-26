using Dapper;
using MySqlConnector;

namespace DapperCourseTests;

public class Examples3_1to1Relationships
{
    // https://medium.com/dapper-net/multiple-mapping-d36c637d14fa
    // https://dapper-tutorial.net/querymultiple

    
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
    
    private static string GetConnectionStringForShop()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    
    //This query will return all the customers, to load the address we have to execute a query for each customer.
    //Foreach customer we have to execute a query to get the address from the database. This is done in the foreach loop.
    //This is called the N+1 problem, because we have 1 query to get the customers (1 Query) and N queries to get the addresses (foreach loop).
    //Where N is the number of customers.
    //This approach should be avoided, because it will result in a lot of queries to the database.
    //Ultimately this will result in a slow application or even a timeouts!
    public List<Customer> GetCustomersWithAddressNPlusOneProblem()
    {
        string sql =
            $"""
             SELECT customer_id AS {nameof(Customer.CustomerId)}, store_id AS {nameof(Customer.StoreId)}, 
                    first_name AS {nameof(Customer.FirstName)}, last_name as {nameof(Customer.LastName)}, 
                    email AS {nameof(Customer.Email)}, address_id AS {nameof(Customer.AddressId)}, 
                    active AS {nameof(Customer.Active)}, create_date AS {nameof(Customer.CreateDate)}, 
                    last_update AS {nameof(Customer.LastUpdate)} 
             FROM customer
             ORDER BY customer_id
             LIMIT 3
             """;
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var customers = connection.Query<Customer>(sql)
            .ToList();  //1 Query
        foreach (var customer in customers) // N Queries
        {
            var sqlAddress = """
                      SELECT    address_id AS AddressId, address AS Address1, address2 AS Address2, district AS District,
                                city_id AS CityId, postal_code AS PostalCode, phone AS Phone, 
                                -- location AS Location, 
                                last_update AS LastUpdate
                      FROM address 
                      WHERE address_id = @AddressId
                      """;
            
            //Every time we execute this query, we have to go to the database and get the address.
            //This is called the N+1 problem, because we have 1 query to get the customers and N queries to get the addresses.
            var address = connection.QuerySingle<Address>(sqlAddress, new
            {
                AddressId = customer.AddressId
            });
            customer.Address = address;
        }
        
        return customers.ToList();
    }
    
    [Test]
    public async Task GetCustomersWithAddressNPlusOneProblemTest()
    {
        // Arrange
        var sut = new Examples3_1to1Relationships();
        
        // Act
        var customers = sut.GetCustomersWithAddressNPlusOneProblem();
        
        // Assert
        await Verify(customers);
    }
   
    // This query will return all the customers and the address in one query.
    // This is done by joining the customer table with the address table.
    // The result of the query is mapped to the Customer class.
    // The address is mapped to the Address class.
    // The splitOn parameter is used to tell Dapper where to split the result.
    // Every column after the splitOn column will be mapped to the Address class.
    
    // Be aware of the fact that if multiple customers have the same address,
    // the address will be duplicated in the result! In this case we have a 1 to 1 relationship,
    // so this is not a problem.
    
    // The result is mapped to the Customer class and the Address class, this is done by the map function
    // in the Query function. The map function is called for every row in the result. map is a
    // anonymous function that takes a customer and an address as input and returns a customer.
    // The customer is returned because we want to return a list of customers.
    // You can see that the customer.Address is set to the address.
    // The Query<T1,T2,T3> function is used because we want to map the result to the Customer and Address class.
    //  connection.Query<Customer, Address, Customer>(...) is used. The Customer, Address, Customer are the types (Generic arguments).
    // The first type argument Customer is the type of the Customer (first argument of the map function).
    // The second type argument Address is the type of the Address (second argument of the map function).
    // The third type argument Customer is the type of the result (the type that is returned by the map function).
    public List<Customer> GetCustomerIncludeAddress()
    {
        string sql = 
            """
                SELECT 
                    c.customer_id AS CustomerId, c.store_id AS StoreId, c.first_name AS FirstName,
                    c.last_name AS LastName, c.email AS Email, c.address_id AS AddressId, c.active AS Active,
                    c.create_date AS CreateDate, c.last_update AS LastUpdate,
                
                'AddressSplit' AS AddressSplit,
                
                    a.address_id AS AddressId, a.address AS Address1, a.address2 AS Address2, a.district AS District,
                    a.city_id AS CityId, a.postal_code AS PostalCode, a.phone AS Phone, 
                    -- a.location AS Location, 
                    a.last_update AS LastUpdate
            
                FROM customer c 
                    JOIN address a on a.address_id = c.address_id
                
                ORDER BY c.customer_id
                LIMIT 3
            """;
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        IEnumerable<Customer> customers = connection.Query<Customer, Address, Customer>(sql, 
            map: (customer, address) =>
            {
                customer.Address = address;
                return customer;
            }, 
            splitOn: "AddressSplit");
        return customers.ToList();
    }
    
    [Test]
    public async Task GetCustomerIncludeAddressTest()
    {
        // Arrange
        var sut = new Examples3_1to1Relationships();
        
        // Act
        var customers = sut.GetCustomerIncludeAddress();
        
        // Assert
        await Verify(customers);
    }

    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        public Country Country { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
    
    // Now we want to include the city in the result. We can do this by joining the city table.
    // It's actually the same as the previous query, but now we join the city table.
    // Now we have to split the result on the AddressId and the CityId.
    // Take a careful look at the generic arguments of the Query function.
    // Also take a look at the map function which has one extra argument (city) compared to the previous query.
    
    // The city is mapped with, customer.Address.City = city;
    public List<Customer> GetCustomerIncludeAddressIncludeCity()
    {
        string sql = 
            """
                SELECT c.customer_id AS CustomerId, c.store_id AS StoreId, c.first_name AS FirstName,
                c.last_name AS LastName, c.email AS Email, c.active AS Active,
                c.create_date AS CreateDate, c.last_update AS LastUpdate, 
                c.address_id AS AddressId,
                
                'AddressSplit' AS AddressSplit,
                
                a.address_id AS AddressId, a.address AS Address1, a.address2 AS Address2, a.district AS District,
                a.postal_code AS PostalCode, a.phone AS Phone,
                a.location AS Location, a.last_update AS LastUpdate,
                
                'CitySplit' AS CitySplit,
                
                ci.city_id AS CityId,
                ci.city as Name, ci.country_id AS CountryId, ci.last_update as LastUpdate
                FROM customer c 
                    JOIN address a on a.address_id = c.address_id
                        JOIN city ci on ci.city_id = a.city_id
                        
                ORDER BY c.customer_id
                LIMIT 3
            """;
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var customers = connection.Query<Customer, Address, City, Customer>(sql, 
            (customer, address, city) =>
            {
                customer.Address = address;
                customer.Address.City = city;
                return customer;
            }, splitOn: "AddressSplit, CitySplit");
        return customers.ToList();
    }
    
    [Test]
    public async Task GetCustomerIncludeAddressIncludeCityTest()
    {
        // Arrange
        var sut = new Examples3_1to1Relationships();
        
        // Act
        var customers = sut.GetCustomerIncludeAddressIncludeCity();
        
        // Assert
        await Verify(customers);
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
                
                'CustomerSplit' as CustomerSplit,
                
                a.address_id AS AddressId,
                a.address AS Address1, 
                a.address2 AS Address2, a.district AS District,
                a.postal_code AS PostalCode, a.phone AS Phone,
                a.location AS Location, a.last_update AS LastUpdate,
                
                'AddressSplit' AS AddressSplit,
                
                a.city_id AS CityId,
                ci.city as Name, ci.last_update as LastUpdate,
                
                'CitySplit' AS CitySplit,
                
                ci.country_id AS CountryId,
                co.country AS Name, co.last_update AS LastUpdate
                FROM customer c
                    JOIN address a on a.address_id = c.address_id
                        JOIN city ci on ci.city_id = a.city_id
                            JOIN country co on co.country_id = ci.country_id
                            
                ORDER BY c.customer_id
                LIMIT 3
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
            }, splitOn: "CustomerSplit, AddressSplit, CitySplit");
        return customers.ToList();
    }
    
    [Test]
    public async Task GetCustomerIncludeAddressIncludeCityIncludeCountryTest()
    {
        // Arrange
        var sut = new Examples3_1to1Relationships();
        
        // Act
        var customers = sut.GetCustomerIncludeAddressIncludeCityIncludeCountry();
        
        // Assert
        await Verify(customers);
    }
}
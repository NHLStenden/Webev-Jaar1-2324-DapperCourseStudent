using System.Text.Json;
using Dapper;
using FluentAssertions;
using MySqlConnector;

namespace DapperCourseTests;

public class Exercises3
{
    private readonly string _connectionString;

    public Exercises3()
    {
        _connectionString = ConnectionStrings.GetConnectionStringSakila();
    }
    //In this exercise we will use the sakila database to practice with relationships.
    //Each payment has one customer
    //Create a query that returns all payments with the customer information
    //ORDER BY payment_id
    //From the perspective of the payment, each payment has one customer (1 to 1).
    //Actually this is a 1 to many relationship, because a customer can have multiple payments.
    //There are no 1 to 1 relationships in the sakila database, so we will use the 1 to many relationship as 1 to 1 example.
    //Actually 1 to 1 relationships are not very common in databases, because you can just add the columns to the same table.

    //Tip: if you are joining two or more tables with 1 to 1 relationships, you can use the one query and split the result.
    //Use the Query<TFirst, TSecond, TReturn>(...) method for this. 
    public class Payment
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int StaffId { get; set; }

        // public Staff Staff { get; set; } = null!;
        public int RentalId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime LastUpdate { get; set; }
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
    }

  

    public List<Payment> ExerciseOneToOne()
    {
        string sql = $@"
            SELECT payment_id AS {nameof(Payment.PaymentId)}, 
                    p.customer_id AS {nameof(Payment.CustomerId)},
                    staff_id AS {nameof(Payment.StaffId)},
                    rental_id AS {nameof(Payment.RentalId)},
                    amount AS {nameof(Payment.Amount)},
                    payment_date AS {nameof(Payment.PaymentDate)},
                    p.last_update AS {nameof(Payment.LastUpdate)},
                    'SplitCustomer' as 'SplitCustomer',
                    c.customer_id AS {nameof(Customer.CustomerId)},
                    store_id AS {nameof(Customer.StoreId)},
                    first_name AS {nameof(Customer.FirstName)},
                    last_name AS {nameof(Customer.LastName)},
                    email AS {nameof(Customer.Email)}
            FROM payment p
                JOIN customer c ON p.customer_id = c.customer_id
            ORDER BY payment_id
                ";
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        List<Payment> result = connection.Query<Payment, Customer, Payment>(sql, (payment, customer) =>
            {
                payment.Customer = customer;
                return payment;
            }, splitOn: "SplitCustomer").ToList();
        return result;
    }

    [Test]
    public void TestExerciseOneToOne()
    {
        List<Payment> payments = ExerciseOneToOne();
        payments.Should().HaveCount(16044);

        payments.First().Amount.Should().Be(2.99m);
        payments.First().PaymentId.Should().Be(1);
        payments.First().CustomerId.Should().Be(1);
        payments.First().StaffId.Should().Be(1);
        payments.First().RentalId.Should().Be(76);
        payments.First().Customer.CustomerId.Should().Be(1);
        payments.First().Customer.FirstName.Should().Be("MARY");
        payments.First().Customer.LastName.Should().Be("SMITH");
        payments.First().Customer.Email.Should().Be("MARY.SMITH@sakilacustomer.org");
        payments.First().StaffId.Should().Be(1);

        payments.Last().Amount.Should().Be(2.99m);
        payments.Last().CustomerId.Should().Be(599);
        payments.Last().StaffId.Should().Be(2);
        payments.Last().RentalId.Should().Be(15725);
        payments.Last().Customer.CustomerId.Should().Be(599);
        payments.Last().Customer.FirstName.Should().Be("AUSTIN");
        payments.Last().Customer.LastName.Should().Be("CINTRON");
        payments.Last().Customer.Email.Should().Be("AUSTIN.CINTRON@sakilacustomer.org");
        payments.Last().StaffId.Should().Be(2);
    }



    // Same as ExerciseOneToOne, but now use add another join the address table with customer.
    // The address table has a one to one relationship with the customer table.
    
    public class Address
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; } = null!;
    }
    
    public List<Payment> ExerciseOneToOneTwoJoins()
    {
        string sql = $@"
            SELECT payment_id AS {nameof(Payment.PaymentId)}, 
                    p.customer_id AS {nameof(Payment.CustomerId)},
                    staff_id AS {nameof(Payment.StaffId)},
                    rental_id AS {nameof(Payment.RentalId)},
                    amount AS {nameof(Payment.Amount)},
                    payment_date AS {nameof(Payment.PaymentDate)},
                    p.last_update AS {nameof(Payment.LastUpdate)},
                    'SplitCustomer' as 'SplitCustomer',
                    c.customer_id AS {nameof(Customer.CustomerId)},
                    c.address_id AS {nameof(Customer.AddressId)},
                    store_id AS {nameof(Customer.StoreId)},
                    first_name AS {nameof(Customer.FirstName)},
                    last_name AS {nameof(Customer.LastName)},
                    email AS {nameof(Customer.Email)},
                    'SplitAddress' as 'SplitAddress',
                    a.address_id AS {nameof(Address.AddressId)},
                    address AS {nameof(Address.Address1)}
            FROM payment p
                JOIN customer c ON p.customer_id = c.customer_id
                JOIN address a ON c.address_id = a.address_id
            ORDER BY payment_id
                ";
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        List<Payment> result = connection.Query<Payment, Customer, Address, Payment>(sql, (payment, customer, address) =>
            {
                payment.Customer = customer;
                payment.Customer.Address = address;
                return payment;
            }, splitOn: "SplitCustomer, SplitAddress").ToList();
        return result;
    }

    [Test]
    public void TestExerciseOneToOne2Joins()
    {
        List<Payment> payments = ExerciseOneToOneTwoJoins();
        payments.Should().HaveCount(16044);

        payments.First().Amount.Should().Be(2.99m);
        payments.First().PaymentId.Should().Be(1);
        payments.First().CustomerId.Should().Be(1);
        payments.First().StaffId.Should().Be(1);
        payments.First().RentalId.Should().Be(76);
        payments.First().Customer.CustomerId.Should().Be(1);
        payments.First().Customer.FirstName.Should().Be("MARY");
        payments.First().Customer.LastName.Should().Be("SMITH");
        payments.First().Customer.Email.Should().Be("MARY.SMITH@sakilacustomer.org");
        payments.First().Customer.AddressId.Should().Be(5);
        payments.First().Customer.Address.AddressId.Should().Be(5);
        payments.First().Customer.Address.Address1.Should().Be("1913 Hanoi Way");


        payments.First().StaffId.Should().Be(1);

        payments.Last().Amount.Should().Be(2.99m);
        payments.Last().CustomerId.Should().Be(599);
        payments.Last().StaffId.Should().Be(2);
        payments.Last().RentalId.Should().Be(15725);
        payments.Last().Customer.CustomerId.Should().Be(599);
        payments.Last().Customer.FirstName.Should().Be("AUSTIN");
        payments.Last().Customer.LastName.Should().Be("CINTRON");
        payments.Last().Customer.Email.Should().Be("AUSTIN.CINTRON@sakilacustomer.org");
        payments.Last().StaffId.Should().Be(2);

        payments.Last().Customer.AddressId.Should().Be(605);
        payments.Last().Customer.Address.AddressId.Should().Be(605);
        payments.Last().Customer.Address.Address1.Should().Be("1325 Fukuyama Street");
    }

    //In this exercise we will move to 1 to many (1 to N) relationships.
    //A store has many customers (List<Customer>)
    //Create a query that returns all stores with the customers
    //Order by store_id, customer_id
    //Limit the result to 10 rows, this create a problem we have only 10 rows,
    //so we don't have all the customers of each store.
    //Take a look at the examples, try different approaches.
    //  -dictionary method
    public class Store
    {
        public int StoreId { get; set; }
        public int ManagerStaffId { get; set; }
        public List<Customer> Customers { get; set; } = null!;
    }

    public List<Store> ExerciseOneToManyWithDictionaryMethod()
    {
        using MySqlConnection connection = new MySqlConnection(_connectionString);
        string sql = $@"
            SELECT s.store_id AS {nameof(Store.StoreId)},
                    manager_staff_id AS {nameof(Store.ManagerStaffId)},
                    'SplitCustomer' as 'SplitCustomer',
                    c.store_id AS {nameof(Customer.StoreId)},    
                    c.customer_id AS {nameof(Customer.CustomerId)},
                    c.address_id AS {nameof(Customer.AddressId)},                  
                    first_name AS {nameof(Customer.FirstName)},
                    last_name AS {nameof(Customer.LastName)},
                    email AS {nameof(Customer.Email)}  
            FROM store s 
                JOIN customer c ON s.store_id = c.store_id
            ORDER BY s.store_id, c.customer_id
            LIMIT 10";

        Dictionary<int, Store> dictionary = new Dictionary<int, Store>();
        IEnumerable<Store> result = connection.Query<Store, Customer, Store>(sql
            , (store, customer) =>
            {
                if (!dictionary.TryGetValue(store.StoreId, out Store? storeEntry))
                {
                    storeEntry = store;
                    storeEntry.Customers = new List<Customer>();
                    dictionary.Add(storeEntry.StoreId, storeEntry);
                }

                storeEntry.Customers.Add(customer);
                return store;
            }, splitOn: $"SplitCustomer");
        return dictionary.Values.ToList();
    }

    [Test]
    public async Task TestExerciseOneToManyWithDictionaryMethod()
    {
        List<Store> stores = ExerciseOneToManyWithDictionaryMethod();
        stores.Should().HaveCount(1);

        stores.First().Customers.Should().HaveCount(10);

        await Verify(stores);
    }
    
    //The same as the previous exercise, but now use the JSON method (see examples).
    //The JSON method returns all the customers.
    //We use GROUP BY in SQL we can not use LIMIT, ORDER BY inside the JSON_ARRAYAGG().
    //This is a problem, actually it works in MariaDB, but not in MySQL!
    
    //In my option the JSON method is easier than the Dapper way (dictionary method).
    //What I think is important is that you know both methods.
    //If it's possible avoid complex nested Types as result of Queries anyway.
    //It's better to use a flat result type or 1 to 1 relationships.
    //Remember that it's always a good idea to create a view (abstract away the complexity)
    //and no complex nested types in the result if possible!
    //But sometimes you have to deal with complex nested types! That's why I have created these exercises!
    //Or use a view (abstract away the complexity) and no complex nested types in the result.
    //But sometimes you have to deal with complex nested types! That's why I have created these examples.
    //I hope you have learned something from it.
    
    //There are three JSON methods that are useful for this:
    //JSON_OBJECT() --> Create a Json Object which can be deserialized to a C# Type
    //JSON_ARRAYAGG() --> Create a Json Array which can be deserialized to a List<C# Type> (or other list like types)
    //JSON_MERGE_PATCH() --> Merge two Json Objects (or more) into one Json Object,
    //  this can be used when you views that return Json Objects and you want to merge them into one Json Object.
    private List<Store> ExerciseOneToManyWithJsonMethod()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task ExerciseOneToManyJsonTest()
    {
        List<Store> stores = ExerciseOneToManyWithJsonMethod();
        
        stores.Should().HaveCount(2);
        
        await Verify(stores);
    }

    //N to N relationships are a bit more complex.
    //We will join actors with films (the in between table is film_actor) 
    //The method should return a list of films with the actors.
    //Order by film_id, Limit the result to 5 rows (actually it strange because now we have 1 films with NOT all the actors!)
    //Use the dictionary method
    public class Actor
    {
        public int ActorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    public class Film
    {
        public int FilmId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<Actor> Actors { get; set; } = null!;
    }

    public List<Film> GetFilmsIncludeActorsDictionaryMethod()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetFilmsIncludeActorsTest()
    {
        //Arrange & Act
        List<Film> films = GetFilmsIncludeActorsDictionaryMethod();

        films.Should().HaveCount(1);
        films.First().Actors.Should().HaveCount(5);
        
        //Assert
        films.Should().HaveCount(1);
        
        await Verify(films);
    }
    
    //The same a the previous exercise, but now use the JSON method (see examples)
    //Limit the result to 5 rows, no we have all the actors for each film
    // (the order in which they are returned we can not control in Mysql,
    // if you what to use this use MariaDB instead)
    
    // Take a look at the examples, a one to many relationship and many to many relationship are actually the same.
    // This makes sense, because a many to many relationship is a one to many relationship in both directions.
    // The only difference is that you have to join the in between table.
    // From the perspective of C# classes both types of relationships (1-1 and 1-many) are the same,
    // in other words they are both 1 to many relationships and result in a List<T> property (many side).
    public List<Film> GetFilmsIncludeActorsJsonMethod()
    {
        throw new NotImplementedException();
    }
    
    [Test]
    public async Task GetFilmsIncludeActorsJsonTest()
    {
        //Arrange & Act
        List<Film> films = GetFilmsIncludeActorsJsonMethod();

        //Assert
        films.Should().HaveCount(1);
        
        await Verify(films);
    }
}
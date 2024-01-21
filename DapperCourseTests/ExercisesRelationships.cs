using System.Text.Json;
using Dapper;
using FluentAssertions;
using MySqlConnector;

namespace DapperCourseTests;

public class ExercisesRelationships
{
    private static string GetConnectionString()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }

    //In this exercise we will use the sakila database to practice with relationships.
    //Each payment has one customer
    //Create a query that returns all payments with the customer information
    //ORDER BY payment_id
    //From the perspective of the payment, each payment has one customer (1 to 1).
    //Actually this is a 1 to many relationship, because a customer can have multiple payments.
    //There are no 1 to 1 relationships in the sakila database, so we will use the 1 to many relationship as 1 to 1 example.
    //Actually 1 to 1 relationships are not very common in databases, because you can just add the columns to the same table.

    //Tip: if you are joining two tables or more tables with 1 to 1 relationships, you can use the one query and split the result.
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

    public class Address
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; } = null!;
    }

    public List<Payment> ExerciseOneToOne()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var result = connection.Query<Payment, Customer, Payment>(
            $@"
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
                ", (payment, customer) =>
            {
                payment.Customer = customer;
                return payment;
            }, splitOn: "SplitCustomer");
        return result.ToList();
    }

    [Test]
    public void TestExerciseOneToOne()
    {
        var payments = ExerciseOneToOne();
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
    public List<Payment> ExerciseOneToOneTwoJoins()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var result = connection.Query<Payment, Customer, Address, Payment>(
            $@"
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
                    'SplitAddress' as SplitAddress,
                    a.address_id AS {nameof(Address.AddressId)},
                    address AS {nameof(Address.Address1)}
            FROM payment p
                JOIN customer c ON p.customer_id = c.customer_id
                    JOIN address a ON c.address_id = a.address_id
            ORDER BY payment_id
                ", (payment, customer, address) =>
            {
                payment.Customer = customer;
                payment.Customer.Address = address;
                return payment;
            }, splitOn: $"SplitCustomer,SplitAddress");
        return result.ToList();
    }

    [Test]
    public void TestExerciseOneToOne2Joins()
    {
        var payments = ExerciseOneToOneTwoJoins();
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
        using var connection = new MySqlConnection(GetConnectionString());

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

        var dictionary = new Dictionary<int, Store>();
        var result = connection.Query<Store, Customer, Store>(sql
            , (store, customer) =>
            {
                if (!dictionary.TryGetValue(store.StoreId, out var storeEntry))
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
        var stores = ExerciseOneToManyWithDictionaryMethod();
        stores.Should().HaveCount(1);

        stores.First().Customers.Should().HaveCount(10);

        await Verify(stores);
    }
    
    //The same as the previous exercise, but now use the JSON method (see examples).
    //The JSON method returns all the customers.
    //We use GROUP BY in SQL we can not use LIMIT on the aggregated result ( JSON_ARRAYAGG() ).
    //No solution is perfect, so you can choose which method you prefer.
    //  -dictionary method
    //  -json method
    //What I think is important is that you know both methods.
    //If it's possible avoid complex nested Types as result of Queries anyway.
    //Better to use a flat result type or 1 to 1 relationships.
    //Or use a view (abstract away the complexity) and no complex nested types in the result.
    //But sometimes you have to deal with complex nested types! That's why I have created these exercises.
    //I hope you have learned something from it.
    private List<Store> ExerciseOneToManyWithJsonMethod()
    {
        using var connection = new MySqlConnection(GetConnectionString());

        string sql = $@"
            SELECT JSON_OBJECT('StoreId', s.store_id,
                   'ManagerStaffId', s.manager_staff_id,
                   'Customers',
                   JSON_ARRAYAGG(
                           JSON_OBJECT('CustomerId', c.customer_id,
                                       'FirstName', c.first_name,
                                       'LastName', c.last_name,
                                       'Email', c.email,
                                       'AddressId', c.address_id,
                                       'StoreId', c.store_id
                                      )
                                 )  
                            )
            FROM store s
                    JOIN customer c ON s.store_id = c.store_id
            GROUP BY s.store_id
            ORDER BY s.store_id
            LIMIT 10";

        var result = new List<Store>();
        var jsonResult = connection.Query<string>(sql);
        foreach (var jsonObjStr in jsonResult)
        {
            var store = JsonSerializer.Deserialize<Store>(jsonObjStr);
            result.Add(store);
        }

        return result;
    }

    [Test]
    public async Task ExerciseOneToManyJsonTest()
    {
        var stores = ExerciseOneToManyWithJsonMethod();
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
        string sql = """
                         SELECT f.film_id AS FilmId, title AS Title, description AS Description,
                                'SplitActor' as 'SplitActor',
                                  a.actor_id AS ActorId,
                                     first_name AS FirstName,
                                     last_name AS LastName
                         FROM film f JOIN film_actor fa ON f.film_id = fa.film_id
                             JOIN actor a ON fa.actor_id = a.actor_id
                         ORDER BY f.film_id
                         LIMIT 5
                     """;

        using var connection = new MySqlConnection(GetConnectionString());
        var dictionary = new Dictionary<int, Film>();
        var result = connection.Query<Film, Actor, Film>(sql
            , (film, actor) =>
            {
                if (!dictionary.TryGetValue(film.FilmId, out var filmEntry))
                {
                    filmEntry = film;
                    filmEntry.Actors = new List<Actor>();
                    dictionary.Add(filmEntry.FilmId, filmEntry);
                }

                filmEntry.Actors.Add(actor);
                return film;
            }, splitOn: $"SplitActor");
        
        return dictionary.Values.ToList();
    }

    [Test]
    public async Task GetFilmsIncludeActorsTest()
    {
        //Arrange & Act
        var films = GetFilmsIncludeActorsDictionaryMethod();

        films.Should().HaveCount(1);
        films.First().Actors.Should().HaveCount(5);
        
        //Assert
        films.Should().HaveCount(1);
        
        await Verify(films);
    }
    
    //The same a the previous exercise, but now use the JSON method (see examples)
    //Limit the result to 5 rows, no we have all the actors for each film (the order in which they are returned we can not control)
    public List<Film> GetFilmsIncludeActorsJsonMethod()
    {
        string sql = """
                     SELECT JSON_OBJECT('FilmId', f.film_id, 'Title', title, 'Description', description,
                            'Actors', JSON_ARRAYAGG(JSON_OBJECT('ActorId', a.actor_id, 'FirstName', first_name, 'LastName', last_name)))
                         
                     FROM film f JOIN film_actor fa ON f.film_id = fa.film_id
                                 JOIN actor a ON fa.actor_id = a.actor_id
                     WHERE f.film_id = 1
                     GROUP BY f.film_id
                     ORDER BY f.film_id
                     LIMIT 5
                     """;
        
        using var connection = new MySqlConnection(GetConnectionString());
        var result = connection.Query<string>(sql);
        var films = new List<Film>();
        foreach (var jsonObjStr in result)
        {
            var film = JsonSerializer.Deserialize<Film>(jsonObjStr);
            films.Add(film);
        }

        return films;
    }
    
    [Test]
    public async Task GetFilmsIncludeActorsJsonTest()
    {
        //Arrange & Act
        var films = GetFilmsIncludeActorsJsonMethod();

        //Assert
        films.Should().HaveCount(1);
        
        await Verify(films);
    }
}
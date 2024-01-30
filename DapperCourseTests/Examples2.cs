using Dapper;
using FluentAssertions;
using MySqlConnector;

namespace DapperCourseTests;

public class Examples2
{
    private static readonly string ConnectionString;
    static Examples2()
    {
        ConnectionString = ConnectionStrings.GetConnectionStringSakila();
    }
    
    // Be really careful with SQL injection!
    // https://www.w3schools.com/sql/sql_injection.asp
    // It's a common mistake to use string concatenation (+ sign) or string interpolation ($ in combination with {}) to build SQL queries
    // This is a big security risk! A hacker can use SQL injection to access your database! He is able to read, modify or delete data!
    // Never ever use string concatenation or string interpolation to build SQL queries!!!!!
    // Never ever use string concatenation or string interpolation to build SQL queries!!!!!
    // Never ever use string concatenation or string interpolation to build SQL queries!!!!!
    // Never ever use string concatenation or string interpolation to build SQL queries!!!!!
    // SQL injection is one of the most common web hacking techniques according to OWASP (Open Web Application Security Project)!
    // Always use parameters! See the examples below!
    public List<FilmListSlower> SqlInjectionExample(string category)
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql = $@"
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                    WHERE category = '{category}'
                  ";
        
        //same problem with + (string concatenation (plus sign)
        //var sql = $@"
        //            SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price,
        //                   length as Length, rating as Rating, actors as Actors
        //            FROM nicer_but_slower_film_list
        //            WHERE category = '" + category + @"'
        //          ";
        
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(sql);
        return films.ToList();
    }
    
    [Test]
    public void SqlInjectionExampleTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> allCategoriesMovies = sut.SqlInjectionExample("Action' OR 1 = 1 -- ");
        
        // Assert
        allCategoriesMovies.Should().HaveCount(1000);
    }
    
    
    // In this example a view is used instead of a table, the view is called nicer_but_slower_film_list
    // You can take a look at the view in MySQL Workbench or in the IDE (Rider or Visual Studio)
    // The view nicer_but_slower_film_list is a join of the tables film, film_category and category, film_actor and actor
    // Left joins are used, so all films are returned, even if there are no actors or categories for a film!
    public class FilmListSlower
    {
        public int FilmId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Length { get; set; }
        public string Rating { get; set; }

        public string Actors { get; set; }
    }
    
    public List<FilmListSlower> GetFilmListSlower()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql = """
                       SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price,
                              length as Length, rating as Rating, actors as Actors
                       FROM nicer_but_slower_film_list
                     """;
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(sql);
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> films = sut.GetFilmListSlower();

        // Assert
        Assert.That(films, Is.Not.Null);
    }
    
    // In this example we use a parameter (category) to filter the films by category
    // The parameter is added to the SQL query with @Category
    // The parameter is added to the connection.Query method with param: new {Category = category}
    // This is a good practice to avoid SQL injection!
    // @Category in the SQL query is officially called a named parameter (because it has a name) or Query Parameter Placeholder.
    // https://www.learndapper.com/parameters/
    public List<FilmListSlower> GetFilmListSlowerWithCategoryParameter(string category)
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql = """
                       SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price,
                              length as Length, rating as Rating, actors as Actors
                       FROM nicer_but_slower_film_list
                       WHERE category = @Category
                     """;
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(sql, param: new {Category = category});
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerWithCategoryParameterTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> actionFilms = sut.GetFilmListSlowerWithCategoryParameter("Action");
        
        // Assert
        Assert.That(actionFilms, Is.Not.Null);
    }
    
    //Let's use parameters for Insertion, Updating and Deletion
    //To facilitate this, we create a new table called mywatchlist
    //Before we can use the table watchlist, we have to create it!
    //DROP TABLE IF EXISTS watchlist;
    //CREATE TABLE watchlist (
    //    id INT NOT NULL AUTO_INCREMENT,
    //    title VARCHAR(255) NOT NULL,
    //    PRIMARY KEY (id)
    //);
    //Use the Dapper method Execute() to create the database table, execute returns the number of affected rows
    //If the table is created successfully, the number of affected rows is 0 
    // (because the table is empty and no rows are affected).
    public int CreateDatabaseTableWatchlistAndWatchListCategory()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql = @"
                    DROP TABLE IF EXISTS watchlist;
                    CREATE TABLE watchlist (
                        id INT NOT NULL AUTO_INCREMENT,
                        title VARCHAR(255) NOT NULL,
                        watchlist_category_id INT NOT NULL REFERENCES watchlist_category(id),
                        PRIMARY KEY (id)
                    );

                    DROP TABLE IF EXISTS watchlist_category;
                    CREATE TABLE watchlist_category (
                        id INT NOT NULL AUTO_INCREMENT,
                        title VARCHAR(255) NOT NULL,
                        PRIMARY KEY (id)
                    );
                  ";
        int numberOfEffectedRows = connection.Execute(sql);
        return numberOfEffectedRows;
    }
    
    [Test]
    public void CreateDatabaseTableWatchlistTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        int numberOfEffectedRows = sut.CreateDatabaseTableWatchlistAndWatchListCategory();
        
        // Assert
        numberOfEffectedRows.Should().Be(0);
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        int checkTableExists1 = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM watchlist");
        checkTableExists1.Should().Be(0);
        int checkTableExists2 = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM watchlist_category");
        checkTableExists2.Should().Be(0);

    }
    
    // Now that the tables watchlist and watchlist_category are created, we can insert rows into the tables
    // We use the Dapper method Execute() to insert a row into the table
    // We can parameterize the SQL query with @Title (SQL parameter placeholder)
    // Often it's necessary to return the id of the inserted row, it's a good idea to return the id when we insert a row.
    // Every database has a different way to return the id of the inserted row.
    // In MySQL we can use the function LAST_INSERT_ID() to return the id of the inserted row.
    // We can use the Dapper method ExecuteScalar<T>() to return the id of the inserted row.
    // So we need to insert a row into the table watchlist_category, then return the id of the inserted row.
    // Then we can insert a row into the table watchlist and use the id of the inserted row from the table watchlist_category.
    //
    // A query looks like this, actually we have two queries, but we can execute them in one go with the ExecuteScalar<T>() method.
    // INSERT INTO table (column0, column1, ...) VALUES (@column0, @column1, ...);
    // SELECT LAST_INSERT_ID();
    //
    // This method returns the id of the inserted row in the table watchlist.
    public int InsertIntoWatchList(string categoryTitle, string movieTitle)
    {
        string insertWatchlistCategory = @"
                    INSERT INTO watchlist_category (title) VALUES (@Title);
                    SELECT LAST_INSERT_ID();
                  ";
        
        string insertIntoWatchlist = @"
                    INSERT INTO watchlist (title, watchlist_category_id) VALUES (@Title, @WatchlistCategoryId);
                    SELECT LAST_INSERT_ID();
                  ";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        int watchlistCategoryId = connection.
            ExecuteScalar<int>(insertWatchlistCategory, param: new {Title = categoryTitle});
        
        int idOfInsertedRow = 
            connection.ExecuteScalar<int>(insertIntoWatchlist, 
                param: new {Title = movieTitle, WatchlistCategoryId = watchlistCategoryId});
        return idOfInsertedRow;
    }
    
    [Test]
    public void InsertIntoWatchListTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        CreateDatabaseTableWatchlistAndWatchListCategory();
        
        // Act
        int idOfInsertedRow = sut.InsertIntoWatchList("Action List", "The Matrix");
        
        // Assert
        idOfInsertedRow.Should().Be(1);
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql =
            """
                SELECT w.title as MovieTitle, wc.title as CategoryTitle
                FROM watchlist w
                    JOIN watchlist_category wc ON w.watchlist_category_id = wc.id
            """;
        
        // I'm not a big fan of tuples, but it's a good way to return multiple values from a query without creating a new class
        List<(string, string)> movies = connection.Query<(string, string)>(sql).ToList();
        movies.First().Item1.Should().Be("The Matrix");
        movies.First().Item2.Should().Be("Action List");
    }
    
    // When we update a row it's a good idea to use the primary key (id) to identify the row.
    // When we update a row it's a good idea to return the number of affected rows (this should be 1).
    public int UpdateWatchListCategoryTitle(int watchlistCategoryId, string newTitle)
    {
        string updateWatchlistCategory = @"
                    UPDATE watchlist_category SET title = @NewTitle WHERE id = @WatchlistCategoryId;
                  ";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        int numberOfEffectedRows = connection.Execute(updateWatchlistCategory, param: 
            new {WatchlistCategoryId = watchlistCategoryId, NewTitle = newTitle});

        return numberOfEffectedRows;
    }
    
    [Test]
    public void UpdateWatchListCategoryTitleTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        CreateDatabaseTableWatchlistAndWatchListCategory();
        
        // Act
        int idOfInsertedRow = sut.InsertIntoWatchList("Action List", "The Matrix");
        int numberOfEffectedRows = sut.UpdateWatchListCategoryTitle(idOfInsertedRow, "Action List 2");
        
        // Assert
        numberOfEffectedRows.Should().Be(1);
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql =
            """
                SELECT w.title as MovieTitle, wc.title as CategoryTitle
                FROM watchlist w
                    JOIN watchlist_category wc ON w.watchlist_category_id = wc.id
            """;
        
        // I'm not a big fan of tuples, but it's a good way to return multiple values from a query without creating a new class
        List<(string, string)> movies = connection.Query<(string, string)>(sql).ToList();
        movies.First().Item1.Should().Be("The Matrix");
        movies.First().Item2.Should().Be("Action List 2");
    }
    
    // When we delete a row it's a good idea to use the primary key (id) to identify the row.
    // !!!!Make sure you always have a where clause in your delete and update statement!!!!
    // Do you understand why?
    public void DeleteWatchListCategory(int watchlistCategoryId)
    {
        string deleteWatchlistCategory = @"
                    DELETE FROM watchlist_category WHERE id = @WatchlistCategoryId;
                  ";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        connection.Execute(deleteWatchlistCategory, param: new {WatchlistCategoryId = watchlistCategoryId});
    }
    
    [Test]
    public void DeleteWatchListCategoryTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        CreateDatabaseTableWatchlistAndWatchListCategory();
        
        // Act
        int idOfInsertedRow = sut.InsertIntoWatchList("Action List", "The Matrix");
        sut.DeleteWatchListCategory(idOfInsertedRow);
        
        // Assert
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql =
            """
                SELECT w.title as MovieTitle, wc.title as CategoryTitle
                FROM watchlist w
                    JOIN watchlist_category wc ON w.watchlist_category_id = wc.id
            """;
        
        // I'm not a big fan of tuples, but it's a good way to return multiple values from a query without creating a new class
        List<(string, string)> movies = connection.Query<(string, string)>(sql).ToList();
        movies.Should().BeEmpty();
    }
    
    public List<FilmListSlower> GetFilmListSlowerWithCategoryParameterAndRating
                                    (string category, string rating)
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql = """
                       SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price,
                              length as Length, rating as Rating, actors as Actors
                       FROM nicer_but_slower_film_list
                       WHERE category = @Category
                           AND rating = @Rating
                     """;
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(sql, param: new {Category = category, Rating = rating});
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerWithCategoryParameterAndRatingTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> actionFilmsPg13 = sut.GetFilmListSlowerWithCategoryParameterAndRating("Action", "PG-13");
        
        // Assert
        Assert.That(actionFilmsPg13, Is.Not.Null);
    }

    public List<FilmListSlower> GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter
        (string? category = null, string? rating = null)
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        string sql = """
                       SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price,
                              length as Length, rating as Rating, actors as Actors
                       FROM nicer_but_slower_film_list
                       WHERE
                           (@Category IS NULL OR category = @Category) -- trick with IS NULL and OR to make the parameter optional! Make sure to use parentheses!
                           AND
                           (@Rating IS NULL OR rating = @Rating)
                     """;
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(sql, param: new {Category = category, Rating = rating});
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameterTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act -- with one parameter (rating)
        List<FilmListSlower> pg13Films = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter(rating: "PG-13");
        
        // Assert
        pg13Films.Should().HaveCount(223);
        
        // Act -- with one parameter (category)
        List<FilmListSlower> actionFilms = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter(category: "Action");
        
        // Assert
        actionFilms.Should().HaveCount(64);
        
        
        // Act -- no parameter
        List<FilmListSlower> allFilms = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter();
        
        // Assert
        allFilms.Should().HaveCount(1000);
        
        // Act -- with two parameters
        List<FilmListSlower> actionFilmsPg13 = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter("Action", "PG-13");
        allFilms.Should().HaveCount(11);
    }
    
    //In this example we use the nameof operator to get the name of the property (nameof(FilmListSlower.FilmId))
    //This is a good practice to avoid typos!
    //Also notice the optional parameters with default values (page = 1, pageSize = 10, sortColumn = "length", sortDirection = "ASC")
    //With the optional parameters we can call the method without parameters (GetFilmListSlowerWithPagingAndSorting())
    //or with one parameter (GetFilmListSlowerWithPagingAndSorting(page: 1))
    //or with two parameters (GetFilmListSlowerWithPagingAndSorting(page: 1, pageSize: 20))
    //or with three parameters (GetFilmListSlowerWithPagingAndSorting(page: 1, pageSize: 20, sortColumn: "title"))
    //or with four parameters (GetFilmListSlowerWithPagingAndSorting(page: 1, pageSize: 20, sortColumn: "title", sortDirection: "DESC"))
    //or any combination you like and the oder of the parameters doesn't matter, if you use the parameter names!;
    //Parameter names can always be used to specify which parameter we want to use! One can argue that this is more readable than positional parameters.
    //But most programmers are used to positional parameters, so it's a matter of taste and common practice to use positional parameters instead of named parameters.
    //But it's good to know that named parameters are possible!
    //If you use positional parameters, the order of the parameters is important! An IDE like Rider will show the parameter names / types (sometimes in grey),
    //so you can see the order of the parameters. And make the code more readable.
    public List<FilmListSlower> GetFilmListSlowerWithPagingAndSorting(int page = 1, int pageSize = 10, string sortColumn = "length", string sortDirection = "ASC")
    {
        // take a look as alias for columns (fid as FilmId, title as Title, ...)
        // we can use the nameof operator to get the name of the property (nameof(FilmListSlower.FilmId))
        string sql = $"""
                        SELECT  fid         as {nameof(FilmListSlower.FilmId)},
                                title       as {nameof(FilmListSlower.Title)},
                                description as {nameof(FilmListSlower.Description)},
                                category    as {nameof(FilmListSlower.Category)},
                                price       as {nameof(FilmListSlower.Price)},
                                length      as {nameof(FilmListSlower.Length)},
                                rating      as {nameof(FilmListSlower.Rating)},
                                actors      as {nameof(FilmListSlower.Actors)}
                        FROM nicer_but_slower_film_list
                        ORDER BY @SortColumn @SortDirection
                        LIMIT @PageSize
                        OFFSET @Offset -- @(Page - 1) * @PageSize -- this is not correct for MySQL!
                      """;
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(sql, 
                param: new {
                    Offset = (page - 1) * pageSize, 
                    PageSize = pageSize, 
                    SortColumn = sortColumn, 
                    SortDirection = sortDirection
                }
            );
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerWithPagingAndSortingTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> films = sut.GetFilmListSlowerWithPagingAndSorting();
        
        // Assert
        films.Should().HaveCount(10);
        
        
        // Act
        List<FilmListSlower> filmsPage2 = sut.GetFilmListSlowerWithPagingAndSorting(page: 1, sortDirection: "DESC");
        
        // Assert
        filmsPage2.Should().HaveCount(10);
        
        // Act
        List<FilmListSlower> filmsPage3 = sut.GetFilmListSlowerWithPagingAndSorting(page: 2, sortColumn: "title");
        filmsPage2.Should().HaveCount(10);
    }
    
    public class PageAndSortParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        public int Offset => (Page - 1) * PageSize;
        
        public string SortColumn { get; set; } = "length";
        public string SortDirection { get; set; } = "ASC";
    }
    
    public List<FilmListSlower> GetFilmsWithPagingAndSorting(PageAndSortParameters pageAndSortParameters)
    {

        string sql = @"
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                    ORDER BY @SortColumn @SortDirection
                    LIMIT @PageSize
                    OFFSET @Offset
                  ";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(sql, 
            param: pageAndSortParameters);
        return films.ToList();
    }
    
    [Test]
    public void GetFilmsWithPagingAndSortingTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> films = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters());
        
        // Assert
        films.Should().HaveCount(10);
        
        
        // Act
        List<FilmListSlower> filmsPage2 = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 1, SortDirection = "DESC"});
        
        // Assert
        filmsPage2.Should().HaveCount(10);
        
        // Act
        List<FilmListSlower> filmsPage3 = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 2, SortColumn = "title"});
        filmsPage2.Should().HaveCount(10);
    }


    [Test]
    public void GetFilmsWithPagingAndSortingWithParametersAndObjectShouldBeTheSame()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> films = sut.GetFilmListSlowerWithPagingAndSorting();
        List<FilmListSlower> filmsParametersObject = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters());

        films.Should().BeEquivalentTo(filmsParametersObject);
        
        // Assert
        films.Should().HaveCount(10);
        
        
        // Act
        List<FilmListSlower> filmsPage2 = sut.GetFilmListSlowerWithPagingAndSorting(page: 1, sortDirection: "DESC");
        List<FilmListSlower> filmsPage2ParametersObject = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 1, SortDirection = "DESC"});
        
        filmsPage2.Should().BeEquivalentTo(filmsPage2ParametersObject);
        
        // Assert
        filmsPage2.Should().HaveCount(10);
        
        // Act
        List<FilmListSlower> filmsPage3 = sut.GetFilmListSlowerWithPagingAndSorting(page: 2, sortColumn: "title");
        List<FilmListSlower> filmsPage3ParametersObject = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 2, SortColumn = "title"});
        
        filmsPage3.Should().BeEquivalentTo(filmsPage3ParametersObject);
        
        filmsPage2.Should().HaveCount(10);
    }

    public List<FilmListSlower> FilmsWithSqlBuilder()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        SqlBuilder sqlBuilder = new SqlBuilder();
        sqlBuilder.Select("title as Title")
            .Select("category as Category")
            .Where("length > @Length", new { Length = 120 })
            .Where("rating = @Rating", new { Rating = "PG-13" })
            .OrderBy("title DESC")
            .OrderBy("length DESC")
            .AddParameters(new { PageSize = 10, Offset = 0 });
            
        
        SqlBuilder.Template? template = sqlBuilder.AddTemplate("SELECT /**select**/ FROM nicer_but_slower_film_list /**where**/ /**orderby**/ LIMIT @PageSize OFFSET @Offset");
        
        
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(template.RawSql, template.Parameters);
        return films.ToList();
    } 
    
    [Test]
    public void FilmsWithSqlBuilderTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        // Act
        List<FilmListSlower> films = sut.FilmsWithSqlBuilder();
        
        // Assert
        films.Should().HaveCount(10);
    }
    
    public class QueryParameters
    {
        public int PageSize { get; set; } = 10;
        public int Offset { get; set; } = 0;
        
        public List<string> SelectColumns { get; set; } = new List<string>();
        
        public List<(string, object)> WhereColumns { get; set; } = new List<(string, object)>();
        
        public List<string> OrderByColumns { get; set; } = new List<string>();
    }
    
    public List<FilmListSlower> GetFilmsWithSqlBuilderAndParameters(QueryParameters queryParameters)
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        SqlBuilder sqlBuilder = new SqlBuilder();
        
        foreach (string selectColumn in queryParameters.SelectColumns)
        {
            sqlBuilder.Select(selectColumn);
        }

        foreach ((string, object) whereColumn in queryParameters.WhereColumns)
        {
            sqlBuilder.Where(whereColumn.Item1, whereColumn.Item2);
        }
        
        foreach (string orderByColumn in queryParameters.OrderByColumns)
        {
            sqlBuilder.OrderBy(orderByColumn);
        }
        
        sqlBuilder.AddParameters(new { PageSize = queryParameters.PageSize, Offset = queryParameters.Offset });
        
        SqlBuilder.Template? template = sqlBuilder.AddTemplate("SELECT /**select**/ FROM nicer_but_slower_film_list /**where**/ /**orderby**/ LIMIT @PageSize OFFSET @Offset");
        
        IEnumerable<FilmListSlower> films = connection.Query<FilmListSlower>(template.RawSql, template.Parameters);
        return films.ToList();
    }
    
    [Test]
    public void GetFilmsWithSqlBuilderAndParametersTest()
    {
        // Arrange
        Examples2 sut = new Examples2();
        
        
        QueryParameters queryParameters = new QueryParameters()
        {
            SelectColumns = new List<string>() {"title as Title", "category as Category"},
            WhereColumns = new List<(string, object)>() {("length > @Length", new { Length = 120 }), ("rating = @Rating", new { Rating = "PG-13" })},
            OrderByColumns = new List<string>() {"title DESC", "length DESC"},
            PageSize = 10,
            Offset = 0
        };
        
        // Act
        List<FilmListSlower> films = sut.GetFilmsWithSqlBuilderAndParameters(queryParameters);
        
        // Assert
        films.Should().HaveCount(10);
    }
    
    
}
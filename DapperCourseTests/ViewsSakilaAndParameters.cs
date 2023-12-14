using Dapper;
using FluentAssertions;
using MySql.Data.MySqlClient;

namespace DapperCourseTests;

public class ViewsSakilaAndParameters
{
    private static string GetConnectionString()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
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
        using var connection = new MySqlConnection(GetConnectionString());
        var sql = $@"
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
        
        var films = connection.Query<FilmListSlower>(sql);
        return films.ToList();
    }
    
    [Test]
    public void SqlInjectionExampleTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var allCategoriesMovies = sut.SqlInjectionExample("Action' OR 1 = 1 -- ");
        
        // Assert
        Assert.Pass();
    }
    

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
        using var connection = new MySqlConnection(GetConnectionString());
        var sql = """
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                  """;
        var films = connection.Query<FilmListSlower>(sql);
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var films = sut.GetFilmListSlower();

        // Assert
        Assert.That(films, Is.Not.Null);
    }
    
    public List<FilmListSlower> GetFilmListSlowerWithCategoryParameter(string category)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var sql = """
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                    WHERE category = @Category
                  """;
        var films = connection.Query<FilmListSlower>(sql, param: new {Category = category});
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerWithCategoryParameterTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var actionFilms = sut.GetFilmListSlowerWithCategoryParameter("Action");
        
        // Assert
        Assert.That(actionFilms, Is.Not.Null);
    }
    
    public List<FilmListSlower> GetFilmListSlowerWithCategoryParameterAndRating(string category, string rating)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var sql = """
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                    WHERE category = @Category 
                        AND rating = @Rating
                  """;
        var films = connection.Query<FilmListSlower>(sql, param: new {Category = category, Rating = rating});
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerWithCategoryParameterAndRatingTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var actionFilmsPg13 = sut.GetFilmListSlowerWithCategoryParameterAndRating("Action", "PG-13");
        
        // Assert
        Assert.That(actionFilmsPg13, Is.Not.Null);
    }

    public List<FilmListSlower> GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter(string category = null,
        string rating = null)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var sql = """
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                    WHERE 
                        (@Category IS NULL OR category = @Category) -- trick with IS NULL and OR to make the parameter optional! Make sure to use parentheses! 
                        AND 
                        (@Rating IS NULL OR rating = @Rating) 
                  """;
        var films = connection.Query<FilmListSlower>(sql, param: new {Category = category, Rating = rating});
        return films.ToList();
    }
    
    [Test]
    public void GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameterTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act -- with one parameter (rating)
        var pg13Films = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter(rating: "PG-13");
        
        // Assert
        pg13Films.Should().HaveCount(223);
        
        // Act -- with one parameter (category)
        var actionFilms = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter(category: "Action");
        
        // Assert
        actionFilms.Should().HaveCount(64);
        
        
        // Act -- no parameter
        var allFilms = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter();
        
        // Assert
        allFilms.Should().HaveCount(1000);
        
        // Act -- with two parameters
        var actionFilmsPg13 = sut.GetFilmListSlowerWithCategoryParameterAndRatingOptionalParameter("Action", "PG-13");
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
        var sql = $"""
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
        
        using var connection = new MySqlConnection(GetConnectionString());
        var films = connection.Query<FilmListSlower>(sql, 
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
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var films = sut.GetFilmListSlowerWithPagingAndSorting();
        
        // Assert
        films.Should().HaveCount(10);
        
        
        // Act
        var filmsPage2 = sut.GetFilmListSlowerWithPagingAndSorting(page: 1, sortDirection: "DESC");
        
        // Assert
        filmsPage2.Should().HaveCount(10);
        
        // Act
        var filmsPage3 = sut.GetFilmListSlowerWithPagingAndSorting(page: 2, sortColumn: "title");
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

        var sql = @"
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                    ORDER BY @SortColumn @SortDirection
                    LIMIT @PageSize
                    OFFSET @Offset
                  ";
        
        using var connection = new MySqlConnection(GetConnectionString());
        var films = connection.Query<FilmListSlower>(sql, 
            param: pageAndSortParameters);
        return films.ToList();
    }
    
    [Test]
    public void GetFilmsWithPagingAndSortingTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var films = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters());
        
        // Assert
        films.Should().HaveCount(10);
        
        
        // Act
        var filmsPage2 = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 1, SortDirection = "DESC"});
        
        // Assert
        filmsPage2.Should().HaveCount(10);
        
        // Act
        var filmsPage3 = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 2, SortColumn = "title"});
        filmsPage2.Should().HaveCount(10);
    }


    [Test]
    public void GetFilmsWithPagingAndSortingWithParametersAndObjectShouldBeTheSame()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var films = sut.GetFilmListSlowerWithPagingAndSorting();
        var filmsParametersObject = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters());

        films.Should().BeEquivalentTo(filmsParametersObject);
        
        // Assert
        films.Should().HaveCount(10);
        
        
        // Act
        var filmsPage2 = sut.GetFilmListSlowerWithPagingAndSorting(page: 1, sortDirection: "DESC");
        var filmsPage2ParametersObject = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 1, SortDirection = "DESC"});
        
        filmsPage2.Should().BeEquivalentTo(filmsPage2ParametersObject);
        
        // Assert
        filmsPage2.Should().HaveCount(10);
        
        // Act
        var filmsPage3 = sut.GetFilmListSlowerWithPagingAndSorting(page: 2, sortColumn: "title");
        var filmsPage3ParametersObject = sut.GetFilmsWithPagingAndSorting(new PageAndSortParameters() {Page = 2, SortColumn = "title"});
        
        filmsPage3.Should().BeEquivalentTo(filmsPage3ParametersObject);
        
        filmsPage2.Should().HaveCount(10);
    }

    public List<FilmListSlower> FilmsWithSqlBuilder()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var sqlBuilder = new SqlBuilder();
        sqlBuilder.Select("title as Title")
            .Select("category as Category")
            .Where("length > @Length", new { Length = 120 })
            .Where("rating = @Rating", new { Rating = "PG-13" })
            .OrderBy("title DESC")
            .OrderBy("length DESC")
            .AddParameters(new { PageSize = 10, Offset = 0 });
            
        
        var template = sqlBuilder.AddTemplate("SELECT /**select**/ FROM nicer_but_slower_film_list /**where**/ /**orderby**/ LIMIT @PageSize OFFSET @Offset");
        
        
        var films = connection.Query<FilmListSlower>(template.RawSql, template.Parameters);
        return films.ToList();
    } 
    
    [Test]
    public void FilmsWithSqlBuilderTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        // Act
        var films = sut.FilmsWithSqlBuilder();
        
        // Assert
        films.Should().HaveCount(10);
    }
    
    public class QueryParameters
    {
        public int Length { get; set; } = 120;
        public string Rating { get; set; } = "PG-13";
        public int PageSize { get; set; } = 10;
        public int Offset { get; set; } = 0;
        
        public List<string> SelectColumns { get; set; } = new List<string>();
        
        public List<(string, object)> WhereColumns { get; set; } = new List<(string, object)>();
        
        public List<string> OrderByColumns { get; set; } = new List<string>();
    }
    
    public List<FilmListSlower> GetFilmsWithSqlBuilderAndParameters(QueryParameters queryParameters)
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var sqlBuilder = new SqlBuilder();
        
        foreach (var selectColumn in queryParameters.SelectColumns)
        {
            sqlBuilder.Select(selectColumn);
        }

        foreach (var whereColumn in queryParameters.WhereColumns)
        {
            sqlBuilder.Where(whereColumn.Item1, whereColumn.Item2);
        }
        
        foreach (var orderByColumn in queryParameters.OrderByColumns)
        {
            sqlBuilder.OrderBy(orderByColumn);
        }
        
        sqlBuilder.AddParameters(new { PageSize = queryParameters.PageSize, Offset = queryParameters.Offset });
            
        
        var template = sqlBuilder.AddTemplate("SELECT /**select**/ FROM nicer_but_slower_film_list /**where**/ /**orderby**/ LIMIT @PageSize OFFSET @Offset");
        
        
        var films = connection.Query<FilmListSlower>(template.RawSql, template.Parameters);
        return films.ToList();
    }
    
    [Test]
    public void GetFilmsWithSqlBuilderAndParametersTest()
    {
        // Arrange
        var sut = new ViewsSakilaAndParameters();
        
        
        var queryParameters = new QueryParameters()
        {
            SelectColumns = new List<string>() {"title as Title", "category as Category"},
            WhereColumns = new List<(string, object)>() {("length > @Length", new { Length = 120 }), ("rating = @Rating", new { Rating = "PG-13" })},
            OrderByColumns = new List<string>() {"title DESC", "length DESC"},
            PageSize = 10,
            Offset = 0
        };
        
        // Act
        var films = sut.GetFilmsWithSqlBuilderAndParameters(queryParameters);
        
        // Assert
        films.Should().HaveCount(10);
    }
    
    
}
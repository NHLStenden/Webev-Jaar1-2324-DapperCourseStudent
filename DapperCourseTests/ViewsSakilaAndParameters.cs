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
    
    public List<FilmListSlower> GetFilmListSlowerWithPagingAndSorting(int page = 1, int pageSize = 10, string sortColumn = "length", string sortDirection = "ASC")
    {
        var sql = @"
                    SELECT fid as FilmId, title as Title, description as Description, category as Category, price as Price, 
                           length as Length, rating as Rating, actors as Actors
                    FROM nicer_but_slower_film_list
                    ORDER BY @SortColumn @SortDirection
                    LIMIT @PageSize
                    OFFSET @Offset -- @(Page - 1) * @PageSize -- this is not working in MySql
                  ";
        
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
    
    
    
}
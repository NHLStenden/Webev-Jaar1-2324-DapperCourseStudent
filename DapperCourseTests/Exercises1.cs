using DapperCourse;
using FluentAssertions;

namespace DapperCourseTests;


public class Exercises1
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public Task Exercise1()
    {
        // Arrange
        var simpleQueries = new SimpleQueries();
        
        // Act
        var movies = simpleQueries.Exercise1();
        
        // Assert
        movies.Should().HaveCount(10);

        return Verify(movies);
    }
    
    [Test]
    public Task Exercise1_1()
    {
        // Arrange
        var simpleQueries = new SimpleQueries();
        
        // Act
        IEnumerable<SimpleQueries.ResultExercise1BAndC> movies = simpleQueries.Exercise1B();
        
        // Assert
        movies.Should().HaveCount(10);

        return Verify(movies);
    }

    [Test]
    public void Exercise2()
    {
        // Arrange
        var simpleQueries = new SimpleQueries();
        
        // Act
        // not a good to use an object? here, but it's what Dapper returns in this exercise
        object? yearOfMovie = simpleQueries.Exercise2();
        
        // Assert
        yearOfMovie.Should().Be(1999);
    }
    
    [Test]
    public void Exercise2_1()
    {
        // Arrange
        var simpleQueries = new SimpleQueries();
        
        // Act
        // not a good to use an object? here, but it's what Dapper returns in this exercise
        int yearOfMovie = simpleQueries.Exercise2_1();
        
        // Assert
        yearOfMovie.Should().Be(1999);
    }

    [Test]
    public void Exercise3()
    {
        // Arrange
        var simpleQueries = new SimpleQueries();
        
        // Act
        List<int> yearsOfJapaneseMovies = simpleQueries.Exercise3();
        
        // Assert
        yearsOfJapaneseMovies.Should().HaveCount(3);
        yearsOfJapaneseMovies.Should().ContainInOrder(1954, 1997, 2001);
    }
    
    
    //relationships
    [Test]
    public Task GetMovieWithDirectorByMultipleResultSet()
    {
        // Arrange
        var simpleQueries = new SimpleQueries();
        
        // Act
        var movie = simpleQueries.GetMovieWithDirectorByMultipleResultSet(922);
        
        // Assert
        movie.Should().NotBeNull();

        return Verify(movie);

    }
}
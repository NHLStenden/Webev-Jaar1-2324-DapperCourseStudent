using DapperCourse;

namespace DapperCourseTests;

public class RelationshipsTests
{
    [Test]
    public void AuthorsWithBooks()
    {
        // Arrange
        var sut = new RelationshipsBook();

        // Act
        var result = sut.GetAuthorsWithBooks();
        
        // Assert
        Assert.AreEqual(3, result.Count());
    }
    
    [Test]
    public void BookWithAuthor()
    {
        // Arrange
        var sut = new RelationshipsBook();

        // Act
        var result = sut.GetBookWithAuthor();
        
        // Assert
        Assert.AreEqual(3, result.Count());
    }

    [Test]
    public void SmartMapper()
    {
        // Arrange
        var sut = new RelationshipsBook();
        
        // Act
        var result = sut.SmartMapper();
        
        // Assert
        Assert.AreEqual(3, result.Count());
    }

}
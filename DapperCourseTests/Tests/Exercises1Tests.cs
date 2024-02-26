using FluentAssertions;

namespace DapperCourseTests.Tests;

public class Exercises1Tests
{
    [Test]
    public void Exercise0Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        bool result = sut.Exercise0();
        
        // Assert
        result.Should().BeTrue();
        // Assert.AreEqual(true, result); 
    }
}
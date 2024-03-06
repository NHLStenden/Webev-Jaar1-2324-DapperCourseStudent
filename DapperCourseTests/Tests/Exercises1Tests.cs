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
    [Test]
    public void ExerciseScalar1Test()
    {
        // Arrange
        Exercises1 sut = new Exercises1();
        
        // Act
        int count = sut.ExerciseScalar1();
        
        // Assert
        count.Should().Be(28);
    }
}
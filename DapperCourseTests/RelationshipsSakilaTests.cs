using DapperCourse;

namespace DapperCourseTests;

[TestFixture]
public class RelationshipsSakilaTests
{
    
    [Test]
    public void GetActorsTest()
    {
        // Arrange
        var sut = new RelationshipsSakila();
        
        // Act
        var actors = sut.GetActors();

        // Assert
        Assert.That(actors, Is.Not.Null);
    }
    
    [Test]
    public void GetCustomerWithAddressTest()
    {
        // Arrange
        var sut = new RelationshipsSakila();
        
        // Act
        var customers = sut.GetCustomerIncludeAddress();

        // Assert
        Assert.That(customers, Is.Not.Null);
    }

    [Test]
    public void GetCustomerWithAddressWithCityTest()
    {
        // Arrange
        var sut = new RelationshipsSakila();
        
        // Act
        var customers = sut.GetCustomerIncludeAddressIncludeCity();
        
        // Assert
        Assert.That(customers, Is.Not.Null);
    }

    [Test]
    public void GetCustomerIncludeAddressIncludeCityIncludeCountry()
    {
        // Arrange
        var sut = new RelationshipsSakila();
        
        // Act
        var customers = sut.GetCustomerIncludeAddressIncludeCityIncludeCountry();
        
        // Assert
        Assert.That(customers, Is.Not.Null);
    }


}
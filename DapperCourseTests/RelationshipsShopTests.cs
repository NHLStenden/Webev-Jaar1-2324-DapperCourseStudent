using DapperCourse;

namespace DapperCourseTests;

public class RelationshipsShopTests
{
    [Test]
    public void GetProductWithCategoryTest()
    {
        // Arrange
        var sut = new RelationshipsShop();
        
        // Act
        var productWithCategory = sut.GetProductWithCategory();

        // Assert
        Assert.That(productWithCategory, Is.Not.Null);
    }
    
    [Test]
    public void GetCategoryWithProductsTest()
    {
        // Arrange
        var sut = new RelationshipsShop();
        
        // Act
        var categoryWithProducts = sut.CategoryWithProducts();



        // Assert
        Assert.That(categoryWithProducts, Is.Not.Null);
    }
}
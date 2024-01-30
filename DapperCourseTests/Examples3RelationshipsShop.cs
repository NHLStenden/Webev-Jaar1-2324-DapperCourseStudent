using Dapper;
using MySqlConnector;

namespace DapperCourseTests;

public class Examples3RelationshipsShop
{
    private static readonly string ConnectionString;
    static Examples3RelationshipsShop()
    {
        ConnectionString = ConnectionStrings.GetConnectionStringShop();
    }
    
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }

    public List<Product> GetProductWithCategory()
    {
        using MySqlConnection connection = new MySqlConnection(ConnectionString);

        string sql = @"SELECT ProductID, ProductName, p.CategoryID, CategoryName
                FROM Products p 
                INNER JOIN Categories c ON p.CategoryID = c.CategoryID";

        IEnumerable<Product> products = connection.Query<Product, Category, Product>(sql, (product, category) =>
            {
                product.Category = category;
                return product;
            },
            splitOn: "CategoryId");

        products.ToList().ForEach(product =>
            Console.WriteLine($"Product: {product.ProductName}, Category: {product.Category.CategoryName}"));

        return products.ToList();
    }
    
    [Test]
    public async Task TestGetProductWithCategory()
    {
        List<Product> products = GetProductWithCategory();
        await Verify(products);
    }

    public List<Category> CategoryWithProducts()
    {
        string sql = @"SELECT c.CategoryID, CategoryName, p.ProductID, ProductName
                FROM Categories c 
                INNER JOIN Products p ON p.CategoryID = c.CategoryID";
        
        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<Category> categories = connection.Query<Category, Product, Category>(sql, (category, product) =>
            {
                category.Products ??= new List<Product>();
                category.Products.Add(product);
                return category;
            },
            splitOn: "ProductId");
        return categories.ToList();
    }
    
    [Test]
    public async Task TestCategoryWithProducts()
    {
        List<Category> categories = CategoryWithProducts();
        await Verify(categories);
    }
    
    
}
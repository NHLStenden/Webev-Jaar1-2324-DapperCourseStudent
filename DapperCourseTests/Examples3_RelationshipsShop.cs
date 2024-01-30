using Dapper;
using MySqlConnector;

namespace DapperCourse;

public class Examples3RelationshipsShop
{
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

    public string GetConnectionStringForShop()
    {
        return "Server=localhost;port=3306;Database=Shop;Uid=root;Pwd=Test@1234!";
    }

    public List<Product> GetProductWithCategory()
    {
        using var connection = new MySqlConnection(GetConnectionStringForShop());

        var sql = @"SELECT ProductID, ProductName, p.CategoryID, CategoryName
                FROM Products p 
                INNER JOIN Categories c ON p.CategoryID = c.CategoryID";

        var products = connection.Query<Product, Category, Product>(sql, (product, category) =>
            {
                product.Category = category;
                return product;
            },
            splitOn: "CategoryId");

        products.ToList().ForEach(product =>
            Console.WriteLine($"Product: {product.ProductName}, Category: {product.Category.CategoryName}"));

        return products.ToList();
    }

    public List<Category> CategoryWithProducts()
    {
        var sql = @"SELECT c.CategoryID, CategoryName, p.ProductID, ProductName
                FROM Categories c 
                INNER JOIN Products p ON p.CategoryID = c.CategoryID";
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var categories = connection.Query<Category, Product, Category>(sql, (category, product) =>
            {
                category.Products ??= new List<Product>();
                category.Products.Add(product);
                return category;
            },
            splitOn: "ProductId");
        return categories.ToList();
    }
    
    
}
namespace DapperCourseTests;

public static class ConnectionStrings
{
    public static string GetConnectionStringSakila()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    
    public static string GetConnectionStringMovies()
    {
        return "server=localhost;port=3306;database=Movies;user=root;password=Test@1234!";
    }
    
    public static string GetConnectionStringShop()
    {
        return "server=localhost;port=3306;database=Shop;user=root;password=Test@1234!";
    }

    public static string GetConnectionStringBooks()
    {
        return "server=localhost;port=3306;database=Books;user=root;password=Test@1234!";
    }
}
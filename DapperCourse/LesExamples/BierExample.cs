using Dapper;
using MySqlConnector;


namespace DapperCourse.LesExamples;

public class BierExample
{
    private static string GetConnectionString()
    {
        return "server=localhost;port=3306;database=bieren;user=root;password=Test@1234!";
    }
    
    public class BrouwerMetAantalBieren
    {
        public int Brouwcode { get; set; }
        public string Naam { get; set; } = null!;
        public int Aantal { get; set; }
    }
    
    public static List<BrouwerMetAantalBieren> GetBrouwersMetAantalBieren()
    {
        var sql = """
                    SELECT brouwcode, naam, aantal
                    FROM BrouwerMetAantalBieren
                    WHERE aantal > 10
                  """;
        using var connection = new MySqlConnection(GetConnectionString());
        return connection.Query<BrouwerMetAantalBieren>(sql).ToList();
    }

    
}
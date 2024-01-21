using Dapper;
using MySqlConnector;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DapperCourseTests;

public class Examples3_NtoNRelationships
{    
    private static string GetConnectionString()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    




    public class Film
    {
        public int FilmId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ReleaseYear { get; set; }
        public int LanguageId { get; set; }
        public int? OriginalLanguageId { get; set; }
        public int RentalDuration { get; set; }
        public decimal RentalRate { get; set; }
        public int Length { get; set; }
        public decimal ReplacementCost { get; set; }
        public string Rating { get; set; } = null!;
        public string SpecialFeatures { get; set; } = null!;
        public DateTime LastUpdate { get; set; }

       
    }


    public class Actor
    {
        public int ActorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
        
        public List<Film> Films { get; set; } = null!;
    }
    
    //In this example we will get The Actors with their Films
    //We use JSON_ARRAYAGG to get the Films as a JSON array
    //We have no control over the order of the JSON array
    //Maybe this a an solution, but it's out of this scope for this course:
    //https://stackoverflow.com/questions/60572309/how-to-order-by-a-json-object-in-mysql
    public List<Actor> GetActorsIncludeFilms()
    {
        string sql =
            """
            SELECT JSON_OBJECT(
                           'ActorId', a.actor_id,
                           'FirstName', a.first_name,
                           'LastName', a.last_name,
                           'Films', JSON_ARRAYAGG(
                                   JSON_OBJECT(
                                           'FilmId', f.film_id,
                                           'Title', f.title,
                                           'Description', f.description,
                                           'ReleaseYear', f.release_year,
                                           'Length', f.length,
                                           'Rating', f.rating,
                                           'SpecialFeatures', f.special_features
                                   )
                            )
                   )
            FROM 
                actor a 
                    JOIN film_actor fa ON a.actor_id = fa.actor_id
                        JOIN film f ON fa.film_id = f.film_id
            GROUP BY a.actor_id
            ORDER BY a.actor_id
            LIMIT 3
            """;
        
        using var connection = new MySqlConnection(GetConnectionString());
        var actorsAsJson = connection.Query<string>(sql);
        var actors = new List<Actor>();
        foreach (var actorAsJson in actorsAsJson)
        {
            var actor = JsonSerializer.Deserialize<Actor>(actorAsJson);
            actors.Add(actor);
        }
        return actors.ToList();
    }
    
    [Test]
    public async Task GetActorsIncludeFilmsTest()
    {
        // Arrange
        var sut = new Examples3_NtoNRelationships();
        
        // Act
        var films = sut.GetActorsIncludeFilms();
        
        // Assert
        await Verify(films);
    }
}
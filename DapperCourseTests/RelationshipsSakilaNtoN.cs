using Argon;
using Dapper;
using MySql.Data.MySqlClient;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DapperCourseTests;

public class RelationshipsSakilaNtoN
{    
    private static string GetConnectionString()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    
    private string sql =
        """
        SELECT m.film_id as FilmId, m.title, m.description as Description, 
               m.release_year as ReleaseYear, language_id as LanguageId,
                m.original_language_id as OriginalLanguageId, m.rental_duration as RentalDuration,
                m.rental_rate as RentalRate, m.length as Length, m.replacement_cost as ReplacementCost,
                m.rating, m.special_features as SpecialFeatures, m.last_update as LastUpdate,
               JSON_ARRAYAGG(JSON_OBJECT('ActorId', a.actor_id, 'FirstName', a.last_name, 'LastName', a.first_name, 'LastUpdate', DATE_FORMAT(a.last_update, '%Y-%m-%dT%TZ'))) AS actors
        FROM 
            film m JOIN 
                film_actor ma ON m.film_id = ma.film_id JOIN 
                    actor a ON ma.actor_id = a.actor_id
        GROUP BY m.film_id
        """;



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

        public List<Actor> Actors { get; set; } = null!;
    }


    public class Actor
    {
        public int ActorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
    
    public List<Film> GetFilmsWithActors()
    {
        using var connection = new MySqlConnection(GetConnectionString());
        var films = connection.Query<Film, string, Film>(sql,
            (film, actor) =>
            {
                film.Actors = film.Actors ?? new List<Actor>();
                var actors = JsonSerializer.Deserialize<List<Actor>>(actor);
                film.Actors.AddRange(actors);
                return film;
            }, splitOn: "actors");
        return films.ToList();
    }
    
    [Test]
    public void GetFilmsWithActorsTest()
    {
        // Arrange
        var sut = new RelationshipsSakilaNtoN();
        
        // Act
        var films = sut.GetFilmsWithActors();
        
        // Assert
        Assert.That(films, Is.Not.Null);
    }
}
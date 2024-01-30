using Dapper;
using FluentAssertions;
using MySqlConnector;

namespace DapperCourseTests;

public class Examples3RelationshipsMovies
{
    private static readonly string ConnectionString;

    static Examples3RelationshipsMovies()
    {
        ConnectionString = ConnectionStrings.GetConnectionStringMovies();
    }

    public class MovieAndDirector
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public int Duration { get; set; }
        public string Language { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public string ReleaseCountryCode { get; set; } = null!;

        public List<Director> Directors { get; set; } = new();
    }

    public class Director
    {
        public int DirectorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    public MovieAndDirector GetMovieWithDirectoryById(int movieId)
    {
        string sql = @"SELECT 
                            m.Title, m.Year, m.Duration, m.Language, m.ReleaseDate, m.ReleaseCountryCode, 
                            'SplitOn' as SplitOn,
                            d.DirectorId, d.FirstName, d.LastName
                        FROM Movies m 
                            JOIN DirectorMovie dm ON m.MovieId = dm.MoviesMovieId 
                                JOIN Directors d ON dm.DirectorsDirectorId = d.DirectorId
                        WHERE m.MovieId = @movieId";

        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        IEnumerable<MovieAndDirector> movies = connection.Query<MovieAndDirector, Director, MovieAndDirector>(sql,
            (movie, director) =>
            {
                movie.Directors.Add(director);
                return movie;
            }, new { movieId }, splitOn: "SplitOn");
        return movies.First();
    }

    [Test]
    public void TestGetMovieWithDirectoryById()
    {
        MovieAndDirector movie = GetMovieWithDirectoryById(922);
        movie.Title.Should().Be("Aliens");
        movie.Year.Should().Be(1986);
        movie.Duration.Should().Be(137);
        movie.Language.Should().Be("English");
        movie.ReleaseDate.Should().Be(new DateTime(1986, 8, 29));
        movie.ReleaseCountryCode.Should().Be("UK");
        movie.Directors.Should().HaveCount(1);
        movie.Directors.First().FirstName.Should().Be("James");
        movie.Directors.First().LastName.Should().Be("Cameron");
    }

    public MovieAndDirector GetMovieWithDirectorByMultipleResultSet(int movieId)
    {
        string sql = @"SELECT m.Title, m.Year, m.Duration, m.Language, m.ReleaseDate, m.ReleaseCountryCode
                        FROM Movies m 
                        WHERE m.MovieId = @movieId;
                        
                        SELECT d.DirectorId, d.FirstName, d.LastName
                        FROM DirectorMovie dm 
                            JOIN Directors d ON dm.DirectorsDirectorId = d.DirectorId
                        WHERE dm.MoviesMovieId = @movieId";

        using MySqlConnection connection = new MySqlConnection(ConnectionString);
        SqlMapper.GridReader gridReader = connection.QueryMultiple(sql, new { movieId });
        MovieAndDirector movie = gridReader.Read<MovieAndDirector>().Single();
        List<Director> directors = gridReader.Read<Director>().ToList();
        movie.Directors.AddRange(directors);
        return movie;
    }
}
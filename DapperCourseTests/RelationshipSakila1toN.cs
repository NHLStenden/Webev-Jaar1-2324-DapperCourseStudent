using Argon;
using Dapper;
using DapperCourse;
using FluentAssertions;
using MySql.Data.MySqlClient;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DapperCourseTests;

public class RelationshipSakila1toN
{
    private static string GetConnectionStringForShop()
    {
        return "Server=localhost;port=3306;Database=sakila;Uid=root;Pwd=Test@1234!";
    }
    
    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
        
        public List<City> Cities { get; set; } = null!;
    }
    
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public int CountryId { get; set; }
        // public Country Country { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
    
    
    
    public List<Country> GetCountriesIncludeCityByGroupingTechnique()
    {
        string sql =
            """
                SELECT c.country_id AS CountryId, c.country AS Name, c.last_update AS LastUpdate,
                       ci.city_id AS CityIdSplit,
                       ci.city_id AS CityId, ci.city AS Name, ci.country_id AS CountryId, ci.last_update AS LastUpdate
                FROM country c
                    JOIN city ci on ci.country_id = c.country_id
                ORDER BY c.country_id
            """;

        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var countries = connection.Query<Country, City, Country>(sql,
            (country, city) =>
            {
                country.Cities = country.Cities ?? new List<City>();
                country.Cities.Add(city);
                return country;
            }, splitOn: "CityIdSplit");

        //grouping by country
        var countriesGrouped = countries.GroupBy(x => x.CountryId).Select(g => new Country()
        {
            CountryId = g.Key,
            Name = g.First().Name,
            LastUpdate = g.First().LastUpdate,
            Cities = g.Select(x => x.Cities.Single()).ToList()
        });
        return countriesGrouped.ToList();
    }
    
    [Test]
    public void GetCountriesIncludeCityByGroupingTechniqueTest()
    {
        // Arrange
        var sut = new RelationshipSakila1toN();
        
        // Act
        var countries = sut.GetCountriesIncludeCityByGroupingTechnique();
        
        // Assert
        countries.Should().HaveCount(109);
    }
    
    public List<Country> GetCountriesIncludeCityByDictionary()
    {
        string sql =
            """
                SELECT c.country_id AS CountryId, c.country AS Name, c.last_update AS LastUpdate,
                       ci.city_id AS CityIdSplit,
                       ci.city_id AS CityId, ci.city AS Name, ci.country_id AS CountryId, ci.last_update AS LastUpdate
                FROM country c
                    JOIN city ci on ci.country_id = c.country_id
                ORDER BY c.country_id
            """;

        var countriesDictionary = new Dictionary<int, Country>();
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var countries = connection.Query<Country, City, Country>(sql,
            (country, city) =>
            {
                if (!countriesDictionary.ContainsKey(country.CountryId))
                {
                    countriesDictionary[country.CountryId] = country;
                }
                country = countriesDictionary[country.CountryId];
                
                country.Cities = country.Cities ?? new List<City>();
                country.Cities.Add(city);
                return country;
            }, splitOn: "CityIdSplit");
        
        return countriesDictionary.Values.ToList();
    }
    
    [Test]
    public void GetCountriesIncludeCityByDictionaryTest()
    {
        // Arrange
        var sut = new RelationshipSakila1toN();
        
        // Act
        var countries = sut.GetCountriesIncludeCityByDictionary();
        
        // Assert   
        countries.Should().HaveCount(109);
    }

    [Test]
    public void
        GetCountriesIncludeCityByDictionaryTestAndGetCountriesIncludeCityByGroupingTechniqueTestShouldBeTheSame()
    {
        //test if the two techniques are the same (by dictionary and by grouping)
        // Arrange
        var sut = new RelationshipSakila1toN();
        
        // Act
        var countriesByDictionary = sut.GetCountriesIncludeCityByDictionary();
        var countriesByGrouping = sut.GetCountriesIncludeCityByGroupingTechnique();
        
        // Assert
        countriesByDictionary.Should().BeEquivalentTo(countriesByGrouping);
    }


    public List<Country> GetCountriesIncludeCityByQueryMultiple()
    {
        var sut = new RelationshipSakila1toN();
        string sql =
            """
                SELECT c.country_id AS CountryId, c.country AS Name, c.last_update AS LastUpdate
                FROM country c;
                    
                SELECT ci.city_id AS CityId, ci.city AS Name, ci.country_id AS CountryId, ci.last_update AS LastUpdate 
                FROM city ci 
            """;
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var countries = connection.QueryMultiple(sql);
        var countriesList = countries.Read<Country>().ToList();
        var citiesList = countries.Read<City>().ToList();
        var citiesDictionary = citiesList.ToLookup(x => x.CountryId, x => x);
        var countriesIncludeCity = countriesList.Select(x => new Country()
        {
            CountryId = x.CountryId,
            Name = x.Name,
            LastUpdate = x.LastUpdate,
            Cities = citiesDictionary[x.CountryId].ToList()
        });
        return countriesIncludeCity.ToList();
    }
    
    [Test]
    public void GetCountriesIncludeCityByQueryMultipleTest()
    {
        // Arrange
        var sut = new RelationshipSakila1toN();
        
        // Act
        var countries = sut.GetCountriesIncludeCityByQueryMultiple();
        
        // Assert
        countries.Should().HaveCount(109);
    }
    
    [Test]
    public void
        GetCountriesIncludeCityByDictionaryTestAndGetCountriesIncludeCityByGroupingTechniqueAndGetCountriesIncludeCityByQueryMultipleTestShouldBeTheSame()
    {
        // Test if the three techniques are the same (by dictionary, by grouping and by query multiple)
        // Arrange
        var sut = new RelationshipSakila1toN();
        
        // Act
        var countriesByDictionary = sut.GetCountriesIncludeCityByDictionary();
        var countriesByGrouping = sut.GetCountriesIncludeCityByGroupingTechnique();
        var countriesByQueryMultiple = sut.GetCountriesIncludeCityByQueryMultiple();
        
        // Assert
        countriesByDictionary.Should().BeEquivalentTo(countriesByGrouping);
        countriesByDictionary.Should().BeEquivalentTo(countriesByQueryMultiple);
    }

    
    // public class Country
    // {
    //     public int CountryId { get; set; }
    //     public string Name { get; set; } = null!;
    //     public DateTime LastUpdate { get; set; }
    //     
    //     public List<City> Cities { get; set; } = null!;
    // }
    //
    // public class City
    // {
    //     public int CityId { get; set; }
    //     public string Name { get; set; } = null!;
    //     public int CountryId { get; set; }
    //     // public Country Country { get; set; } = null!;
    //     public DateTime LastUpdate { get; set; }
    // }
    
    public List<Country?> GetCountryIncludeCitiesAsJsonTrick()
    {
        string sql =
            """
            SELECT JSON_OBJECT( 'CountryId', c1.country_id,
                                'Name', c1.country,
                               'Cities', JSON_ARRAYAGG(
                                        JSON_OBJECT('Name', c2.city,
                                                    'CityId', c2.city_id,
                                                    'CountryId', c2.country_id
                                        )))
            FROM country c1 JOIN city c2 ON c1.country_id = c2.country_id
            GROUP BY c1.country_id, c1.country
            ORDER BY c1.country_id
            """;
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var jsonRows = connection.Query<string>(sql);
        var countries = jsonRows.Select(x => JsonSerializer.Deserialize<Country>(x))
            .ToList();

        return countries;
    }
    
    [Test]
    public void GetCountryIncludeCitiesAsJsonTrickTest()
    {
        // Arrange
        var sut = new RelationshipSakila1toN();
        
        // Act
        var countries = sut.GetCountryIncludeCitiesAsJsonTrick();
        
        // Assert
        countries.Should().HaveCount(109);
    }
}
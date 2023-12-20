using Argon;
using Dapper;
using DapperCourse;
using FluentAssertions;
using MySql.Data.MySqlClient;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DapperCourseTests;

public class Examples3_1toNRelationships
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
    
    // A country has many cities (1 to N relationship)
    // In the map function we can add the cities to the country, this is a list of cities
    // The problem is that each row returned from the database is a country with a city
    // So we need to group the cities by country after the Query method, see grouping by country comment
    // In my opinion this is not the best solution, because we need to group the cities by country
    // So we do a lot of work in the code, but we can also do this in the database and make it easier (and to understand)
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
        var countriesGrouped = countries
            .GroupBy(x => x.CountryId)
            .Select(g => new Country()
        {
            CountryId = g.Key,
            Name = g.First().Name,
            LastUpdate = g.First().LastUpdate,
            Cities = g.Select(x => x.Cities.Single()).ToList()
        });
        return countriesGrouped.ToList();
    }
    
    [Test]
    public async Task GetCountriesIncludeCityByGroupingTechniqueTest()
    {
        // Arrange
        var sut = new Examples3_1toNRelationships();
        
        // Act
        var countries = sut.GetCountriesIncludeCityByGroupingTechnique();
        
        // Assert
        countries.Should().HaveCount(109);
        await Verify(countries);
    }
    
    // This is the same as the previous method, but now we use a dictionary instead of grouping after the Query method.
    // In the map function of Query method this dictionary is used to add the cities to the country
    //                if (!countriesDictionary.ContainsKey(country.CountryId))
    //                {
    //                    countriesDictionary[country.CountryId] = country;
    //                }
    //                country = countriesDictionary[country.CountryId];
    
    // A dictionary is a key value pair, the key is the country id and the value is the country
    // So we can add the cities to the country by using the country id
    // First we check if the dictionary contains the country id, if not we add the country to the dictionary
    // Then we get the country from the dictionary and add the city to the country
    
    // Take a close look at what this method GetCountriesIncludeCityByDictionary returns!
    // Can you explain why this is the case?
    // The only problem I have with this, is that the order of the result is not guaranteed (because it is a dictionary)
    // So maybe it's a good idea to add a sort before returning if the order of the result is important
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
    public async Task GetCountriesIncludeCityByDictionaryTest()
    {
        // Arrange
        var sut = new Examples3_1toNRelationships();
        
        // Act
        var countries = sut.GetCountriesIncludeCityByDictionary();
        
        // Assert   
        countries.Should().HaveCount(109);
        await Verify(countries);
    }

    [Test]
    public void
        GetCountriesIncludeCityByDictionaryTestAndGetCountriesIncludeCityByGroupingTechniqueTestShouldBeTheSame()
    {
        //test if the two techniques are the same (by dictionary and by grouping)
        // Arrange
        var sut = new Examples3_1toNRelationships();
        
        // Act
        var countriesByDictionary = sut.GetCountriesIncludeCityByDictionary();
        var countriesByGrouping = sut.GetCountriesIncludeCityByGroupingTechnique();
        
        // Assert
        countriesByDictionary.Should().BeEquivalentTo(countriesByGrouping);
    }

    //Query multiple is a technique to execute multiple queries in one go
    //This is a technique to get the countries and cities in one go
    //The result is a list of countries and a list of cities
    //We can use the ToLookup() method to create a dictionary of cities
    //And combine the countries and cities to a list of countries with cities
    //In the next example we will use a trick to get the countries and cities in one go, but use sorting.
    //Maybe this is easier to understand.
    //The only problem that I have with this technique is that we cannot guarantee that all cities and countries are returned
    //we condition (WHERE) clauses are involved. 
    public List<Country> GetCountriesIncludeCityByQueryMultiple()
    {
        string sql =
            """
                SELECT c.country_id AS CountryId, c.country AS Name, c.last_update AS LastUpdate
                FROM country c;
                    
                SELECT ci.city_id AS CityId, ci.city AS Name, ci.country_id AS CountryId, ci.last_update AS LastUpdate 
                FROM city ci 
            """;
        
        //sort by country id, then we can also do it in one loop (maybe simpler then code below)
        
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
        var sut = new Examples3_1toNRelationships();
        
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
        var sut = new Examples3_1toNRelationships();
        
        // Act
        var countriesByDictionary = sut.GetCountriesIncludeCityByDictionary();
        var countriesByGrouping = sut.GetCountriesIncludeCityByGroupingTechnique();
        var countriesByQueryMultiple = sut.GetCountriesIncludeCityByQueryMultiple();
        
        // Assert
        countriesByDictionary.Should().BeEquivalentTo(countriesByGrouping);
        countriesByDictionary.Should().BeEquivalentTo(countriesByQueryMultiple);
    }

    // This is a trick to get the countries and cities in one go, but use sorting.
    // We can then use a loop to add the cities to the country
    // Still this will not work if we have a condition (WHERE) clause!
    // The solution is first query all the countries based on the condition (WHERE) clause,
    // store the country_id's in a list and then query all the cities based on the condition (country_id IN @CountryId) clause.
    // This is left as an exercise for the reader.
    public List<Country> GetCountriesMergeInLoop()
    {
        string sql =
            """
                SELECT c.country_id AS CountryId, c.country AS Name, c.last_update AS LastUpdate
                FROM country c ORDER BY c.country_id;
                    
                SELECT ci.city_id AS CityId, ci.city AS Name, ci.country_id AS CountryId, ci.last_update AS LastUpdate
                FROM city ci ORDER BY ci.country_id;
            """;
        
        using var connection = new MySqlConnection(GetConnectionStringForShop());
        var countries = connection.QueryMultiple(sql);
        var countriesList = countries.Read<Country>().ToList();
        var citiesList = countries.Read<City>().ToList();

        int idx = 0;
        foreach (var country in countriesList)
        {
            country.Cities ??= new List<City>();

            while (idx < citiesList.Count && citiesList[idx].CountryId == country.CountryId)
            {
                country.Cities.Add(citiesList[idx]);
                idx++;
            }
            
            // maybe this is easier to understand
            // var citiesForCountry = citiesList.TakeWhile(x => x.CountryId == country.CountryId);
            // country.Cities.AddRange(citiesForCountry);
            // citiesList = citiesList.SkipWhile(x => x.CountryId == country.CountryId).ToList();
        }
        
        return countriesList;
    }
    
    [Test]
    public async Task GetCountriesMergeInLoopTest()
    {
        // Arrange
        var sut = new Examples3_1toNRelationships();
        
        // Act
        var countries = sut.GetCountriesMergeInLoop();
        
        // Assert
        countries.Should().HaveCount(109);
        await Verify(countries);
    }
    
    //JSON Method
    //In my opinion this is the best solution, because we do all the work in the database
    //The problem with the previous solutions is that we need to do a lot of work outside of the database
    //Why don't we return just the structure that we need. Whit JSON we can do this!
    //Most modern databases support JSON, so we can use this technique in most databases.
    // We get back the following JSON for each row:

    //     {
    //         "Name": "Afghanistan",
    //         "Cities": [
    //         {
    //             "Name": "Kabul",
    //             "CityId": 251,
    //             "CountryId": 1
    //         }
    //         ],
    //         "CountryId": 1
    //     }
    
    // another row:
    
    //     {
    //         "Name": "Algeria",
    //         "Cities": [
    //         {
    //             "Name": "Batna",
    //             "CityId": 59,
    //             "CountryId": 2
    //         },
    //         {
    //             "Name": "Béchar",
    //             "CityId": 63,
    //             "CountryId": 2
    //         },
    //         {
    //             "Name": "Skikda",
    //             "CityId": 483,
    //             "CountryId": 2
    //         }
    //         ],
    //         "CountryId": 2
    //     }, 
    //     ...
    // ]
    //
    // [] is an array (in our case a list of cities)
    // {} is a object (in our case a country)
    // As you can see the JSON objects represents a country class structure
    // To convert from the JSON string to C# object instance we can use the JsonSerializer.Deserialize<Country>()  method.
    //
    // As you can see al the work is done in the database, we don't need to do any work in the code!
    // Al we need is to learn how to use JSON in the database.
    // There are many tutorials on the internet, for example:
    // https://www.youtube.com/watch?v=QZBxgX2OWbI (watch till 11:00)   
    // https://www.mysqltutorial.org/mysql-json/
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

        var countries = new List<Country?>();
        foreach (var jsonRow in jsonRows)
        {
            var country = JsonSerializer.Deserialize<Country>(jsonRow);
            countries.Add(country);
        }

        return countries;
        
        // Same as above, without loop but with LINQ
        // var countries = jsonRows.Select(x => JsonSerializer.Deserialize<Country>(x))
        //     .ToList();
    }
    
    [Test]
    public void GetCountryIncludeCitiesAsJsonTrickTest()
    {
        // Arrange
        var sut = new Examples3_1toNRelationships();
        
        // Act
        var countries = sut.GetCountryIncludeCitiesAsJsonTrick();
        
        // Assert
        countries.Should().HaveCount(109);
    }
}
using System.Text.Json;

namespace DapperCourseTests;

public class JsonSerializeDeserializeExample
{
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

    private string _jsonCountry = """
                                 {
                                   "Name": "Algeria",
                                   "Cities": [
                                     {
                                       "Name": "Batna",
                                       "CityId": 59,
                                       "CountryId": 2
                                     },
                                     {
                                       "Name": "Béchar",
                                       "CityId": 63,
                                       "CountryId": 2
                                     },
                                     {
                                       "Name": "Skikda",
                                       "CityId": 483,
                                       "CountryId": 2
                                     }
                                   ],
                                   "CountryId": 2
                                 }
                                 """;
    
    [Test]
    public void DeserializeJson()
    {
        Country country = JsonSerializer.Deserialize<Country>(_jsonCountry)!;
        //place breakpoint to inspect country
    }
    
    public void SerializeAsJson()
    {
        var country = new Country
        {
            CountryId = 1,
            Name = "Afghanistan",
            LastUpdate = DateTime.MinValue,
            Cities = new List<City>
            {
                new City
                {
                    CityId = 251,
                    Name = "Kabul",
                    CountryId = 1,
                    LastUpdate = DateTime.MinValue
                }
            }
        };

        string countryAsJson = JsonSerializer.Serialize(country);
        //place breakpoint to inspect countryAsJson
    }   
}
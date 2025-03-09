using Microsoft.Extensions.Configuration;
using RealEstateSearch.Exceptions;
using RealEstateSearch.Models;
using RealEstateSearch.SearchEngine;

public class Program
{
    public static void Main()
    {
        try
        {
            Console.WriteLine("Initializing Real Estate Search Engine...");
            var searchEngineTest = new RealEstateSearchEngine();

            // Sample properties data
            var properties = new List<Property>
                {
                    new Property
                    {
                        Price = 300000,
                        SquareFootage = 2000,
                        Bedrooms = 3,
                        Bathrooms = 2,
                        Location = "Downtown",
                        PropertyType = "Apartment",
                        YearBuilt = 2010,
                        Latitude = 40.7128,
                        Longitude = -74.0060,
                        ListingDate = DateTime.Now.AddDays(-30),
                        Amenities = new List<string> { "Parking", "Pool", "Gym" },
                        Conditions = "Excellent"
                    },
                    new Property
                    {
                        Price = 350000,
                        SquareFootage = 2200,
                        Bedrooms = 3,
                        Bathrooms = 2,
                        Location = "Downtown",
                        PropertyType = "Condo",
                        YearBuilt = 2015,
                        Latitude = 40.7129,
                        Longitude = -74.0055,
                        ListingDate = DateTime.Now.AddDays(-15),
                        Amenities = new List<string> { "Parking", "Gym" },
                        Conditions = "Excellent"
                    },
                    new Property
                    {
                        Price = 275000,
                        SquareFootage = 1800,
                        Bedrooms = 2,
                        Bathrooms = 1,
                        Location = "Midtown",
                        PropertyType = "Apartment",
                        YearBuilt = 2005,
                        Latitude = 40.7540,
                        Longitude = -73.9900,
                        ListingDate = DateTime.Now.AddDays(-45),
                        Amenities = new List<string> { "Parking" },
                        Conditions = "Good"
                    }
                };

            Console.WriteLine("Loading properties...");
            // Load and validate properties
            searchEngineTest.LoadProperties(properties);

            Console.WriteLine("Performing advanced search...");
            // Perform advanced search
            var searchCriteria = new SearchCriteria
            {
                MinPrice = 250000,
                MaxPrice = 350000,
                MinBedrooms = 2,
                Location = "Downtown",
                MaxDistance = 5,
                Latitude = 40.7128,
                Longitude = -74.0060,
                RequiredAmenities = new List<string> { "Parking" },
                DaysOnMarket = 60
            };

            var searchResults = searchEngineTest.AdvancedSearch(searchCriteria);
            Console.WriteLine($"Found {searchResults.Count} matching properties");

            foreach (var property in searchResults)
            {
                Console.WriteLine($"Property: {property.PropertyType} in {property.Location}");
                Console.WriteLine($"  Price: ${property.Price}");
                Console.WriteLine($"  Size: {property.SquareFootage} sq ft, {property.Bedrooms} bed, {property.Bathrooms} bath");
                Console.WriteLine($"  Built: {property.YearBuilt}");
                Console.WriteLine($"  Amenities: {string.Join(", ", property.Amenities)}");
                Console.WriteLine();
            }

            Console.WriteLine("Performing geographic clustering...");
            // Perform geographic clustering
            var clustersTest = searchEngineTest.PerformGeographicClustering();
            Console.WriteLine($"Found {clustersTest.Count} property clusters");

            foreach (var cluster in clustersTest)
            {
                Console.WriteLine($"Cluster {cluster.ClusterId}:");
                Console.WriteLine($"  Properties: {cluster.PropertyCount}");
                Console.WriteLine($"  Average Price: ${cluster.AveragePrice:N2}");
                Console.WriteLine($"  Center: {cluster.CenterLatitude}, {cluster.CenterLongitude}");
                Console.WriteLine();
            }

            Console.WriteLine("Performing time-based analysis...");
            // Perform time-based analysis
            var timeAnalysisTest = searchEngineTest.PerformTimeAnalysis();

            if (timeAnalysisTest.ContainsKey("AverageDaysOnMarket"))
            {
                Console.WriteLine($"Average Days on Market: {timeAnalysisTest["AverageDaysOnMarket"]:N1} days");

                var priceTrends = (Dictionary<string, object>)timeAnalysisTest["PriceTrends"];
                Console.WriteLine("\nPrice Trends by Month:");
                foreach (var trend in priceTrends)
                {
                    Console.WriteLine($"  {trend.Key}: ${trend.Value:N2}");
                }
            }
            else
            {
                Console.WriteLine($"Time analysis error: {timeAnalysisTest["Error"]}");
            }

            Console.WriteLine("\nUsing similarity-based search...");
            // Similarity search
            var sampleProperty = new Property
            {
                Price = 325000,
                SquareFootage = 2100,
                Bedrooms = 3,
                Bathrooms = 2,
                Location = "Downtown",
                PropertyType = "Apartment",
                YearBuilt = 2010
            };

            var similarPropertiesTest = searchEngineTest.Search(sampleProperty, 2);
            Console.WriteLine($"Found {similarPropertiesTest.Count} similar properties");

            foreach (var property in similarPropertiesTest)
            {
                Console.WriteLine($"Similar Property: {property.PropertyType} in {property.Location}");
                Console.WriteLine($"  Price: ${property.Price}");
                Console.WriteLine($"  Size: {property.SquareFootage} sq ft");
                Console.WriteLine();
            }
        }
        catch (RealEstateSearchException ex)
        {
            Console.WriteLine($"Search Error: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Details: {ex.InnerException.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var searchEngine = new RealEstateSearchEngine(configuration);

        searchEngine.LoadPropertiesFromDatabase();

        var clusters = searchEngine.PerformGeographicClustering();
        var timeAnalysis = searchEngine.PerformTimeAnalysis();

        // Example advanced search
        var criteria = new SearchCriteria
        {
            MinPrice = 100000,
            MaxPrice = 500000,
            MinBedrooms = 2,
            Location = "Seattle"
        };

        var results = searchEngine.AdvancedSearch(criteria);

        // Display results
        foreach (var property in results)
        {
            Console.WriteLine($"Property: Price {property.Price}, {property.Bedrooms} beds, {property.Bathrooms} baths, {property.SquareFootage} sq.ft. in {property.Location}");
        }
    }
}

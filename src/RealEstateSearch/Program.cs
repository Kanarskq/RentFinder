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
            var searchEngine = new RealEstateSearchEngine();

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
                        Condition = "Excellent"
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
                        Condition = "Excellent"
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
                        Condition = "Good"
                    }
                };

            Console.WriteLine("Loading properties...");
            // Load and validate properties
            searchEngine.LoadProperties(properties);

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

            var searchResults = searchEngine.AdvancedSearch(searchCriteria);
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
            var clusters = searchEngine.PerformGeographicClustering();
            Console.WriteLine($"Found {clusters.Count} property clusters");

            foreach (var cluster in clusters)
            {
                Console.WriteLine($"Cluster {cluster.ClusterId}:");
                Console.WriteLine($"  Properties: {cluster.PropertyCount}");
                Console.WriteLine($"  Average Price: ${cluster.AveragePrice:N2}");
                Console.WriteLine($"  Center: {cluster.CenterLatitude}, {cluster.CenterLongitude}");
                Console.WriteLine();
            }

            Console.WriteLine("Performing time-based analysis...");
            // Perform time-based analysis
            var timeAnalysis = searchEngine.PerformTimeAnalysis();

            if (timeAnalysis.ContainsKey("AverageDaysOnMarket"))
            {
                Console.WriteLine($"Average Days on Market: {timeAnalysis["AverageDaysOnMarket"]:N1} days");

                var priceTrends = (Dictionary<string, double>)timeAnalysis["PriceTrends"];
                Console.WriteLine("\nPrice Trends by Month:");
                foreach (var trend in priceTrends)
                {
                    Console.WriteLine($"  {trend.Key}: ${trend.Value:N2}");
                }
            }
            else
            {
                Console.WriteLine($"Time analysis error: {timeAnalysis["Error"]}");
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

            var similarProperties = searchEngine.Search(sampleProperty, 2);
            Console.WriteLine($"Found {similarProperties.Count} similar properties");

            foreach (var property in similarProperties)
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
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RealEstateSearch.Exceptions;
using RealEstateSearch.Models;
using RealEstateSearch.SearchEngine;

namespace RealEstateSearch.Tests;

[TestClass]
public class RealEstateSearchEngineTests
{
    private RealEstateSearchEngine _searchEngine;
    private List<Property> _testProperties;

    [TestInitialize]
    public void Setup()
    {
        _searchEngine = new RealEstateSearchEngine();
        _testProperties = new List<Property>
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
                    Longitude = -74.006,
                    ListingDate = new DateTime(2025, 1, 15),
                    Amenities = new List<string> { "Parking", "Pool", "Gym" },
                    Conditions = "Good"
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
                    Latitude = 40.7130,
                    Longitude = -74.008,
                    ListingDate = new DateTime(2025, 2, 10),
                    Amenities = new List<string> { "Parking", "Gym" },
                    Conditions = "Excellent"
                },
                new Property
                {
                    Price = 250000,
                    SquareFootage = 1800,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    Location = "Suburbs",
                    PropertyType = "House",
                    YearBuilt = 2000,
                    Latitude = 40.8000,
                    Longitude = -74.1000,
                    ListingDate = new DateTime(2025, 1, 5),
                    Amenities = new List<string> { "Garage", "Yard" },
                    Conditions = "Good"
                }
            };
    }

    [TestMethod]
    public void LoadProperties_WithValidProperties_LoadsSuccessfully()
    {
        try
        {
            _searchEngine.LoadProperties(_testProperties);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Expected no exception, but got: {ex.Message}");
        }
    }

    [TestMethod]
    [ExpectedException(typeof(RealEstateSearchException))]
    public void LoadProperties_WithEmptyList_ThrowsException()
    {
        _searchEngine.LoadProperties(new List<Property>());

        // Assert - handled by ExpectedException attribute
    }

    [TestMethod]
    public void AdvancedSearch_WithPriceRange_ReturnsMatchingProperties()
    {
        _searchEngine.LoadProperties(_testProperties);
        var criteria = new SearchCriteria
        {
            MinPrice = 300000,
            MaxPrice = 400000
        };

        var results = _searchEngine.AdvancedSearch(criteria);

        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.All(p => p.Price >= 300000 && p.Price <= 400000));
    }

    [TestMethod]
    public void AdvancedSearch_WithLocation_ReturnsMatchingProperties()
    {
        _searchEngine.LoadProperties(_testProperties);
        var criteria = new SearchCriteria
        {
            Location = "Downtown"
        };

        var results = _searchEngine.AdvancedSearch(criteria);

        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.All(p => p.Location.Equals("Downtown", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void AdvancedSearch_WithRequiredAmenities_ReturnsMatchingProperties()
    {
        _searchEngine.LoadProperties(_testProperties);
        var criteria = new SearchCriteria
        {
            RequiredAmenities = new List<string> { "Parking", "Gym" }
        };

        var results = _searchEngine.AdvancedSearch(criteria);

        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.All(p => p.Amenities.Contains("Parking") && p.Amenities.Contains("Gym")));
    }

    [TestMethod]
    public void PerformGeographicClustering_GroupsPropertiesByLocation()
    {
        _searchEngine.LoadProperties(_testProperties);

        var clusters = _searchEngine.PerformGeographicClustering();

        Assert.IsNotNull(clusters);
        // We expect 2 clusters: Downtown properties and Suburbs property
        Assert.AreEqual(2, clusters.Count);

        // The downtown cluster should have 2 properties
        var downtownCluster = clusters.FirstOrDefault(c => c.Properties.Any(p => p.Location == "Downtown"));
        Assert.IsNotNull(downtownCluster);
        Assert.AreEqual(2, downtownCluster.Properties.Count);

        // The suburbs cluster should have 1 property
        var suburbsCluster = clusters.FirstOrDefault(c => c.Properties.Any(p => p.Location == "Suburbs"));
        Assert.IsNotNull(suburbsCluster);
        Assert.AreEqual(1, suburbsCluster.Properties.Count);
    }

    [TestMethod]
    public void PerformTimeAnalysis_CalculatesCorrectTimeMetrics()
    {
        // Arrange
        _searchEngine.LoadProperties(_testProperties);

        // Act
        var analysis = _searchEngine.PerformTimeAnalysis();

        // Assert
        Assert.IsNotNull(analysis);
        Assert.IsTrue(analysis.ContainsKey("AverageDaysOnMarket"));
        Assert.IsTrue(analysis.ContainsKey("PriceTrends"));

        // First check the type before casting
        Assert.IsInstanceOfType(analysis["PriceTrends"], typeof(Dictionary<string, object>));

        var priceTrends = (Dictionary<string, double>)analysis["PriceTrends"];

        // The priceTrends dictionary should have 2 entries
        Assert.AreEqual(2, priceTrends.Count);
        Assert.IsTrue(priceTrends.ContainsKey("2025-01"));
        Assert.IsTrue(priceTrends.ContainsKey("2025-02"));

        // The average price for January should be (300000 + 250000) / 2 = 275000
        Assert.AreEqual((Single)275000, priceTrends["2025-01"]);
        // The average price for February should be 350000
        Assert.AreEqual((Single)350000, priceTrends["2025-02"]);
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RealEstateSearch.Models;
using RealEstateSearch.SearchEngine;
using RealEstateSearch.Validation;

namespace RealEstateSearch.Tests;

[TestClass]
public class ValidationTests
{
    private RealEstateSearchEngine _searchEngine;

    [TestInitialize]
    public void Setup()
    {
        _searchEngine = new RealEstateSearchEngine();
    }

    [TestMethod]
    public void ValidateProperty_WithValidProperty_ReturnsValidResult()
    {
        var property = new Property
        {
            Price = 300000,
            SquareFootage = 2000,
            Bedrooms = 3,
            Bathrooms = 2,
            Location = "Downtown",
            PropertyType = "Apartment",
            YearBuilt = 2010,
            Latitude = 40.7128,
            Longitude = -74.006
        };

        ValidationResult result = _searchEngine.ValidateProperty(property);

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    [TestMethod]
    public void ValidateProperty_WithNullProperty_ReturnsInvalidResult()
    {
        ValidationResult result = _searchEngine.ValidateProperty(null);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual("Property cannot be null", result.Errors[0]);
    }

    [TestMethod]
    public void ValidateProperty_WithInvalidPrice_ReturnsInvalidResult()
    {
        var property = new Property
        {
            Price = -5000,
            SquareFootage = 2000,
            Bedrooms = 3,
            Bathrooms = 2,
            Location = "Downtown",
            PropertyType = "Apartment",
            YearBuilt = 2010,
            Latitude = 40.7128,
            Longitude = -74.006
        };

        ValidationResult result = _searchEngine.ValidateProperty(property);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Contains("Price must be greater than 0"));
    }

    [TestMethod]
    public void ValidateProperty_WithInvalidSquareFootage_ReturnsInvalidResult()
    {
        var property = new Property
        {
            Price = 300000,
            SquareFootage = 0,
            Bedrooms = 3,
            Bathrooms = 2,
            Location = "Downtown",
            PropertyType = "Apartment",
            YearBuilt = 2010,
            Latitude = 40.7128,
            Longitude = -74.006
        };

        ValidationResult result = _searchEngine.ValidateProperty(property);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Contains("Square footage must be greater than 0"));
    }

    [TestMethod]
    public void ValidateProperty_WithNegativeBedrooms_ReturnsInvalidResult()
    {
        var property = new Property
        {
            Price = 300000,
            SquareFootage = 2000,
            Bedrooms = -1,
            Bathrooms = 2,
            Location = "Downtown",
            PropertyType = "Apartment",
            YearBuilt = 2010,
            Latitude = 40.7128,
            Longitude = -74.006
        };

        ValidationResult result = _searchEngine.ValidateProperty(property);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Contains("Bedrooms cannot be negative"));
    }
}

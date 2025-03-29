using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using MySql.Data.MySqlClient;
using RealEstateSearch.Exceptions;
using RealEstateSearch.Models;
using RealEstateSearch.Validation;
using System.Data;

namespace RealEstateSearch.SearchEngine;

public class RealEstateSearchEngine
{
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private List<Property> _properties;
    private const int MAX_CLUSTER_SIZE = 100;
    private const double CLUSTER_RADIUS_KM = 3.0;
    private readonly string _connectionString;

    public RealEstateSearchEngine(IConfiguration configuration)
    {
        _mlContext = new MLContext(seed: 0);
        _properties = new List<Property>();
        _connectionString = configuration.GetConnectionString("PropertyDatabase") ??
            throw new InvalidOperationException("Connection string 'PropertyDatabase' not found");
    }

    public RealEstateSearchEngine()
    {
        _mlContext = new MLContext(seed: 0);
        _properties = new List<Property>();
    }

    public ValidationResult ValidateProperty(Property property)
    {
        var result = new ValidationResult();

        if (property == null)
        {
            result.Errors.Add("Property cannot be null");
            result.IsValid = false;
            return result;
        }

        if (property.Price <= 0)
            result.Errors.Add("Price must be greater than 0");

        if (property.SquareFootage <= 0)
            result.Errors.Add("Square footage must be greater than 0");

        if (property.Bedrooms < 0)
            result.Errors.Add("Bedrooms cannot be negative");

        if (property.Bathrooms < 0)
            result.Errors.Add("Bathrooms cannot be negative");

        if (string.IsNullOrWhiteSpace(property.Location))
            result.Errors.Add("Location is required");

        if (property.YearBuilt < 1800 || property.YearBuilt > DateTime.Now.Year)
            result.Errors.Add("Invalid year built");

        if (property.Latitude < -90 || property.Latitude > 90)
            result.Errors.Add("Invalid latitude");

        if (property.Longitude < -180 || property.Longitude > 180)
            result.Errors.Add("Invalid longitude");

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    // Load and preprocess the property data
    public void LoadPropertiesFromDatabase()
    {
        try
        {
            _properties = new List<Property>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand("SELECT * FROM properties", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var property = new Property
                        {
                            Price = reader.GetFloat("price"),
                            SquareFootage = reader.GetFloat("square_footage"),
                            Bedrooms = reader.GetFloat("bedrooms"),
                            Bathrooms = reader.GetFloat("bathrooms"),
                            Location = reader.GetString("location"),
                            PropertyType = reader.GetString("property_type"),
                            YearBuilt = reader.GetFloat("year_built"),
                            Latitude = reader.GetDouble("latitude"),
                            Longitude = reader.GetDouble("longitude"),
                        };

                        // Handle the listing date
                        if (!reader.IsDBNull("listing_date"))
                        {
                            property.ListingDate = reader.GetDateTime("listing_date");
                        }

                        // Get amenities from a related table
                        var propertyId = reader.GetInt32("property_id");

                        var validation = ValidateProperty(property);
                        if (validation.IsValid)
                        {
                            _properties.Add(property);
                        }
                    }
                }
            }

            if (_properties.Count == 0)
                throw new RealEstateSearchException("No valid properties loaded from database");


            // Create pipeline to process only the simple features
            var pipeline = _mlContext.Transforms
                .Categorical.OneHotEncoding(outputColumnName: "LocationEncoded", inputColumnName: nameof(Property.Location))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PropertyTypeEncoded", inputColumnName: nameof(Property.PropertyType)))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    nameof(Property.Price),
                    nameof(Property.SquareFootage),
                    nameof(Property.Bedrooms),
                    nameof(Property.Bathrooms),
                    nameof(Property.YearBuilt),
                    "LocationEncoded",
                    "PropertyTypeEncoded"))
                .Append(_mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features"))
                .Append(_mlContext.Transforms.CopyColumns(outputColumnName: "Score", inputColumnName: "NormalizedFeatures")); ;

            var trainingData = _mlContext.Data.LoadFromEnumerable(_properties);
            _model = pipeline.Fit(trainingData);

            Console.WriteLine($"Successfully loaded {_properties.Count} properties");
        }
        catch (Exception ex)
        {
            throw new RealEstateSearchException("Error loading properties", ex);
        }
    }

    public void LoadProperties(List<Property> properties)
    {
        try
        {
            var validProperties = new List<Property>();
            foreach (var property in properties)
            {
                var validation = ValidateProperty(property);
                if (validation.IsValid)
                {
                    validProperties.Add(property);
                }
            }

            _properties = validProperties;

            if (_properties.Count == 0)
                throw new RealEstateSearchException("No valid properties to load");

            // Create pipeline to process only the simple features
            var pipeline = _mlContext.Transforms
                .Categorical.OneHotEncoding(outputColumnName: "LocationEncoded", inputColumnName: nameof(Property.Location))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PropertyTypeEncoded", inputColumnName: nameof(Property.PropertyType)))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    nameof(Property.Price),
                    nameof(Property.SquareFootage),
                    nameof(Property.Bedrooms),
                    nameof(Property.Bathrooms),
                    nameof(Property.YearBuilt),
                    "LocationEncoded",
                    "PropertyTypeEncoded"))
                .Append(_mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features"))
                .Append(_mlContext.Transforms.CopyColumns(outputColumnName: "Score", inputColumnName: "NormalizedFeatures"));

            var trainingData = _mlContext.Data.LoadFromEnumerable(_properties);
            _model = pipeline.Fit(trainingData);

            Console.WriteLine($"Successfully loaded {_properties.Count} properties");
        }
        catch (Exception ex)
        {
            throw new RealEstateSearchException("Error loading properties", ex);
        }
    }

    private List<string> GetAmenitiesForProperty(int propertyId)
    {
        var amenities = new List<string>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new MySqlCommand("SELECT amenity_name FROM property_amenities WHERE property_id = @PropertyId", connection))
            {
                command.Parameters.AddWithValue("@PropertyId", propertyId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        amenities.Add(reader.GetString("amenity_name"));
                    }
                }
            }
        }

        return amenities;
    }

    public List<PropertyCluster> PerformGeographicClustering()
    {
        var clusters = new List<PropertyCluster>();
        var processedProperties = new HashSet<Property>();

        foreach (var property in _properties)
        {
            if (processedProperties.Contains(property))
                continue;

            var cluster = new PropertyCluster
            {
                ClusterId = Guid.NewGuid().ToString(),
                Properties = new List<Property> { property },
                CenterLatitude = property.Latitude,
                CenterLongitude = property.Longitude
            };

            // Current property as processed
            processedProperties.Add(property);

            var nearbyProperties = _properties
            .Where(p => !processedProperties.Contains(p) &&
                  p != property && 
                  CalculateDistance(
                      property.Latitude, property.Longitude,
                      p.Latitude, p.Longitude) <= CLUSTER_RADIUS_KM)
            .Take(MAX_CLUSTER_SIZE - 1);

            cluster.Properties.AddRange(nearbyProperties);

            // Mark all nearby properties as processed
            foreach (var nearbyProperty in nearbyProperties)
            {
                processedProperties.Add(nearbyProperty);
            }

            // Calculate cluster statistics
            cluster.AveragePrice = cluster.Properties.Average(p => p.Price);
            cluster.PropertyCount = cluster.Properties.Count;
            
            clusters.Add(cluster);
        }

        return clusters;
    }

    public Dictionary<string, object> PerformTimeAnalysis()
    {
        var analysis = new Dictionary<string, object>();

        if (_properties.Any(p => p.ListingDate != default))
        {
            // Average days on market
            var averageDaysOnMarket = _properties
                .Where(p => p.ListingDate != default)
                .Average(p => (DateTime.Now - p.ListingDate).TotalDays);

            // Price trends by month
            var priceTrends = _properties
                .Where(p => p.ListingDate != default)
                .GroupBy(p => new { p.ListingDate.Year, p.ListingDate.Month })
                .Select(g => new
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    AveragePrice = (object)g.Average(p => p.Price)
                })
                .OrderBy(x => x.Period)
                .ToDictionary(x => x.Period, x => x.AveragePrice);

            analysis.Add("AverageDaysOnMarket", averageDaysOnMarket);
            analysis.Add("PriceTrends", priceTrends);
        }
        else
        {
            analysis.Add("Error", "No time data available in properties");
        }

        return analysis;
    }

    // Search for similar properties based on input criteria
    public List<Property> Search(Property searchCriteria, int numberOfResults = 5)
    {
        if (_properties.Count == 0 || _model == null)
            throw new InvalidOperationException("Model not trained or no properties loaded");

        // Create prediction engine
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<Property, PropertyMatch>(_model);

        // Calculate similarity scores for all properties
        var similarities = _properties.Select(p => new
        {
            Property = p,
            Similarity = CalculateSimilarity(p, searchCriteria, predictionEngine)
        }).OrderByDescending(x => x.Similarity);

        return similarities.Take(numberOfResults)
                         .Select(x => x.Property)
                         .ToList();
    }

    public List<Property> AdvancedSearch(SearchCriteria criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        var results = _properties.AsEnumerable();

        // Apply filters based on criteria
        if (criteria.MinPrice.HasValue)
            results = results.Where(p => p.Price >= criteria.MinPrice.Value);

        if (criteria.MaxPrice.HasValue)
            results = results.Where(p => p.Price <= criteria.MaxPrice.Value);

        if (criteria.MinSquareFootage.HasValue)
            results = results.Where(p => p.SquareFootage >= criteria.MinSquareFootage.Value);

        if (criteria.MaxSquareFootage.HasValue)
            results = results.Where(p => p.SquareFootage <= criteria.MaxSquareFootage.Value);

        if (criteria.MinBedrooms.HasValue)
            results = results.Where(p => p.Bedrooms >= criteria.MinBedrooms.Value);

        if (criteria.MaxBedrooms.HasValue)
            results = results.Where(p => p.Bedrooms <= criteria.MaxBedrooms.Value);

        if (!string.IsNullOrEmpty(criteria.Location))
            results = results.Where(p => p.Location.Equals(criteria.Location, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(criteria.PropertyType))
            results = results.Where(p => p.PropertyType.Equals(criteria.PropertyType, StringComparison.OrdinalIgnoreCase));

        if (criteria.MaxAge.HasValue)
            results = results.Where(p => (DateTime.Now.Year - p.YearBuilt) <= criteria.MaxAge.Value);

        if (criteria.Latitude.HasValue && criteria.Longitude.HasValue && criteria.MaxDistance.HasValue)
            results = results.Where(p =>
                CalculateDistance(
                    criteria.Latitude.Value, criteria.Longitude.Value,
                    p.Latitude, p.Longitude) <= criteria.MaxDistance.Value);

        if (criteria.RequiredAmenities != null && criteria.RequiredAmenities.Any())
            results = results.Where(p => p.Amenities != null &&
                criteria.RequiredAmenities.All(a => p.Amenities.Contains(a)));

        if (!string.IsNullOrEmpty(criteria.Conditions))
            results = results.Where(p => p.Conditions != null &&
                p.Conditions.Equals(criteria.Conditions, StringComparison.OrdinalIgnoreCase));

        if (criteria.DaysOnMarket.HasValue)
            results = results.Where(p => p.ListingDate != default &&
                (DateTime.Now - p.ListingDate).TotalDays <= criteria.DaysOnMarket.Value);

        return results.ToList();
    }


    // Calculate similarity score between two properties
    private float CalculateSimilarity(Property p1, Property p2, PredictionEngine<Property, PropertyMatch> predEngine)
    {
        try
        {
            var features1 = predEngine.Predict(p1).Score;
            var features2 = predEngine.Predict(p2).Score;

            // Calculate Euclidean distance
            float distance = 0;
            for (int i = 0; i < Math.Min(features1.Length, features2.Length); i++)
            {
                distance += (float)Math.Pow(features1[i] - features2[i], 2);
            }
            distance = (float)Math.Sqrt(distance);

            // Convert distance to similarity score (inverse relationship)
            return 1 / (1 + distance);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calculating similarity: {ex.Message}");
            return 0;
        }
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        lat1 = DegreesToRadians(lat1);
        lat2 = DegreesToRadians(lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

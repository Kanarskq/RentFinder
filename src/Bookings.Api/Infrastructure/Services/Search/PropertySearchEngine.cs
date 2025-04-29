using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace Bookings.Api.Infrastructure.Services.Search;

public class PropertySearchEngine : IPropertySearchEngine
{
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private ITransformer _knnModel;
    private readonly ILogger<PropertySearchEngine> _logger;
    private readonly IPropertyRepository _propertyRepository;
    private List<Property> _properties;
    private PredictionEngine<PropertyMLModel, PropertyMLPrediction> _predictionEngine;
    private const int MAX_CLUSTER_SIZE = 100;
    private const double CLUSTER_RADIUS_KM = 3.0;
    private const int TOP_SIMILAR_PROPERTIES = 12;

    public PropertySearchEngine(ILogger<PropertySearchEngine> logger, IPropertyRepository propertyRepository)
    {
        _mlContext = new MLContext(seed: 0);
        _logger = logger;
        _propertyRepository = propertyRepository;
        _properties = new List<Property>();
    }

    public async Task InitializeAsync()
    {
        try
        {
            _properties = (await _propertyRepository.GetAllAsync()).ToList();

            if (_properties.Count == 0)
            {
                _logger.LogWarning("No properties found to initialize the search engine");
                return;
            }

            var propertyData = _mlContext.Data.LoadFromEnumerable(_properties.Select(p => new PropertyMLModel
            {
                PropertyId = p.Id,
                Price = (float)p.Price,
                OwnerId = p.OwnerId,
                CreatedAt = p.CreatedAt,
                Latitude = (float)p.Latitude,
                Longitude = (float)p.Longitude,
                SquareFootage = p.SquareFootage,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                YearBuilt = p.YearBuilt,
                HasBalcony = p.HasBalcony ? 1 : 0,
                HasParking = p.HasParking ? 1 : 0,
                PetsAllowed = p.PetsAllowed ? 1 : 0,
                PropertyType = p.PropertyType
            }));

            var pipeline = _mlContext.Transforms.Conversion
                .ConvertType("PriceFloat", "Price", DataKind.Single)
                .Append(_mlContext.Transforms.Conversion.ConvertType("OwnerIdFloat", nameof(PropertyMLModel.OwnerId), DataKind.Single))
                .Append(_mlContext.Transforms.Conversion.ConvertType("BedroomsFloat", nameof(PropertyMLModel.Bedrooms), DataKind.Single))
                .Append(_mlContext.Transforms.Conversion.ConvertType("BathroomsFloat", nameof(PropertyMLModel.Bathrooms), DataKind.Single))
                .Append(_mlContext.Transforms.Conversion.ConvertType("YearBuiltFloat", nameof(PropertyMLModel.YearBuilt), DataKind.Single))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding("PropertyTypeEncoded", nameof(PropertyMLModel.PropertyType)))
                .Append(_mlContext.Transforms.CustomMapping<PropertyMLModel, PropertyMLFeaturesModel>(
                    (input, output) => {
                        output.CreatedAtDays = (float)(input.CreatedAt - new DateTime(2000, 1, 1)).TotalDays;
                        output.PriceFloat = (float)input.Price;
                        output.OwnerIdFloat = (float)input.OwnerId;
                        output.LatitudeFloat = (float)input.Latitude;
                        output.LongitudeFloat = (float)input.Longitude;
                        output.SquareFootageFloat = input.SquareFootage;
                        output.BedroomsFloat = (float)input.Bedrooms;
                        output.BathroomsFloat = (float)input.Bathrooms;
                        output.YearBuiltFloat = (float)input.YearBuilt;
                        output.HasBalconyFloat = input.HasBalcony;
                        output.HasParkingFloat = input.HasParking;
                        output.PetsAllowedFloat = input.PetsAllowed;
                        output.PropertyAge = (float)(DateTime.Now.Year - input.YearBuilt);
                    }, "DateTimeMapping"))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "PriceFloat", "OwnerIdFloat", "CreatedAtDays", "LatitudeFloat", "LongitudeFloat",
                    "SquareFootageFloat", "BedroomsFloat", "BathroomsFloat", "YearBuiltFloat",
                    "HasBalconyFloat", "HasParkingFloat", "PetsAllowedFloat", "PropertyTypeEncoded"))
                .Append(_mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features"));

            _model = pipeline.Fit(propertyData);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<PropertyMLModel, PropertyMLPrediction>(_model);

            _logger.LogInformation("Enhanced search engine initialized with {PropertyCount} properties", _properties.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing enhanced property search engine");
            throw;
        }
    }

    public async Task TrainKNNModelAsync()
    {
        await EnsureInitializedAsync();

        if (_properties.Count < 10)
        {
            _logger.LogWarning("Not enough properties to train a KNN model (minimum 10 required)");
            return;
        }

        var propertyData = _mlContext.Data.LoadFromEnumerable(_properties.Select(p => new PropertyMLModel
        {
            PropertyId = p.Id,
            Price = (float)p.Price,
            OwnerId = p.OwnerId,
            CreatedAt = p.CreatedAt,
            Latitude = (float)p.Latitude,
            Longitude = (float)p.Longitude,
            SquareFootage = p.SquareFootage,
            Bedrooms = p.Bedrooms,
            Bathrooms = p.Bathrooms,
            YearBuilt = p.YearBuilt,
            HasBalcony = p.HasBalcony ? 1 : 0,
            HasParking = p.HasParking ? 1 : 0,
            PetsAllowed = p.PetsAllowed ? 1 : 0,
            PropertyType = p.PropertyType
        }));

        var dataSplit = _mlContext.Data.TrainTestSplit(propertyData, testFraction: 0.2);

        var pipeline = _mlContext.Transforms.Conversion
            .ConvertType("PriceFloat", "Price", DataKind.Single)
            .Append(_mlContext.Transforms.Conversion.ConvertType("OwnerIdFloat", nameof(PropertyMLModel.OwnerId), DataKind.Single))
            .Append(_mlContext.Transforms.Conversion.ConvertType("BedroomsFloat", nameof(PropertyMLModel.Bedrooms), DataKind.Single))
            .Append(_mlContext.Transforms.Conversion.ConvertType("BathroomsFloat", nameof(PropertyMLModel.Bathrooms), DataKind.Single))
            .Append(_mlContext.Transforms.Conversion.ConvertType("YearBuiltFloat", nameof(PropertyMLModel.YearBuilt), DataKind.Single))
            .Append(_mlContext.Transforms.Categorical.OneHotEncoding("PropertyTypeEncoded", nameof(PropertyMLModel.PropertyType)))
            .Append(_mlContext.Transforms.CustomMapping<PropertyMLModel, PropertyMLFeaturesModel>(
                (input, output) => {
                    output.CreatedAtDays = (float)(input.CreatedAt - new DateTime(2000, 1, 1)).TotalDays;
                    output.PriceFloat = (float)input.Price;
                    output.OwnerIdFloat = (float)input.OwnerId;
                    output.LatitudeFloat = (float)input.Latitude;
                    output.LongitudeFloat = (float)input.Longitude;
                    output.SquareFootageFloat = input.SquareFootage;
                    output.BedroomsFloat = (float)input.Bedrooms;
                    output.BathroomsFloat = (float)input.Bathrooms;
                    output.YearBuiltFloat = (float)input.YearBuilt;
                    output.HasBalconyFloat = input.HasBalcony;
                    output.HasParkingFloat = input.HasParking;
                    output.PetsAllowedFloat = input.PetsAllowed;
                    output.PropertyAge = (float)(DateTime.Now.Year - input.YearBuilt);
                }, "DateTimeMapping"))
            .Append(_mlContext.Transforms.Concatenate("Features",
                "PriceFloat", "LatitudeFloat", "LongitudeFloat",
                "SquareFootageFloat", "BedroomsFloat", "BathroomsFloat", "YearBuiltFloat",
                "HasBalconyFloat", "HasParkingFloat", "PetsAllowedFloat", "PropertyTypeEncoded"))
            .Append(_mlContext.Transforms.NormalizeMinMax("NormalizedFeatures", "Features"));

        var knnOptions = new KMeansTrainer.Options
        {
            NumberOfClusters = Math.Min(10, _properties.Count / 5),
            MaximumNumberOfIterations = 1000,
        };

        var knnPipeline = pipeline.Append(_mlContext.Clustering.Trainers.KMeans(knnOptions));
        _knnModel = knnPipeline.Fit(dataSplit.TrainSet);

        var predictions = _knnModel.Transform(dataSplit.TestSet);

        _logger.LogInformation("KNN Model trained.");
    }

    public async Task<ModelEvaluationResults> EvaluateModelEffectivenessAsync()
    {
        await EnsureInitializedAsync();

        if (_properties.Count < 10)
        {
            _logger.LogWarning("Not enough properties to evaluate model effectiveness");
            return new ModelEvaluationResults
            {
                Success = false,
                ErrorMessage = "Not enough data for evaluation (minimum 10 properties required)"
            };
        }

        int folds = 5;
        var results = new ModelEvaluationResults
        {
            Success = true,
            Precision = 0,
            Recall = 0,
            F1Score = 0,
        };

        var random = new Random(42);
        var shuffledProperties = _properties.OrderBy(x => random.Next()).ToList();

        int foldSize = shuffledProperties.Count / folds;

        for (int i = 0; i < folds; i++)
        {
            var testSet = shuffledProperties.Skip(i * foldSize).Take(foldSize).ToList();
            var trainSet = shuffledProperties.Except(testSet).ToList();

            double foldPrecision = 0;
            double foldRecall = 0;
            double foldMAP = 0;
            double foldF1Score = 0;

            foreach (var testProperty in testSet)
            {
                var originalProperties = _properties;
                _properties = trainSet;

                var similarProperties = await FindSimilarPropertiesAsync(
                    testProperty.Price,
                    testProperty.Latitude,
                    testProperty.Longitude,
                    testProperty.SquareFootage,
                    testProperty.Bedrooms,
                    testProperty.Bathrooms,
                    testProperty.YearBuilt,
                    testProperty.HasBalcony,
                    testProperty.HasParking,
                    testProperty.PetsAllowed,
                    testProperty.PropertyType,
                    12); 

                _properties = originalProperties;

                var similarList = similarProperties.ToList();

                double precision = CalculateApproximatePrecision(testProperty, similarList);
                foldPrecision += precision;
                double recall = CalculateRecall(testProperty, similarList, originalProperties);
                foldRecall += recall;
                float f1Score = CalculateF1Score((float)precision, (float)recall);
                foldF1Score += f1Score;
            }
            results.Precision += (float)(foldPrecision / testSet.Count);
            results.Recall += (float)(foldRecall / testSet.Count);
            results.F1Score += (float)(foldF1Score / testSet.Count);
        }

        results.Precision /= folds;
        results.Recall /= folds;
        results.F1Score /= folds;

        return results;
    }

    private double CalculateApproximatePrecision(Property testProperty, List<Property> similarProperties)
    {
        if (similarProperties.Count == 0)
            return 0;

        double totalSimilarity = 0;

        foreach (var property in similarProperties)
        {
            double priceSimilarity = 1.0 - Math.Abs((double)(property.Price - testProperty.Price) / Math.Max((double)testProperty.Price, 1.0));
            double sizeSimilarity = 1.0 - Math.Abs((property.SquareFootage - testProperty.SquareFootage) / Math.Max(testProperty.SquareFootage, 1));
            double roomsSimilarity = 1.0 - (Math.Abs(property.Bedrooms - testProperty.Bedrooms) + Math.Abs(property.Bathrooms - testProperty.Bathrooms)) / 10.0;

            double distance = CalculateDistance(testProperty.Latitude, testProperty.Longitude, property.Latitude, property.Longitude);
            double locationSimilarity = Math.Max(0, 1.0 - distance / 15.0);

            double similarity = (priceSimilarity + sizeSimilarity + roomsSimilarity + locationSimilarity) / 4.0;
            totalSimilarity += similarity;
        }

        return totalSimilarity / similarProperties.Count;
    }

    private double CalculateRecall(Property testProperty, List<Property> recommendedProperties, List<Property> allProperties)
    {
        if (recommendedProperties.Count == 0)
            return 0;

        var relevantProperties = allProperties
            .Where(p => p.Id != testProperty.Id) 
            .Where(p => Math.Abs((double)(p.Price - testProperty.Price) / Math.Max((double)testProperty.Price, 1.0)) < 0.2) 
            .Where(p => Math.Abs(p.Bedrooms - testProperty.Bedrooms) <= 1) 
            .Where(p => Math.Abs(p.Bathrooms - testProperty.Bathrooms) <= 1) 
            .Where(p => CalculateDistance(p.Latitude, p.Longitude, testProperty.Latitude, testProperty.Longitude) < 15.0) 
            .Take(20)
            .ToList();

        if (relevantProperties.Count == 0)
            return 0;

        int relevantFound = recommendedProperties.Count(rp => relevantProperties.Any(relp => relp.Id == rp.Id));

        return (double)relevantFound / Math.Min(relevantProperties.Count, 12); 
    }

    private float CalculateF1Score(float precision, float recall)
    {
        if (precision <= 0 || recall <= 0)
            return 0;

        // F1 = 2 * (precision * recall) / (precision + recall)
        return 2 * (precision * recall) / (precision + recall);
    }

    public async Task<IEnumerable<Property>> FindSimilarPropertiesAsync(
    decimal price,
    double latitude,
    double longitude,
    float squareFootage,
    int bedrooms,
    float bathrooms,
    int yearBuilt,
    bool hasBalcony,
    bool hasParking,
    bool petsAllowed,
    string propertyType,
    int maxResults = 12)
    {
        await EnsureInitializedAsync();

        if (_knnModel == null)
        {
            await TrainKNNModelAsync();
        }

        var virtualProperty = new PropertyMLModel
        {
            PropertyId = -1, 
            Price = (float)price,
            OwnerId = -1, 
            CreatedAt = DateTime.UtcNow, 
            Latitude = (float)latitude,
            Longitude = (float)longitude,
            SquareFootage = squareFootage,
            Bedrooms = bedrooms,
            Bathrooms = bathrooms,
            YearBuilt = yearBuilt,
            HasBalcony = hasBalcony ? 1 : 0,
            HasParking = hasParking ? 1 : 0,
            PetsAllowed = petsAllowed ? 1 : 0,
            PropertyType = propertyType
        };

        var virtualFeatures = _predictionEngine.Predict(virtualProperty).NormalizedFeatures;

        var propertiesWithDistances = _properties
         .Select(p =>
         {
             var propertyModel = new PropertyMLModel
             {
                 PropertyId = p.Id,
                 Price = (float)p.Price,
                 OwnerId = p.OwnerId,
                 CreatedAt = p.CreatedAt,
                 Latitude = (float)p.Latitude,
                 Longitude = (float)p.Longitude,
                 SquareFootage = p.SquareFootage,
                 Bedrooms = p.Bedrooms,
                 Bathrooms = p.Bathrooms,
                 YearBuilt = p.YearBuilt,
                 HasBalcony = p.HasBalcony ? 1 : 0,
                 HasParking = p.HasParking ? 1 : 0,
                 PetsAllowed = p.PetsAllowed ? 1 : 0,
                 PropertyType = p.PropertyType
             };

             var targetFeatures = _predictionEngine.Predict(propertyModel).NormalizedFeatures;
             var distance = CalculateEuclideanDistance(virtualFeatures, targetFeatures);

             return new { Property = p, Distance = distance };
         })
         .OrderBy(x => x.Distance)
         .Take(maxResults)
         .Select(x => x.Property);

        return propertiesWithDistances.ToList();
    }

    private async Task EnsureInitializedAsync()
    {
        if (_properties.Count == 0)
        {
            await InitializeAsync();
        }
    }

    private double CalculateEuclideanDistance(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
            throw new ArgumentException("Vectors must be of the same length");

        double sum = 0;
        for (int i = 0; i < vector1.Length; i++)
        {
            double diff = vector1[i] - vector2[i];
            sum += diff * diff;
        }

        return Math.Sqrt(sum);
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6378;

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


public class PropertyMLModel
{
    public int PropertyId { get; set; }
    public float Price { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public float SquareFootage { get; set; }
    public int Bedrooms { get; set; }
    public float Bathrooms { get; set; }
    public int YearBuilt { get; set; }
    public int HasBalcony { get; set; }
    public int HasParking { get; set; }
    public int PetsAllowed { get; set; }
    public string PropertyType { get; set; }
}

public class PropertyMLFeaturesModel
{
    public float PriceFloat { get; set; }
    public float OwnerIdFloat { get; set; }
    public float CreatedAtDays { get; set; }
    public float LatitudeFloat { get; set; }
    public float LongitudeFloat { get; set; }
    public float SquareFootageFloat { get; set; }
    public float BedroomsFloat { get; set; }
    public float BathroomsFloat { get; set; }
    public float YearBuiltFloat { get; set; }
    public float HasBalconyFloat { get; set; }
    public float HasParkingFloat { get; set; }
    public float PetsAllowedFloat { get; set; }
    public float PropertyAge { get; set; }
}

public class PropertyMLPrediction
{
    public float[] NormalizedFeatures { get; set; }
}

public class PropertyCluster
{
    public string ClusterId { get; set; }
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public List<Property> Properties { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int PropertyCount { get; set; }
    public List<PropertyTypeDistribution> PropertyTypes { get; set; }
}

public class PropertyTypeDistribution
{
    public string Type { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class ModelEvaluationResults
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public float Precision { get; set; }
    public float Recall { get; set; }
    public float F1Score { get; set; }
}
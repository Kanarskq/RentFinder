using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.PropertyAggregate;

public class Property : Entity, IAggregateRoot
{
    public int Id { get; private set; }
    public int OwnerId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal Price { get; private set; }
    public string Status { get; private set; }
    public int Bedrooms { get; private set; }
    public int Bathrooms { get; private set; }
    public float SquareFootage { get; private set; }
    public bool HasBalcony { get; private set; }
    public bool HasParking { get; private set; }
    public bool PetsAllowed { get; private set; }
    public string PropertyType { get; private set; } 
    public int YearBuilt { get; private set; }
    private readonly List<PropertyImage> _images;
    public IReadOnlyCollection<PropertyImage> Images => _images;

    private Property()
    {
        _images = new List<PropertyImage>();
    }

    public Property(
        int ownerId,
        string title,
        string description,
        double latitude,
        double longitude,
        decimal price,
        int bedrooms,
        int bathrooms,
        float squareFootage,
        bool hasBalcony,
        bool hasParking,
        bool petsAllowed,
        string propertyType,
        int yearBuilt)
    {
        OwnerId = ownerId;
        Title = title;
        Description = description;
        Latitude = latitude;
        Longitude = longitude;
        Price = price;
        CreatedAt = DateTime.UtcNow;
        Status = PropertyStatus.Available;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        SquareFootage = squareFootage;
        HasBalcony = hasBalcony;
        HasParking = hasParking;
        PetsAllowed = petsAllowed;
        PropertyType = propertyType;
        YearBuilt = yearBuilt;

        _images = new List<PropertyImage>();
    }

    public void UpdateDetails(
        string title,
        string description,
        decimal price,
        int bedrooms,
        int bathrooms,
        float squareFootage,
        bool hasBalcony,
        bool hasParking,
        bool petsAllowed,
        string propertyType,
        int yearBuilt)
    {
        Title = title;
        Description = description;
        Price = price;
        Bedrooms = bedrooms;
        Bathrooms = bathrooms;
        SquareFootage = squareFootage;
        HasBalcony = hasBalcony;
        HasParking = hasParking;
        PetsAllowed = petsAllowed;
        PropertyType = propertyType;
        YearBuilt = yearBuilt;
    }

    public void UpdateLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public PropertyImage AddImage(byte[] imageData, string imageType, string caption)
    {
        if (_images.Count >= 3)
        {
            throw new InvalidOperationException("Property cannot have more than 3 images");
        }

        var image = new PropertyImage(Id, imageData, imageType, caption);
        _images.Add(image);
        return image;
    }

    public void RemoveImage(int imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image != null)
        {
            _images.Remove(image);
        }
    }

    public void MarkAsBooked()
    {
        Status = PropertyStatus.Booked;
    }

    public void MarkAsAvailable()
    {
        Status = PropertyStatus.Available;
    }

    public void MarkAsUnavailable()
    {
        Status = PropertyStatus.Unavailable;
    }
}

public static class PropertyStatus
{
    public const string Available = "Available";
    public const string Booked = "Booked";
    public const string Unavailable = "Unavailable";

    public static readonly IReadOnlyList<string> AllStatuses = new[] { Available, Booked, Unavailable };
}
using Bookings.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Domain.AggregatesModel.PropertyAggregate;

public class PropertyImage : Entity
{
    public int Id { get; private set; }
    public int PropertyId { get; private set; }
    public byte[] ImageData { get; private set; } // Store image as byte array
    public string ImageType { get; private set; } // Store the content type (e.g., "image/jpeg")
    public string Caption { get; private set; }
    public DateTime UploadedAt { get; private set; }

    private PropertyImage() { }

    public PropertyImage(int propertyId, byte[] imageData, string imageType, string caption)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        PropertyId = propertyId;
        ImageData = imageData;
        ImageType = imageType;
        Caption = caption;
        UploadedAt = DateTime.UtcNow;
    }

    public void UpdateImage(byte[] imageData, string imageType)
    {
        if (imageData == null || imageData.Length == 0)
        {
            throw new ArgumentException("Image data cannot be null or empty", nameof(imageData));
        }

        ImageData = imageData;
        ImageType = imageType;
        UploadedAt = DateTime.UtcNow; // Update the timestamp when image is updated
    }

    public void UpdateCaption(string caption)
    {
        Caption = caption;
    }
}

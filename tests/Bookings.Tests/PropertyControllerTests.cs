using Bookings.Api.Application.Commands.Properties;
using Bookings.Api.Application.Queries.Properties;
using Bookings.Api.Controllers;
using Bookings.Api.Controllers.Request.Properties;
using Bookings.Api.Infrastructure.Services.Properties;
using Bookings.Api.Infrastructure.Services.Search;
using Bookings.Domain.AggregatesModel.PropertyAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bookings.Tests
{
    [TestFixture]
    public class PropertyControllerTests
    {
        private Mock<ILogger<PropertyController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<IPropertyQueries> _queriesMock;
        private Mock<PropertySearchEngine> _searchEngineMock;
        private Mock<ILogger<PropertyServices>> _serviceLoggerMock;
        private PropertyServices _propertyServices;
        private PropertyController _controller;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<PropertyController>>();
            _mediatorMock = new Mock<IMediator>();
            _queriesMock = new Mock<IPropertyQueries>();
            _serviceLoggerMock = new Mock<ILogger<PropertyServices>>();
            _searchEngineMock = new Mock<PropertySearchEngine>(MockBehavior.Loose,
                Mock.Of<ILogger<PropertySearchEngine>>(),
                Mock.Of<IPropertyRepository>());

            _propertyServices = new PropertyServices(
                _serviceLoggerMock.Object,
                _mediatorMock.Object,
                _queriesMock.Object,
                _searchEngineMock.Object);

            _controller = new PropertyController(
                _loggerMock.Object,
                _mediatorMock.Object,
                _propertyServices,
                _queriesMock.Object,
                _searchEngineMock.Object);
        }

        [Test]
        public async Task GetPropertyById_WhenPropertyExists_ReturnsOkResultWithProperty()
        {
            int propertyId = 1;
            var property = new Property(
                1, "Test Property", "Description",
                40.7128, -74.0060, 1500.0m,
                2, 1, 850, true, true, false,
                "Apartment", 2015);

            _queriesMock
                .Setup(q => q.GetPropertyByIdAsync(propertyId))
                .ReturnsAsync(property);

            var result = await _controller.GetPropertyById(propertyId);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(property, okResult.Value);
        }

        [Test]
        public async Task GetPropertyById_WhenPropertyDoesNotExist_ReturnsNotFoundResult()
        {
            int propertyId = 1;

            _queriesMock
                .Setup(q => q.GetPropertyByIdAsync(propertyId))
                .ReturnsAsync((Property)null);

            var result = await _controller.GetPropertyById(propertyId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetAllProperties_WhenPropertiesExist_ReturnsOkResultWithProperties()
        {
            var properties = new List<Property>
            {
                new Property(1, "Property 1", "Description 1", 40.7128, -74.0060, 1000m, 1, 1, 600, false, true, true, "Apartment", 2010),
                new Property(2, "Property 2", "Description 2", 41.8781, -87.6298, 2000m, 2, 2, 1200, true, true, false, "House", 2015)
            };

            _queriesMock
                .Setup(q => q.GetAllPropertiesAsync())
                .ReturnsAsync(properties);

            var result = await _controller.GetAllProperties();

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(properties, okResult.Value);
        }

        [Test]
        public async Task GetAllProperties_WhenNoPropertiesExist_ReturnsNotFoundResult()
        {
            _queriesMock
                .Setup(q => q.GetAllPropertiesAsync())
                .ReturnsAsync(new List<Property>());

            var result = await _controller.GetAllProperties();

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task UpdateProperty_WhenPropertyDoesNotExist_ReturnsNotFoundResult()
        {
            int propertyId = 1;
            var request = new UpdatePropertyRequest(
                Title: "Updated Property",
                Description: "Updated description",
                Price: 1800.0m,
                Bedrooms: 3,
                Bathrooms: 2,
                SquareFootage: 950,
                HasBalcony: false,
                HasParking: true,
                PetsAllowed: true,
                PropertyType: "Apartment",
                YearBuilt: 2015
            );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdatePropertyCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.UpdateProperty(propertyId, request);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task MarkPropertyAsAvailable_WhenPropertyExists_ReturnsOkResult()
        {
            int propertyId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MarkPropertyAsAvailableCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.MarkPropertyAsAvailable(propertyId);

            Assert.IsInstanceOf<OkResult>(result);

            _mediatorMock.Verify(m => m.Send(
                It.Is<MarkPropertyAsAvailableCommand>(cmd => cmd.PropertyId == propertyId),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task MarkPropertyAsAvailable_WhenPropertyDoesNotExist_ReturnsNotFoundResult()
        {
            int propertyId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MarkPropertyAsAvailableCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.MarkPropertyAsAvailable(propertyId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task MarkPropertyAsUnavailable_WhenPropertyExists_ReturnsOkResult()
        {
            int propertyId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MarkPropertyAsUnavailableCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.MarkPropertyAsUnavailable(propertyId);

            Assert.IsInstanceOf<OkResult>(result);

            _mediatorMock.Verify(m => m.Send(
                It.Is<MarkPropertyAsUnavailableCommand>(cmd => cmd.PropertyId == propertyId),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task MarkPropertyAsUnavailable_WhenPropertyDoesNotExist_ReturnsNotFoundResult()
        {
            int propertyId = 1;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MarkPropertyAsUnavailableCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.MarkPropertyAsUnavailable(propertyId);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task AddPropertyImage_WhenImageIsValid_ReturnsOkResult()
        {
            int propertyId = 1;
            var fileMock = new Mock<IFormFile>();
            var content = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });

            fileMock.Setup(f => f.Length).Returns(content.Length);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<Stream, CancellationToken>((stream, token) =>
                {
                    content.CopyTo(stream);
                    stream.Flush();
                    content.Position = 0;
                })
                .Returns(Task.CompletedTask);

            var request = new AddPropertyImageRequest
            {
                Image = fileMock.Object,
                Caption = "Test Image"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AddPropertyImageCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.AddPropertyImage(propertyId, request);

            Assert.IsInstanceOf<OkResult>(result);

            _mediatorMock.Verify(m => m.Send(
                It.Is<AddPropertyImageCommand>(cmd =>
                    cmd.PropertyId == propertyId &&
                    cmd.Caption == request.Caption &&
                    cmd.ImageType == "image/jpeg"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task AddPropertyImage_WhenImageIsEmpty_ReturnsBadRequestResult()
        {
            int propertyId = 1;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var request = new AddPropertyImageRequest
            {
                Image = fileMock.Object,
                Caption = "Empty Image"
            };

            var result = await _controller.AddPropertyImage(propertyId, request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task AddPropertyImage_WhenImageIsTooLarge_ReturnsBadRequestResult()
        {
            int propertyId = 1;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(6 * 1024 * 1024); // 6MB, exceeds 5MB limit

            var request = new AddPropertyImageRequest
            {
                Image = fileMock.Object,
                Caption = "Large Image"
            };

            var result = await _controller.AddPropertyImage(propertyId, request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task AddPropertyImage_WhenImageTypeIsInvalid_ReturnsBadRequestResult()
        {
            int propertyId = 1;
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1000);
            fileMock.Setup(f => f.ContentType).Returns("application/pdf"); // Invalid image type

            var request = new AddPropertyImageRequest
            {
                Image = fileMock.Object,
                Caption = "PDF Document"
            };

            var result = await _controller.AddPropertyImage(propertyId, request);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetPropertiesByStatus_WhenPropertiesExistWithStatus_ReturnsOkResultWithProperties()
        {
            string status = PropertyStatus.Available;
            var properties = new List<Property>
            {
                new Property(1, "Luxury Apartment", "Nice apartment", 40.7128, -74.0060, 1500m, 2, 1, 850, true, true, false, "Apartment", 2015),
                new Property(2, "Beach House", "Beautiful house", 25.7617, -80.1918, 3000m, 4, 3, 2000, true, true, true, "House", 2010)
            };

            _queriesMock
                .Setup(q => q.GetPropertiesByStatusAsync(status))
                .ReturnsAsync(properties);

            var result = await _controller.GetPropertiesByStatus(status);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(properties, okResult.Value);
        }

        [Test]
        public async Task GetPropertyImage_WhenImageExists_ReturnsFileResultWithImage()
        {
            int propertyId = 1;
            var imageData = new byte[] { 0x01, 0x02, 0x03 };
            var imageType = "image/jpeg";
            var image = new PropertyImage(propertyId, imageData, imageType, "Living Room");

            _queriesMock
                .Setup(q => q.GetPropertyImageAsync(propertyId))
                .ReturnsAsync(image);

            var result = await _controller.GetPropertyImage(propertyId);

            Assert.IsInstanceOf<FileContentResult>(result);
            var fileResult = result as FileContentResult;
            Assert.AreEqual(imageData, fileResult.FileContents);
            Assert.AreEqual(imageType, fileResult.ContentType);
        }
    }
}
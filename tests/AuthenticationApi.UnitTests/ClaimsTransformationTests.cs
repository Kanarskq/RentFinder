using Authentication.Api;
using Authetication.Api.Models;
using Authetication.Api.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.UnitTests;

[TestFixture]
public class ClaimsTransformationTests
{
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<IUserService> _mockUserService;
    private ClaimsTransformation _claimsTransformation;

    [SetUp]
    public void Setup()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockUserService = new Mock<IUserService>();
        _claimsTransformation = new ClaimsTransformation(_mockHttpContextAccessor.Object, _mockUserService.Object);
    }

    [Test]
    public async Task TransformAsync_UnauthenticatedUser_ReturnsSamePrincipal()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await _claimsTransformation.TransformAsync(principal);

        // Assert
        Assert.AreEqual(principal, result);
    }

    [Test]
    public async Task TransformAsync_AuthenticatedUserWithoutNameIdentifier_ReturnsSamePrincipal()
    {
        // Arrange
        var identity = new ClaimsIdentity(new List<Claim>(), "test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var result = await _claimsTransformation.TransformAsync(principal);

        // Assert
        Assert.AreEqual(principal, result);
    }

    [Test]
    public async Task TransformAsync_ExistingUser_AddsRoleClaim()
    {
        // Arrange
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "auth0|123"),
        new Claim(ClaimTypes.Name, "Test User"),
        new Claim(ClaimTypes.Email, "test@example.com"),
        new Claim("email_verified", "true")
    };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        var existingUser = new User
        {
            Auth0Id = "auth0|123",
            Name = "Test User",
            Email = "test@example.com",
            Role = UserRoles.Tenant
        };

        _mockUserService.Setup(s => s.GetUserByAuth0IdAsync("auth0|123"))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _claimsTransformation.TransformAsync(principal);

        // Assert
        Assert.IsTrue(result.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserRoles.Tenant));
    }

    [Test]
    public async Task TransformAsync_NewUser_CreatesUserAndAddsRoleClaim()
    {
        // Arrange
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "auth0|new"),
        new Claim(ClaimTypes.Name, "New User"),
        new Claim(ClaimTypes.Email, "new@example.com"),
        new Claim("email_verified", "true")
    };

        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _mockUserService.Setup(s => s.GetUserByAuth0IdAsync("auth0|new"))
            .ReturnsAsync((User)null);

        _mockUserService.Setup(s => s.CreateOrUpdateUserAsync(It.IsAny<User>()))
            .ReturnsAsync(new User
            {
                Auth0Id = "auth0|new",
                Name = "New User",
                Email = "new@example.com",
                Role = UserRoles.Tenant
            });

        // Act
        var result = await _claimsTransformation.TransformAsync(principal);

        // Assert
        _mockUserService.Verify(s => s.CreateOrUpdateUserAsync(It.IsAny<User>()), Times.Once);
        Assert.IsTrue(result.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == UserRoles.Tenant));
    }
}
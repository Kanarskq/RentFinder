using Authentication.Api;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.UnitTests;

[TestFixture]
public class HasScopeHandlerTests
{
    private HasScopeHandler _handler;
    private HasScopeRequirement _requirement;

    [SetUp]
    public void Setup()
    {
        _requirement = new HasScopeRequirement("read:messages");
        _handler = new HasScopeHandler();
    }

    [Test]
    public async Task HandleRequirementAsync_NoScopeClaim_DoesNotSucceed()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()));
        var context = new AuthorizationHandlerContext(new[] { _requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.IsFalse(context.HasSucceeded);
    }

    [Test]
    public async Task HandleRequirementAsync_HasMatchingScope_Succeeds()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("scope", "read:messages write:messages")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(new[] { _requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.IsTrue(context.HasSucceeded);
    }

    [Test]
    public async Task HandleRequirementAsync_NoMatchingScope_DoesNotSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("scope", "write:messages delete:messages")
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var context = new AuthorizationHandlerContext(new[] { _requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.IsFalse(context.HasSucceeded);
    }
}

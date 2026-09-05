namespace CampaignSaaS.UnitTests.Identity;

using CampaignSaaS.Modules.Identity.Infrastructure.Services;
using FluentAssertions;
using Xunit;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut = new();

    [Fact]
    public void HashPassword_ShouldReturnFormattedHash()
    {
        // Act
        var hash = _sut.HashPassword("MySecretPassword123!");

        // Assert
        hash.Should().NotBeNullOrWhiteSpace();
        hash.Split(':').Should().HaveCount(4);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "StrongPassword#2026";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "CorrectPassword123!";
        var hash = _sut.HashPassword(password);

        // Act
        var result = _sut.VerifyPassword("WrongPassword123!", hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithInvalidHashFormat_ShouldReturnFalse()
    {
        // Act
        var result = _sut.VerifyPassword("Password123!", "invalid_hash_string");

        // Assert
        result.Should().BeFalse();
    }
}

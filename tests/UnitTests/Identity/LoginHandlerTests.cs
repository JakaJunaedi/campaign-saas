namespace CampaignSaaS.UnitTests.Identity;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Application.Commands.Login;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class LoginHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IOrganizationRepository _orgRepo = Substitute.For<IOrganizationRepository>();
    private readonly IRefreshTokenRepository _refreshRepo = Substitute.For<IRefreshTokenRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly IIdentityUnitOfWork _uow = Substitute.For<IIdentityUnitOfWork>();
    private readonly LoginCommandHandler _sut;

    public LoginHandlerTests()
    {
        _sut = new LoginCommandHandler(
            _userRepo,
            _orgRepo,
            _refreshRepo,
            _hasher,
            _jwt,
            _uow);

        _jwt.AccessTokenExpirationMinutes.Returns(15);
        _jwt.GenerateRefreshToken().Returns(("new_refresh_token", DateTimeOffset.UtcNow.AddDays(7)));
        _jwt.GenerateAccessToken(Arg.Any<User>(), Arg.Any<Organization>()).Returns("new_jwt_access_token");
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnAuthResult()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var user = User.Create(orgId, "user@test.com", "valid_hash", "Test User", UserRole.AgencyOwner);
        var org = Organization.Create("Test Org", "test-org", OrganizationStatus.Active);

        _userRepo.GetByEmailGlobalAsync("user@test.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.VerifyPassword("Password123!", "valid_hash").Returns(true);
        _orgRepo.GetByIdAsync(orgId, Arg.Any<CancellationToken>()).Returns(org);

        var command = new LoginCommand("user@test.com", "Password123!");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().Be("new_jwt_access_token");
        result.Value.RefreshToken.Should().Be("new_refresh_token");
        result.Value.User.Email.Should().Be("user@test.com");
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var user = User.Create(orgId, "user@test.com", "valid_hash", "Test User", UserRole.AgencyOwner);

        _userRepo.GetByEmailGlobalAsync("user@test.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.VerifyPassword("WrongPassword", "valid_hash").Returns(false);

        var command = new LoginCommand("user@test.com", "WrongPassword");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Auth.InvalidCredentials");
    }

    [Fact]
    public async Task Handle_WithSuspendedOrg_ShouldReturnUnauthorized()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var user = User.Create(orgId, "user@test.com", "valid_hash", "Test User", UserRole.AgencyOwner);
        var suspendedOrg = Organization.Create("Suspended Org", "suspended-org", OrganizationStatus.Suspended);

        _userRepo.GetByEmailGlobalAsync("user@test.com", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.VerifyPassword("Password123!", "valid_hash").Returns(true);
        _orgRepo.GetByIdAsync(orgId, Arg.Any<CancellationToken>()).Returns(suspendedOrg);

        var command = new LoginCommand("user@test.com", "Password123!");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Auth.OrganizationSuspended");
    }
}

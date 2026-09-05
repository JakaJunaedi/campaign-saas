namespace CampaignSaaS.UnitTests.Identity;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Application.Commands.RefreshToken;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class RefreshTokenHandlerTests
{
    private readonly IRefreshTokenRepository _refreshRepo = Substitute.For<IRefreshTokenRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IOrganizationRepository _orgRepo = Substitute.For<IOrganizationRepository>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly IIdentityUnitOfWork _uow = Substitute.For<IIdentityUnitOfWork>();
    private readonly RefreshTokenCommandHandler _sut;

    public RefreshTokenHandlerTests()
    {
        _sut = new RefreshTokenCommandHandler(
            _refreshRepo,
            _userRepo,
            _orgRepo,
            _jwt,
            _uow);

        _jwt.AccessTokenExpirationMinutes.Returns(15);
        _jwt.GenerateRefreshToken().Returns(("new_rotated_refresh_token", DateTimeOffset.UtcNow.AddDays(7)));
        _jwt.GenerateAccessToken(Arg.Any<User>(), Arg.Any<Organization>()).Returns("new_jwt_access_token");
    }

    [Fact]
    public async Task Handle_WithValidActiveToken_ShouldRotateAndReturnNewTokens()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var oldToken = RefreshToken.Create(orgId, userId, "existing_valid_token", DateTimeOffset.UtcNow.AddDays(3));
        var user = User.Create(orgId, "user@test.com", "hash", "Test User", UserRole.AgencyOwner);
        var org = Organization.Create("Test Org", "test-org", OrganizationStatus.Active);

        _refreshRepo.GetByTokenAsync("existing_valid_token", Arg.Any<CancellationToken>()).Returns(oldToken);
        _userRepo.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
        _orgRepo.GetByIdAsync(orgId, Arg.Any<CancellationToken>()).Returns(org);

        var command = new RefreshTokenCommand("existing_valid_token");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().Be("new_jwt_access_token");
        result.Value.RefreshToken.Should().Be("new_rotated_refresh_token");

        oldToken.IsRevoked.Should().BeTrue();
        oldToken.ReplacedByToken.Should().Be("new_rotated_refresh_token");

        _refreshRepo.Received(1).Update(oldToken);
        await _refreshRepo.Received(1).AddAsync(Arg.Is<RefreshToken>(r => r.Token == "new_rotated_refresh_token"), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExpiredToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expiredToken = RefreshToken.Create(orgId, userId, "expired_token", DateTimeOffset.UtcNow.AddDays(-1));

        _refreshRepo.GetByTokenAsync("expired_token", Arg.Any<CancellationToken>()).Returns(expiredToken);

        var command = new RefreshTokenCommand("expired_token");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Auth.InvalidRefreshToken");
    }

    [Fact]
    public async Task Handle_WithRevokedToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var revokedToken = RefreshToken.Create(orgId, userId, "revoked_token", DateTimeOffset.UtcNow.AddDays(3));
        revokedToken.Revoke();

        _refreshRepo.GetByTokenAsync("revoked_token", Arg.Any<CancellationToken>()).Returns(revokedToken);

        var command = new RefreshTokenCommand("revoked_token");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Auth.InvalidRefreshToken");
    }
}

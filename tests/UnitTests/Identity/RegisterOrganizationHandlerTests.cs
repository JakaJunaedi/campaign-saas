namespace CampaignSaaS.UnitTests.Identity;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Application.Commands.RegisterOrganization;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class RegisterOrganizationHandlerTests
{
    private readonly IOrganizationRepository _orgRepo = Substitute.For<IOrganizationRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshRepo = Substitute.For<IRefreshTokenRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly IIdentityUnitOfWork _uow = Substitute.For<IIdentityUnitOfWork>();
    private readonly RegisterOrganizationCommandHandler _sut;

    public RegisterOrganizationHandlerTests()
    {
        _sut = new RegisterOrganizationCommandHandler(
            _orgRepo,
            _userRepo,
            _refreshRepo,
            _hasher,
            _jwt,
            _uow);

        _jwt.AccessTokenExpirationMinutes.Returns(15);
        _jwt.GenerateRefreshToken().Returns(("sample_refresh_token", DateTimeOffset.UtcNow.AddDays(7)));
        _jwt.GenerateAccessToken(Arg.Any<User>(), Arg.Any<Organization>()).Returns("sample_jwt_access_token");
        _hasher.HashPassword(Arg.Any<string>()).Returns("hashed_password_sample");
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateOrgAndAdmin()
    {
        // Arrange
        var command = new RegisterOrganizationCommand(
            "Star Media Agency",
            "star-media",
            "John Doe",
            "john@starmedia.com",
            "Password123!");

        _orgRepo.ExistsBySlugAsync("star-media", Arg.Any<CancellationToken>()).Returns(false);
        _userRepo.GetByEmailGlobalAsync("john@starmedia.com", Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.AccessToken.Should().Be("sample_jwt_access_token");
        result.Value.RefreshToken.Should().Be("sample_refresh_token");
        result.Value.User.Email.Should().Be("john@starmedia.com");
        result.Value.User.Role.Should().Be(UserRole.AgencyOwner.ToString());
        result.Value.Organization.Slug.Should().Be("star-media");

        await _orgRepo.Received(1).AddAsync(Arg.Is<Organization>(o => o.Slug == "star-media"), Arg.Any<CancellationToken>());
        await _userRepo.Received(1).AddAsync(Arg.Is<User>(u => u.Email == "john@starmedia.com"), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateSlug_ShouldReturnConflict()
    {
        // Arrange
        var command = new RegisterOrganizationCommand(
            "Star Media Agency",
            "star-media",
            "John Doe",
            "john@starmedia.com",
            "Password123!");

        _orgRepo.ExistsBySlugAsync("star-media", Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Organization.DuplicateSlug");
        await _orgRepo.DidNotReceive().AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnConflict()
    {
        // Arrange
        var command = new RegisterOrganizationCommand(
            "Star Media Agency",
            "star-media",
            "John Doe",
            "john@starmedia.com",
            "Password123!");

        var existingUser = User.Create(Guid.NewGuid(), "john@starmedia.com", "hash", "Existing", UserRole.AgencyOwner);

        _orgRepo.ExistsBySlugAsync("star-media", Arg.Any<CancellationToken>()).Returns(false);
        _userRepo.GetByEmailGlobalAsync("john@starmedia.com", Arg.Any<CancellationToken>()).Returns(existingUser);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("User.DuplicateEmail");
    }
}

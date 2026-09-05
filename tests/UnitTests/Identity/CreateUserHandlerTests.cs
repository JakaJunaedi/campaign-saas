namespace CampaignSaaS.UnitTests.Identity;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Application.Commands.CreateUser;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using FluentAssertions;
using NSubstitute;
using Xunit;

public class CreateUserHandlerTests
{
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ICurrentTenantContext _tenantContext = Substitute.For<ICurrentTenantContext>();
    private readonly IIdentityUnitOfWork _uow = Substitute.For<IIdentityUnitOfWork>();
    private readonly CreateUserCommandHandler _sut;

    public CreateUserHandlerTests()
    {
        _sut = new CreateUserCommandHandler(
            _userRepo,
            _hasher,
            _tenantContext,
            _uow);

        _hasher.HashPassword(Arg.Any<string>()).Returns("hashed_pwd");
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateUser()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _userRepo.ExistsByEmailAsync(orgId, "manager@test.com", Arg.Any<CancellationToken>()).Returns(false);

        var command = new CreateUserCommand("Jane Doe", "manager@test.com", "Password123!", "CampaignManager");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Email.Should().Be("manager@test.com");
        result.Value.Role.Should().Be("CampaignManager");
        result.Value.OrganizationId.Should().Be(orgId);

        await _userRepo.Received(1).AddAsync(Arg.Is<User>(u => u.Email == "manager@test.com" && u.OrganizationId == orgId), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutTenantContext_ShouldReturnForbidden()
    {
        // Arrange
        _tenantContext.OrganizationId.Returns((Guid?)null);
        var command = new CreateUserCommand("Jane Doe", "manager@test.com", "Password123!", "CampaignManager");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("Tenant.Required");
    }

    [Fact]
    public async Task Handle_WithDuplicateEmailInOrg_ShouldReturnConflict()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        _tenantContext.OrganizationId.Returns(orgId);
        _userRepo.ExistsByEmailAsync(orgId, "manager@test.com", Arg.Any<CancellationToken>()).Returns(true);

        var command = new CreateUserCommand("Jane Doe", "manager@test.com", "Password123!", "CampaignManager");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("User.DuplicateEmail");
    }
}

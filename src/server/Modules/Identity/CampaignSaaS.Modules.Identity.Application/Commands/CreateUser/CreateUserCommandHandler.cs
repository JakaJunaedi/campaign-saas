namespace CampaignSaaS.Modules.Identity.Application.Commands.CreateUser;

using CampaignSaaS.Modules.Identity.Application.Abstractions;
using CampaignSaaS.Modules.Identity.Contracts.DTOs;
using CampaignSaaS.Modules.Identity.Domain.Entities;
using CampaignSaaS.Modules.Identity.Domain.Enums;
using CampaignSaaS.SharedKernel.MultiTenancy;
using ErrorOr;
using MediatR;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentTenantContext _tenantContext;
    private readonly IIdentityUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ICurrentTenantContext tenantContext,
        IIdentityUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            return Error.Forbidden("Tenant.Required", "Active organization context is required to create a user.");
        }

        var organizationId = _tenantContext.OrganizationId.Value;
        var normalizedEmail = request.Email.ToLowerInvariant().Trim();

        if (await _userRepository.ExistsByEmailAsync(organizationId, normalizedEmail, cancellationToken))
        {
            return Error.Conflict("User.DuplicateEmail", $"User with email '{normalizedEmail}' already exists in this organization.");
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            return Error.Validation("User.InvalidRole", $"Role '{request.Role}' is invalid.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(organizationId, normalizedEmail, passwordHash, request.FullName, role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.OrganizationId,
            user.Email,
            user.FullName,
            user.Role.ToString(),
            user.IsActive,
            user.CreatedAt);
    }
}

namespace CampaignSaaS.ArchitectureTests;

using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

public class ModuleBoundaryTests
{
    [Fact]
    public void DomainLayer_ShouldNot_DependOn_InfrastructureLayer()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.SharedKernel.Domain.IDomainEvent).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Domain layer must not depend on Entity Framework Core or Infrastructure.");
    }

    [Fact]
    public void SharedKernel_ShouldNot_DependOn_AnySpecificModule()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.SharedKernel.Domain.IDomainEvent).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("CampaignSaaS.Modules")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("SharedKernel must not reference any specific module.");
    }

    [Fact]
    public void IdentityDomain_ShouldNot_DependOn_InfrastructureOrApplication()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Identity.Domain.Entities.User).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Identity.Infrastructure",
                "CampaignSaaS.Modules.Identity.Application",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Identity Domain must not depend on Infrastructure, Application, or EF Core.");
    }

    [Fact]
    public void IdentityApplication_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Identity.Application.Commands.Login.LoginCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Identity.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Identity Application must not depend on Infrastructure or EF Core.");
    }

    [Fact]
    public void ClientDomain_ShouldNot_DependOn_InfrastructureOrApplication()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Client.Domain.Entities.Client).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Client.Infrastructure",
                "CampaignSaaS.Modules.Client.Application",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Client Domain must not depend on Infrastructure, Application, or EF Core.");
    }

    [Fact]
    public void ClientApplication_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Client.Application.Commands.CreateClient.CreateClientCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Client.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Client Application must not depend on Infrastructure or EF Core.");
    }

    [Fact]
    public void ClientModule_ShouldNot_DependOn_IdentityDbContext()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Client.Infrastructure.Persistence.ClientDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("CampaignSaaS.Modules.Identity.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Client module must not reference Identity DbContext or Infrastructure directly.");
    }

    [Fact]
    public void CreatorDomain_ShouldNot_DependOn_InfrastructureOrApplication()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Creator.Domain.Entities.Creator).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Creator.Infrastructure",
                "CampaignSaaS.Modules.Creator.Application",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Creator Domain must not depend on Infrastructure, Application, or EF Core.");
    }

    [Fact]
    public void CreatorApplication_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Creator.Application.Commands.CreateCreator.CreateCreatorCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Creator.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Creator Application must not depend on Infrastructure or EF Core.");
    }

    [Fact]
    public void CreatorModule_ShouldNot_DependOn_OtherModuleDbContexts()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Creator.Infrastructure.Persistence.CreatorDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Identity.Infrastructure",
                "CampaignSaaS.Modules.Client.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Creator module must not reference other module DbContexts directly.");
    }

    [Fact]
    public void CampaignDomain_ShouldNot_DependOn_InfrastructureOrApplication()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Campaign.Domain.Entities.Campaign).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Campaign.Infrastructure",
                "CampaignSaaS.Modules.Campaign.Application",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Campaign Domain must not depend on Infrastructure, Application, or EF Core.");
    }

    [Fact]
    public void CampaignApplication_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Campaign.Application.Commands.CreateCampaign.CreateCampaignCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Campaign.Infrastructure",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Campaign Application must not depend on Infrastructure or EF Core.");
    }

    [Fact]
    public void CampaignModule_ShouldNot_DependOn_OtherModuleDbContexts()
    {
        var result = Types.InAssembly(typeof(CampaignSaaS.Modules.Campaign.Infrastructure.Persistence.CampaignDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "CampaignSaaS.Modules.Identity.Infrastructure",
                "CampaignSaaS.Modules.Client.Infrastructure",
                "CampaignSaaS.Modules.Creator.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue("Campaign module must not reference other module DbContexts directly.");
    }
}

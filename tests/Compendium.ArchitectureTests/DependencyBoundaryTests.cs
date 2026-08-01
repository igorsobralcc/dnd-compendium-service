using System.Reflection;
using Compendium.Application.Equipment;
using Compendium.Domain.Equipment;
using Compendium.Infra.Persistence;

namespace Compendium.ArchitectureTests;

public sealed class DependencyBoundaryTests
{
    [Fact]
    public void Domain_has_no_project_or_external_package_dependencies()
    {
        Assert.Empty(RepositoryLayout.ProjectReferences("Compendium.Domain"));
        Assert.Empty(RepositoryLayout.PackageReferences("Compendium.Domain"));

        Assert.DoesNotContain(
            typeof(EquipmentItem).Assembly.GetReferencedAssemblies(),
            IsForbiddenFrameworkReference);
    }

    [Fact]
    public void Application_depends_only_on_domain_and_not_on_frameworks()
    {
        Assert.Equal(
            ["Compendium.Domain"],
            RepositoryLayout.ProjectReferences("Compendium.Application"));
        Assert.Empty(RepositoryLayout.PackageReferences("Compendium.Application"));

        Assert.DoesNotContain(
            typeof(CreateEquipmentItemUseCase).Assembly.GetReferencedAssemblies(),
            IsForbiddenFrameworkReference);
    }

    [Fact]
    public void Infrastructure_depends_on_application_ports()
    {
        var infrastructureTypes = typeof(CompendiumDbContext).Assembly.GetTypes();
        var applicationPorts = typeof(IEquipmentRepository).Assembly
            .GetTypes()
            .Where(type => type.IsInterface && type.Name.EndsWith("Repository"))
            .ToArray();

        Assert.NotEmpty(applicationPorts);
        Assert.All(
            applicationPorts,
            port => Assert.Contains(
                infrastructureTypes,
                type => !type.IsAbstract && port.IsAssignableFrom(type)));
    }

    [Fact]
    public void Api_has_no_direct_infrastructure_project_reference()
    {
        Assert.DoesNotContain(
            "Compendium.Infra",
            RepositoryLayout.ProjectReferences("Compendium.API"));
        Assert.DoesNotContain(
            typeof(Program).Assembly.GetReferencedAssemblies(),
            reference => reference.Name == "Compendium.Infra");
    }

    private static bool IsForbiddenFrameworkReference(AssemblyName reference) =>
        reference.Name is not null
        && (reference.Name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)
            || reference.Name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
            || reference.Name.StartsWith("Microsoft.Extensions.DependencyInjection", StringComparison.Ordinal));
}

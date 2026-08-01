using System.Reflection;

namespace Compendium.ArchitectureTests;

public sealed class ApiConventionTests
{
    [Fact]
    public void Every_concrete_controller_follows_mvc_and_dependency_conventions()
    {
        var controllers = typeof(Program).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && IsController(type))
            .ToArray();

        Assert.NotEmpty(controllers);
        Assert.All(controllers, controller =>
        {
            Assert.EndsWith("Controller", controller.Name);
            Assert.True(HasApiControllerAttribute(controller));

            var dependencies = controller
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany(constructor => constructor.GetParameters())
                .Select(parameter => parameter.ParameterType.Namespace ?? string.Empty);

            Assert.DoesNotContain(
                dependencies,
                dependency => dependency.StartsWith(
                    "Compendium.Infra",
                    StringComparison.Ordinal));
        });
    }

    [Fact]
    public void Application_source_contains_no_minimal_api_route_handlers()
    {
        var violations = FindMinimalApiRegistrations(
            Directory.EnumerateFiles(
                Path.Combine(RepositoryLayout.Root, "src", "Compendium.API"),
                "*.cs",
                SearchOption.AllDirectories)
                .Where(path => !path.Contains(
                    $"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}",
                    StringComparison.Ordinal)));

        Assert.Empty(violations);
    }

    [Theory]
    [InlineData("app.MapGet(\"/legacy\", Handler);")]
    [InlineData("routes.MapPost(\"/legacy\", Handler);")]
    [InlineData("app.MapGroup(\"/legacy\");")]
    public void Minimal_api_rule_detects_a_temporary_violation(string source)
    {
        var temporaryFile = Path.GetTempFileName();

        try
        {
            File.WriteAllText(temporaryFile, source);
            Assert.Single(FindMinimalApiRegistrations([temporaryFile]));
        }
        finally
        {
            File.Delete(temporaryFile);
        }
    }

    private static IReadOnlyCollection<string> FindMinimalApiRegistrations(
        IEnumerable<string> files)
    {
        string[] forbiddenCalls =
        [
            ".MapGet(",
            ".MapPost(",
            ".MapPut(",
            ".MapDelete(",
            ".MapPatch(",
            ".MapGroup("
        ];

        return files
            .Where(path => forbiddenCalls.Any(call =>
                File.ReadAllText(path).Contains(call, StringComparison.Ordinal)))
            .ToArray();
    }

    private static bool IsController(Type type) =>
        InheritsFrom(type, "Microsoft.AspNetCore.Mvc.ControllerBase");

    private static bool HasApiControllerAttribute(Type type) =>
        type.GetCustomAttributes(inherit: true).Any(attribute =>
            attribute.GetType().FullName
            == "Microsoft.AspNetCore.Mvc.ApiControllerAttribute");

    private static bool InheritsFrom(Type type, string baseTypeName)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (current.FullName == baseTypeName)
            {
                return true;
            }
        }

        return false;
    }
}

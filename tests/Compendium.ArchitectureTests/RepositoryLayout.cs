using System.Xml.Linq;

namespace Compendium.ArchitectureTests;

internal static class RepositoryLayout
{
    public static string Root { get; } = FindRoot();

    public static string Project(string name) =>
        Path.Combine(Root, "src", name, $"{name}.csproj");

    public static IReadOnlyCollection<string> ProjectReferences(string projectName)
    {
        var project = XDocument.Load(Project(projectName));

        return project
            .Descendants("ProjectReference")
            .Select(reference => Path.GetFileNameWithoutExtension(
                reference.Attribute("Include")!.Value))
            .ToArray();
    }

    public static IReadOnlyCollection<string> PackageReferences(string projectName)
    {
        var project = XDocument.Load(Project(projectName));

        return project
            .Descendants("PackageReference")
            .Select(reference => reference.Attribute("Include")!.Value)
            .ToArray();
    }

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(
                directory.FullName,
                "dnd-compendium-service.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Repository root was not found.");
    }
}

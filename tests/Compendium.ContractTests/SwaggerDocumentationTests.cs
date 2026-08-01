using System.Text.Json.Nodes;

namespace Compendium.ContractTests;

public sealed class SwaggerDocumentationTests : IClassFixture<CompendiumApiFactory>
{
    private static readonly HashSet<string> OperationMethods =
    [
        "get",
        "post",
        "put",
        "delete",
        "patch"
    ];

    private readonly HttpClient client;

    public SwaggerDocumentationTests(CompendiumApiFactory factory) =>
        client = factory.CreateAdministrativeClient();

    [Fact]
    public async Task Every_application_operation_has_descriptions_and_examples()
    {
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.True(
            response.IsSuccessStatusCode,
            $"Swagger generation failed: {responseBody}");
        var document = JsonNode.Parse(responseBody)!;
        var schemas = document["components"]!["schemas"]!.AsObject();
        var operations = document["paths"]!
            .AsObject()
            .Where(path => IsApplicationPath(path.Key))
            .SelectMany(path => path.Value!
                .AsObject()
                .Where(operation => OperationMethods.Contains(operation.Key))
                .Select(operation => new
                {
                    Path = path.Key,
                    Method = operation.Key,
                    Value = operation.Value!
                }))
            .ToArray();

        Assert.Equal(83, operations.Length);

        foreach (var operation in operations)
        {
            Assert.False(
                string.IsNullOrWhiteSpace(
                    operation.Value["summary"]?.GetValue<string>()),
                $"{operation.Method.ToUpperInvariant()} {operation.Path} has no summary.");
            Assert.False(
                string.IsNullOrWhiteSpace(
                    operation.Value["description"]?.GetValue<string>()),
                $"{operation.Method.ToUpperInvariant()} {operation.Path} has no description.");

            AssertParametersDocumented(operation.Value, operation.Method, operation.Path);
            AssertRequestSchemaDocumented(
                operation.Value,
                schemas,
                operation.Method,
                operation.Path);
        }
    }

    private static void AssertParametersDocumented(
        JsonNode operation,
        string method,
        string path)
    {
        foreach (var parameter in operation["parameters"]?.AsArray() ?? [])
        {
            var name = parameter!["name"]!.GetValue<string>();
            Assert.False(
                string.IsNullOrWhiteSpace(
                    parameter["description"]?.GetValue<string>()),
                $"Parameter '{name}' on {method.ToUpperInvariant()} {path} has no description.");
            Assert.True(
                HasExampleOrAllowedValues(parameter),
                $"Parameter '{name}' on {method.ToUpperInvariant()} {path} has no example or allowed values.");
        }
    }

    private static void AssertRequestSchemaDocumented(
        JsonNode operation,
        JsonObject schemas,
        string method,
        string path)
    {
        var schema = operation["requestBody"]?["content"]?["application/json"]?["schema"];
        if (schema is null)
        {
            return;
        }

        var visited = new HashSet<string>();
        AssertSchemaFields(schema, schemas, visited, $"{method.ToUpperInvariant()} {path}");
    }

    private static void AssertSchemaFields(
        JsonNode schema,
        JsonObject schemas,
        HashSet<string> visited,
        string operation)
    {
        if (schema["$ref"] is JsonNode referenceNode)
        {
            var reference = referenceNode.GetValue<string>();
            if (!visited.Add(reference))
            {
                return;
            }

            var schemaName = reference.Split('/').Last();
            AssertSchemaFields(schemas[schemaName]!, schemas, visited, operation);
            return;
        }

        foreach (var property in schema["properties"]?.AsObject() ?? [])
        {
            var propertySchema = property.Value!;
            Assert.False(
                string.IsNullOrWhiteSpace(
                    propertySchema["description"]?.GetValue<string>()),
                $"Field '{property.Key}' used by {operation} has no description: {propertySchema.ToJsonString()}");
            Assert.True(
                HasExampleOrAllowedValues(propertySchema),
                $"Field '{property.Key}' used by {operation} has no example or allowed values.");

            AssertSchemaFields(propertySchema, schemas, visited, operation);
            if (propertySchema["items"] is JsonNode items)
            {
                AssertSchemaFields(items, schemas, visited, operation);
            }
        }
    }

    private static bool HasExampleOrAllowedValues(JsonNode node) =>
        node["example"] is not null ||
        node["examples"] is not null ||
        node["enum"] is JsonArray { Count: > 0 } ||
        node["$ref"] is not null ||
        node["items"] is not null ||
        node["properties"] is not null;

    private static bool IsApplicationPath(string path) =>
        path == "/" ||
        path.StartsWith("/api/compendium", StringComparison.Ordinal) ||
        path.StartsWith("/internal/compendium", StringComparison.Ordinal);
}

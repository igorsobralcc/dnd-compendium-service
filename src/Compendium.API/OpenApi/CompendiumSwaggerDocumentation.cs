using System.Globalization;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Compendium.API.OpenApi;

internal sealed class CompendiumOperationDocumentationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor
            is not ControllerActionDescriptor action)
        {
            return;
        }

        var resource = OpenApiText.HumanizeController(action.ControllerName);
        var actionName = OpenApiText.Humanize(action.ActionName);
        var method = context.ApiDescription.HttpMethod ?? "HTTP";
        var route = $"/{context.ApiDescription.RelativePath}";
        var policies = action.EndpointMetadata
            .OfType<IAuthorizeData>()
            .Select(metadata => metadata.Policy)
            .Where(policy => !string.IsNullOrWhiteSpace(policy))
            .Distinct()
            .ToArray();

        operation.Summary = OpenApiText.Summary(actionName, resource);
        operation.Description = policies.Length == 0
            ? $"Handles `{method} {route}` for {resource}. This operation is available without an API key."
            : $"Handles `{method} {route}` for {resource}. Requires the `{string.Join("`, `", policies)}` authorization policy.";

        foreach (var parameter in operation.Parameters ?? [])
        {
            var parameterName = parameter.Name ?? "value";
            parameter.Description = OpenApiText.DescribeField(parameterName);

            if (parameter is OpenApiParameter concreteParameter)
            {
                concreteParameter.Example = OpenApiExamples.For(
                parameterName,
                context.ApiDescription.ParameterDescriptions
                    .FirstOrDefault(description =>
                        string.Equals(
                            description.Name,
                            parameterName,
                            StringComparison.OrdinalIgnoreCase))
                    ?.Type);
            }
        }

        foreach (var response in operation.Responses ?? [])
        {
            if (response.Value is not null)
            {
                response.Value.Description = OpenApiText.DescribeResponse(
                    response.Key,
                    operation.Summary ?? "Operation");
            }
        }
    }
}

internal sealed class CompendiumSchemaDocumentationFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Description ??= OpenApiText.DescribeType(context.Type);

        if (schema.Properties is not null)
        {
            foreach (var (name, propertySchema) in schema.Properties)
            {
                var property = context.Type.GetProperty(
                    name,
                    BindingFlags.IgnoreCase |
                    BindingFlags.Public |
                    BindingFlags.Instance);
                var propertyType = property?.PropertyType;

                propertySchema.Description ??= OpenApiText.DescribeField(
                    property?.Name ?? name,
                    propertyType);
                if (propertySchema is OpenApiSchema concretePropertySchema)
                {
                    concretePropertySchema.Example ??= OpenApiExamples.For(
                        name,
                        propertyType);
                }
            }
        }

        if (schema is OpenApiSchema concreteSchema)
        {
            concreteSchema.Example ??= OpenApiExamples.ForType(context.Type);
        }
    }
}

internal static class OpenApiText
{
    public static string Summary(string action, string resource)
    {
        var normalizedAction = action.ToLowerInvariant();

        return normalizedAction switch
        {
            "get" or "get details" or "get by code" or "get localized" or
                "get all" => $"Get {resource}",
            "list" or "list issues" => $"List {resource}",
            "create" => $"Create {resource}",
            "update" => $"Update {resource}",
            "remove" => $"Remove {resource}",
            "activate" => $"Activate {resource}",
            "deactivate" => $"Deactivate {resource}",
            "validate" => $"Validate {resource}",
            "import" => $"Import {resource}",
            _ => $"{action} {resource}"
        };
    }

    public static string HumanizeController(string controllerName) =>
        Humanize(controllerName);

    public static string Humanize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "resource";
        }

        var words = new List<string>();
        var start = 0;

        for (var index = 1; index < value.Length; index++)
        {
            if (!char.IsUpper(value[index]) || char.IsUpper(value[index - 1]))
            {
                continue;
            }

            words.Add(value[start..index]);
            start = index;
        }

        words.Add(value[start..]);
        return string.Join(' ', words);
    }

    public static string DescribeType(Type type) =>
        $"{Humanize(type.Name.Replace("Request", string.Empty, StringComparison.Ordinal).Replace("Dto", string.Empty, StringComparison.Ordinal))} payload.";

    public static string DescribeField(string name, Type? type = null)
    {
        var normalized = name.ToLowerInvariant();
        var description = normalized switch
        {
            "id" => "Unique identifier for the resource.",
            var value when value.EndsWith("id", StringComparison.Ordinal) =>
                $"Unique identifier of the related {Humanize(name[..^2]).ToLowerInvariant()} resource.",
            "code" => "Stable, human-readable code used to identify the resource.",
            var value when value.EndsWith("code", StringComparison.Ordinal) =>
                $"Stable code for {Humanize(name[..^4]).ToLowerInvariant()}.",
            "name" => "Display name of the resource.",
            "description" => "Human-readable rules description.",
            "locale" => "Requested IETF locale used for localized values.",
            "fallbacklocale" => "Fallback IETF locale used when a translation is unavailable.",
            "field" => "Name of the translatable field.",
            "entitytype" => "Compendium entity type, such as class, feature, equipment, or choice_set.",
            "entitykind" => "Kind of compendium entity that owns the relationship.",
            "ownertype" => "Type of entity that owns the starting-equipment rule.",
            "level" => "Character or class level. Allowed range: 1 through 20.",
            "page" => "One-based result page number.",
            "pagesize" => "Maximum number of results per page. Allowed range: 1 through 200.",
            "revision" => "Return changes after this non-negative revision.",
            "changedsince" => "Return changes recorded at or after this UTC timestamp.",
            "text" => "Translated text stored for the selected entity field.",
            "version" => "Published source or ruleset version label.",
            "weight" => "Equipment weight in the catalog's standard unit.",
            "costamount" => "Numeric equipment cost before applying the currency denomination.",
            "costcurrency" => "Currency denomination used by the equipment cost.",
            "damagedice" => "Dice expression used to roll weapon damage, such as 1d8.",
            "quantity" => "Number of referenced items. Must be greater than zero.",
            "selectioncount" => "Number of options that must be selected from the group.",
            "baseac" => "Base Armor Class supplied by this armor rule.",
            "addsdexterity" => "Whether the Dexterity modifier contributes to Armor Class.",
            "maximumdexteritybonus" => "Maximum Dexterity modifier allowed by the armor, when limited.",
            "bonus" => "Additional numeric modifier applied by the rule.",
            "abilitycode" => "Ability code associated with the tool, when applicable.",
            "rules" => "Validation or mechanical rules applied to the resource.",
            "effects" => "Mechanical effects produced by the resource.",
            "requirements" => "Requirements that must be satisfied before the resource applies.",
            "groups" => "Ordered starting-equipment choice groups.",
            "options" => "Selectable options owned by this group or choice set.",
            "filters" => "Filters that restrict the available choice-set options.",
            "items" => "Items contained in the equipment pack or import manifest.",
            _ when type is not null &&
                (type.IsEnum || Nullable.GetUnderlyingType(type)?.IsEnum == true) =>
                $"Allowed {Humanize(name).ToLowerInvariant()} value. See the schema for all possible values.",
            _ => $"{Humanize(name)} value used by this operation."
        };

        return description;
    }

    public static string DescribeResponse(string statusCode, string summary) =>
        statusCode switch
        {
            "200" => $"{summary} completed successfully.",
            "201" => $"{summary} created a resource successfully.",
            "204" => $"{summary} completed successfully without a response body.",
            "400" => "The request failed validation. The response uses Problem Details.",
            "401" => "A valid API key is required. The response uses Problem Details.",
            "403" => "The API key does not grant the required permission. The response uses Problem Details.",
            "404" => "The requested resource was not found. The response uses Problem Details.",
            "409" => "The request conflicts with existing state. The response uses Problem Details.",
            "500" => "An unexpected error occurred. The response uses Problem Details.",
            _ => $"HTTP {statusCode} response for {summary.ToLowerInvariant()}."
        };
}

internal static class OpenApiExamples
{
    private const string ExampleId = "0196f7a4-7e31-7f21-9d9a-9b8d7f68a501";

    public static JsonNode? For(string name, Type? type)
    {
        var actualType = Nullable.GetUnderlyingType(type ?? typeof(string))
            ?? type
            ?? typeof(string);
        var normalized = name.ToLowerInvariant();

        if (actualType.IsEnum)
        {
            return JsonValue.Create(Convert.ToInt32(
                Enum.GetValues(actualType).GetValue(0),
                CultureInfo.InvariantCulture));
        }

        if (actualType == typeof(Guid) || normalized.EndsWith("id", StringComparison.Ordinal))
        {
            return JsonValue.Create(ExampleId);
        }

        if (actualType == typeof(bool))
        {
            return JsonValue.Create(true);
        }

        if (actualType == typeof(DateTimeOffset) || actualType == typeof(DateTime))
        {
            return JsonValue.Create("2026-08-01T12:00:00Z");
        }

        if (actualType == typeof(int) || actualType == typeof(long) ||
            actualType == typeof(short))
        {
            return JsonValue.Create(normalized switch
            {
                "page" => 1,
                "pagesize" => 50,
                "level" => 1,
                "revision" => 0,
                "quantity" => 1,
                "selectioncount" => 1,
                "baseac" => 12,
                _ => 1
            });
        }

        if (actualType == typeof(decimal) || actualType == typeof(double) ||
            actualType == typeof(float))
        {
            return JsonValue.Create(normalized switch
            {
                "weight" => 2.5,
                "costamount" => 15.0,
                _ => 1.0
            });
        }

        if (actualType != typeof(string))
        {
            return null;
        }

        return JsonValue.Create(normalized switch
        {
            "code" => "extra_attack",
            var value when value.EndsWith("code", StringComparison.Ordinal) => "strength",
            "name" => "Extra Attack",
            "description" => "A concise description of the compendium rule.",
            "locale" => "en-US",
            "fallbacklocale" => "en-US",
            "field" => "name",
            "entitytype" => "feature",
            "entitykind" => "feature",
            "ownertype" => "class",
            "version" => "1.0.0",
            "damagedice" => "1d8",
            "abilitycode" => "strength",
            "operator" => "equals",
            "text" => "Localized rule text",
            _ => "example-value"
        });
    }

    public static JsonNode? ForType(Type type)
    {
        var actualType = Nullable.GetUnderlyingType(type) ?? type;

        return actualType.IsPrimitive || actualType == typeof(string) ||
            actualType == typeof(Guid) || actualType.IsEnum
            ? For(actualType.Name, actualType)
            : null;
    }
}

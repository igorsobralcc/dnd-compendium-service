using System.Net;
using Compendium.API.Security;

namespace Compendium.ContractTests;

public sealed record HttpContract(
    string Method,
    string Route,
    string? Name,
    HttpStatusCode SuccessStatus,
    string? AuthorizationPolicy);

public static class HttpContractMatrix
{
    private const string Api = "/api/compendium";

    public static IReadOnlyList<HttpContract> Routes { get; } =
    [
        Write("POST", $"{Api}/rulesets", "CreateRuleset", HttpStatusCode.Created),
        Write("PUT", $"{Api}/rulesets/{{code}}", "UpdateRuleset"),
        Read($"{Api}/rulesets/{{code}}", "GetRulesetByCode"),
        Write("POST", $"{Api}/rule-sources", "CreateRuleSource", HttpStatusCode.Created),
        Write("POST", $"{Api}/rule-sources/{{id:guid}}/activate", "ActivateRuleSource", HttpStatusCode.NoContent),
        Write("POST", $"{Api}/rule-sources/{{id:guid}}/deactivate", "DeactivateRuleSource", HttpStatusCode.NoContent),
        Read($"{Api}/rulesets/{{rulesetId:guid}}/rule-sources", "ListRuleSourcesByRuleset"),
        Write("POST", $"{Api}/source-versions", "CreateSourceVersion", HttpStatusCode.Created),
        Write("POST", $"{Api}/rule-sources/{{ruleSourceId:guid}}/source-versions/{{versionId:guid}}/current", "MarkSourceVersionAsCurrent", HttpStatusCode.NoContent),
        Read($"{Api}/rule-sources/{{ruleSourceId:guid}}/source-versions/current", "GetCurrentSourceVersion"),
        Read($"{Api}/rule-sources/{{ruleSourceId:guid}}/source-versions", "ListSourceVersions"),

        Write("POST", $"{Api}/abilities", "CreateAbility", HttpStatusCode.Created),
        Write("PUT", $"{Api}/abilities/{{code}}", "UpdateAbility"),
        Read($"{Api}/abilities", "ListAbilities"),
        Write("POST", $"{Api}/skills", "CreateSkill", HttpStatusCode.Created),
        Write("PUT", $"{Api}/skills/{{code}}", "UpdateSkill"),
        Read($"{Api}/skills", "ListSkills"),
        Write("POST", $"{Api}/languages", "CreateLanguage", HttpStatusCode.Created),
        Write("PUT", $"{Api}/languages/{{code}}", "UpdateLanguage"),
        Read($"{Api}/languages", "ListLanguages"),
        Write("POST", $"{Api}/proficiencies", "CreateProficiency", HttpStatusCode.Created),
        Write("PUT", $"{Api}/proficiencies/{{code}}", "UpdateProficiency"),
        Read($"{Api}/proficiencies", "ListProficiencies"),
        Write("POST", $"{Api}/armor-training-categories", "CreateArmorTrainingCategory", HttpStatusCode.Created),
        Read($"{Api}/armor-training-categories", "ListArmorTrainingCategories"),
        Write("POST", $"{Api}/hit-dice", "CreateHitDie", HttpStatusCode.Created),
        Read($"{Api}/hit-dice", "ListHitDice"),
        Write("POST", $"{Api}/ability-score-methods", "CreateAbilityScoreMethod", HttpStatusCode.Created),
        Read($"{Api}/ability-score-methods", "ListAbilityScoreMethods"),

        Write("POST", $"{Api}/classes", "CreateClass", HttpStatusCode.Created),
        Write("PUT", $"{Api}/classes/{{code}}", "UpdateClass"),
        Read($"{Api}/classes", "ListClasses"),
        Read($"{Api}/classes/{{code}}", "GetClassDetails"),
        Write("PUT", $"{Api}/classes/{{code}}/progression", "ConfigureClassProgression"),
        Read($"{Api}/classes/{{code}}/progression", "GetClassProgression"),
        Write("POST", $"{Api}/classes/{{classCode}}/subclasses", "CreateSubclass", HttpStatusCode.Created),
        Read($"{Api}/classes/{{classCode}}/subclasses", "ListSubclassesByClass"),
        Read($"{Api}/classes/{{classCode}}/subclasses/{{subclassCode}}", "GetSubclassDetails"),
        Write("POST", $"{Api}/classes/{{classCode}}/subclasses/{{subclassCode}}/features", "LinkSubclassFeature", HttpStatusCode.NoContent),

        Write("POST", $"{Api}/features", "CreateFeature", HttpStatusCode.Created),
        Write("PUT", $"{Api}/features/{{code}}", "UpdateFeature"),
        Read($"{Api}/features", "ListFeatures"),
        Read($"{Api}/features/{{code}}", "GetFeatureDetails"),
        Write("POST", $"{Api}/effect-schemas", "CreateEffectSchema", HttpStatusCode.Created),
        Write("POST", $"{Api}/features/{{featureCode}}/effects", "AttachEffectToFeature", HttpStatusCode.NoContent),
        Read($"{Api}/features/{{featureCode}}/effects", "GetFeatureEffects"),
        Write("POST", $"{Api}/entity-prerequisites", "AddPrerequisiteToEntity", HttpStatusCode.Created),
        Write("DELETE", $"{Api}/entity-prerequisites/{{prerequisiteId:guid}}", "RemovePrerequisiteFromEntity", HttpStatusCode.NoContent),
        Read($"{Api}/entity-prerequisites/{{entityKind}}/{{entityId:guid}}", "GetEntityPrerequisites"),
        Write("POST", $"{Api}/choice-sets", "CreateChoiceSet", HttpStatusCode.Created),
        Read($"{Api}/choice-sets/{{code}}", "GetChoiceSetDetails"),
        Read($"{Api}/choice-sets/by-source/{{entityKind}}/{{entityId:guid}}", "ListChoiceSetsBySourceEntity"),
        Write("POST", $"{Api}/choice-sets/{{code}}/options", "AddChoiceOption", HttpStatusCode.NoContent),
        Write("POST", $"{Api}/choice-sets/{{code}}/filters", "AddChoiceFilter", HttpStatusCode.NoContent),

        Write("POST", $"{Api}/equipment/", null, HttpStatusCode.Created),
        Write("PUT", $"{Api}/equipment/{{code}}", null),
        Read($"{Api}/equipment/", null),
        Read($"{Api}/equipment/{{code}}", null),
        Write("POST", $"{Api}/equipment/weapons/properties", null, HttpStatusCode.Created),
        Write("POST", $"{Api}/equipment/weapons/masteries", null, HttpStatusCode.Created),
        Write("POST", $"{Api}/equipment/weapons", null),
        Write("POST", $"{Api}/equipment/weapons/{{equipmentItemId:guid}}/properties", null, HttpStatusCode.NoContent),
        Read($"{Api}/equipment/weapons", null),
        Read($"{Api}/equipment/weapons/{{equipmentItemId:guid}}", null),
        Write("POST", $"{Api}/equipment/armors", null),
        Write("PUT", $"{Api}/equipment/armors/{{equipmentItemId:guid}}/ac-rule", null, HttpStatusCode.NoContent),
        Read($"{Api}/equipment/armors", null),
        Read($"{Api}/equipment/armors/{{equipmentItemId:guid}}", null),
        Write("POST", $"{Api}/equipment/tools", null, HttpStatusCode.Created),
        Write("POST", $"{Api}/equipment/packs", null, HttpStatusCode.Created),
        Write("POST", $"{Api}/equipment/starting-rules", null),
        Read($"{Api}/equipment/starting-rules/{{ownerType}}/{{ownerId:guid}}", null),

        Write("PUT", $"{Api}/entities/{{entityType}}/{{entityId:guid}}/translations/{{locale}}/{{field}}", "UpsertTranslation"),
        Read($"{Api}/entities/{{entityType}}/{{entityId:guid}}/translations/", "GetTranslationsForEntity"),
        Read($"{Api}/entities/{{entityType}}/{{entityId:guid}}/translations/localized", "GetLocalizedEntityTranslations"),
        Write("POST", $"{Api}/source-versions/{{sourceVersionId:guid}}/imports", "ImportSourceVersion", HttpStatusCode.Created),
        Write("POST", $"{Api}/source-versions/{{sourceVersionId:guid}}/validation", "ValidateSourceVersion"),
        Read($"{Api}/source-versions/{{sourceVersionId:guid}}/validation/issues", "ListSourceVersionValidationIssues"),

        Internal("/internal/compendium/character-creation-options", "GetCharacterCreationOptionsV1"),
        Internal("/internal/compendium/entities/{entityType}/{entityId:guid}/mechanics", "GetMechanicalEntityDetailsV1"),
        Internal("/internal/compendium/changes", "ListCompendiumChangesV1"),
        Read("/", "GetServiceStatus"),
        Read("/internal/compendium/metadata", "GetCompendiumMetadata")
    ];

    private static HttpContract Read(string route, string? name) =>
        new("GET", route, name, HttpStatusCode.OK, null);

    private static HttpContract Internal(string route, string name) =>
        new("GET", route, name, HttpStatusCode.OK, CompendiumSecurity.InternalReadPolicy);

    private static HttpContract Write(
        string method,
        string route,
        string? name,
        HttpStatusCode successStatus = HttpStatusCode.OK) =>
        new(method, route, name, successStatus, CompendiumSecurity.AdministrativeWritePolicy);
}

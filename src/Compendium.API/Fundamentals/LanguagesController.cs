using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Fundamentals;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Fundamentals;

[Route("/api/compendium/languages")]
public sealed class LanguagesController(
    CreateLanguageUseCase createLanguage,
    UpdateLanguageUseCase updateLanguage,
    ListLanguagesQuery listLanguages) : CompendiumControllerBase
{
    [HttpPost(Name = "CreateLanguage")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateLanguageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createLanguage.ExecuteAsync(
            new CreateLanguageCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name),
            cancellationToken);

        return CreatedOrProblem(
            result,
            language => $"/api/compendium/languages/{language.Code}");
    }

    [HttpPut("{code}", Name = "UpdateLanguage")]
    [AdministrativeWrite]
    public async Task<IActionResult> Update(
        string code,
        UpdateLanguageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateLanguage.ExecuteAsync(
            new UpdateLanguageCommand(
                code,
                request.RuleSourceId,
                request.SourceVersionId,
                request.Name),
            cancellationToken);

        return OkOrProblem(result);
    }

    [HttpGet(Name = "ListLanguages")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listLanguages.ExecuteAsync(cancellationToken);
        return OkOrProblem(result);
    }
}

public sealed record CreateLanguageRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name);

public sealed record UpdateLanguageRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Name);

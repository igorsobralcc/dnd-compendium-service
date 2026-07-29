using Compendium.API.Controllers;
using Compendium.API.Security;
using Compendium.Application.Fundamentals;
using Microsoft.AspNetCore.Mvc;

namespace Compendium.API.Fundamentals;

[Route("/api/compendium/armor-training-categories")]
public sealed class ArmorTrainingCategoriesController(
    CreateArmorTrainingCategoryUseCase createArmorTrainingCategory,
    ListArmorTrainingCategoriesQuery listArmorTrainingCategories)
    : CompendiumControllerBase
{
    [HttpPost(Name = "CreateArmorTrainingCategory")]
    [AdministrativeWrite]
    public async Task<IActionResult> Create(
        CreateArmorTrainingCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createArmorTrainingCategory.ExecuteAsync(
            new CreateArmorTrainingCategoryCommand(
                request.RuleSourceId,
                request.SourceVersionId,
                request.Code,
                request.Name,
                request.SortOrder),
            cancellationToken);

        return CreatedOrProblem(
            result,
            category =>
                $"/api/compendium/armor-training-categories/{category.Code}");
    }

    [HttpGet(Name = "ListArmorTrainingCategories")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var result = await listArmorTrainingCategories.ExecuteAsync(
            cancellationToken);

        return OkOrProblem(result);
    }
}

public sealed record CreateArmorTrainingCategoryRequest(
    Guid RuleSourceId,
    Guid SourceVersionId,
    string Code,
    string Name,
    int SortOrder);

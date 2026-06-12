using Compendium.Domain.SharedKernel;
using Compendium.Application.Fundamentals;
using Compendium.Application.Sources;
using Microsoft.Extensions.DependencyInjection;

namespace Compendium.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<CreateRulesetUseCase>();
        services.AddScoped<UpdateRulesetUseCase>();
        services.AddScoped<GetRulesetByCodeQuery>();
        services.AddScoped<CreateRuleSourceUseCase>();
        services.AddScoped<ActivateRuleSourceUseCase>();
        services.AddScoped<DeactivateRuleSourceUseCase>();
        services.AddScoped<ListRuleSourcesByRulesetQuery>();
        services.AddScoped<CreateSourceVersionUseCase>();
        services.AddScoped<MarkSourceVersionAsCurrentUseCase>();
        services.AddScoped<GetCurrentSourceVersionQuery>();
        services.AddScoped<ListSourceVersionsQuery>();
        services.AddScoped<CreateAbilityUseCase>();
        services.AddScoped<UpdateAbilityUseCase>();
        services.AddScoped<ListAbilitiesQuery>();
        services.AddScoped<CreateSkillUseCase>();
        services.AddScoped<UpdateSkillUseCase>();
        services.AddScoped<ListSkillsQuery>();
        services.AddScoped<CreateLanguageUseCase>();
        services.AddScoped<UpdateLanguageUseCase>();
        services.AddScoped<ListLanguagesQuery>();
        services.AddScoped<CreateProficiencyUseCase>();
        services.AddScoped<UpdateProficiencyUseCase>();
        services.AddScoped<ListProficienciesQuery>();
        services.AddScoped<CreateArmorTrainingCategoryUseCase>();
        services.AddScoped<ListArmorTrainingCategoriesQuery>();
        services.AddScoped<CreateHitDieUseCase>();
        services.AddScoped<ListHitDiceQuery>();
        services.AddScoped<CreateAbilityScoreMethodUseCase>();
        services.AddScoped<ListAbilityScoreMethodsQuery>();

        return services;
    }
}

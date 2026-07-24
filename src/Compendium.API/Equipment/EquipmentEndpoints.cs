using Compendium.API.Errors;
using Compendium.Application.Equipment;
using Compendium.Domain.Equipment;

namespace Compendium.API.Equipment;

public static class EquipmentEndpoints
{
    public static IEndpointRouteBuilder MapEquipmentEndpoints(this IEndpointRouteBuilder app)
    {
        var g=app.MapGroup("/api/compendium/equipment").WithTags("Equipment");
        g.MapPost("/",CreateItem);g.MapPut("/{code}",UpdateItem);g.MapGet("/",ListItems);g.MapGet("/{code}",GetItem);
        g.MapPost("/weapons/properties",CreateWeaponProperty);g.MapPost("/weapons/masteries",CreateMastery);g.MapPost("/weapons",CreateWeapon);
        g.MapPost("/weapons/{equipmentItemId:guid}/properties",AttachProperty);g.MapGet("/weapons",ListWeapons);g.MapGet("/weapons/{equipmentItemId:guid}",GetWeapon);
        g.MapPost("/armors",CreateArmor);g.MapPut("/armors/{equipmentItemId:guid}/ac-rule",ConfigureArmor);g.MapGet("/armors",ListArmors);g.MapGet("/armors/{equipmentItemId:guid}",GetArmor);
        g.MapPost("/tools",CreateTool);g.MapPost("/packs",CreatePack);g.MapPost("/starting-rules",CreateStartingRule);g.MapGet("/starting-rules/{ownerType}/{ownerId:guid}",GetStartingRule);
        return app;
    }
    private static IResult Result<T>(Application.Errors.ApplicationResult<T> r,Func<T,IResult> ok)=>r.IsSuccess?ok(r.Value):HttpErrorMapper.ToProblem(r.Error);
    private static async Task<IResult>CreateItem(CreateEquipmentItemRequest r,CreateEquipmentItemUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(new(r.RuleSourceId,r.SourceVersionId,r.Code,r.Name,r.Category,r.Weight,r.CostAmount,r.CostCurrency,r.Description),ct),x=>Results.Created($"/api/compendium/equipment/{x.Code}",x));
    private static async Task<IResult>UpdateItem(string code,UpdateEquipmentItemRequest r,UpdateEquipmentItemUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(new(code,r.RuleSourceId,r.SourceVersionId,r.Name,r.Category,r.Weight,r.CostAmount,r.CostCurrency,r.Description),ct),Results.Ok);
    private static async Task<IResult>ListItems(EquipmentCategory? category,ListEquipmentItemsQuery q,CancellationToken ct)=>Result(await q.ExecuteAsync(category,ct),Results.Ok);
    private static async Task<IResult>GetItem(string code,GetEquipmentItemDetailsQuery q,CancellationToken ct)=>Result(await q.ExecuteAsync(code,ct),Results.Ok);
    private static async Task<IResult>CreateWeapon(CreateWeaponRequest r,CreateWeaponUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(new(r.EquipmentItemId,r.Category,r.DamageDice,r.DamageType),ct),Results.Ok);
    private static async Task<IResult>CreateWeaponProperty(CreateWeaponPropertyRequest r,CreateWeaponPropertyUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(new(r.Code,r.Name,r.ValueType,r.Rules),ct),x=>Results.Created($"/api/compendium/equipment/weapons/properties/{x}",new{id=x}));
    private static async Task<IResult>AttachProperty(Guid equipmentItemId,AttachWeaponPropertyRequest r,AttachWeaponPropertyUseCase u,CancellationToken ct){var x=await u.ExecuteAsync(new(equipmentItemId,r.WeaponPropertyId,r.Values),ct);return x.IsSuccess?Results.NoContent():HttpErrorMapper.ToProblem(x.Error);}
    private static async Task<IResult>CreateMastery(ConfigureWeaponMasteryCommand r,ConfigureWeaponMasteryUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(r,ct),x=>Results.Created($"/api/compendium/equipment/weapons/masteries/{x}",new{id=x}));
    private static async Task<IResult>ListWeapons(ListWeaponsQuery q,CancellationToken ct)=>Result(await q.ExecuteAsync(ct),Results.Ok);
    private static async Task<IResult>GetWeapon(Guid equipmentItemId,GetWeaponDetailsQuery q,CancellationToken ct)=>Result(await q.ExecuteAsync(equipmentItemId,ct),Results.Ok);
    private static async Task<IResult>CreateArmor(CreateArmorCommand r,CreateArmorUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(r,ct),Results.Ok);
    private static async Task<IResult>ConfigureArmor(Guid equipmentItemId,ConfigureArmorRequest r,ConfigureArmorAcRuleUseCase u,CancellationToken ct){var x=await u.ExecuteAsync(new(equipmentItemId,r.BaseAc,r.AddsDexterity,r.MaximumDexterityBonus,r.Bonus,r.Drawbacks),ct);return x.IsSuccess?Results.NoContent():HttpErrorMapper.ToProblem(x.Error);}
    private static async Task<IResult>ListArmors(ListArmorsQuery q,CancellationToken ct)=>Result(await q.ExecuteAsync(ct),Results.Ok);
    private static async Task<IResult>GetArmor(Guid equipmentItemId,GetArmorDetailsQuery q,CancellationToken ct)=>Result(await q.ExecuteAsync(equipmentItemId,ct),Results.Ok);
    private static async Task<IResult>CreateTool(CreateToolCommand r,CreateToolUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(r,ct),x=>Results.Created($"/api/compendium/equipment/tools/{x}",new{id=x}));
    private static async Task<IResult>CreatePack(CreateEquipmentPackCommand r,CreateEquipmentPackUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(r,ct),x=>Results.Created($"/api/compendium/equipment/packs/{x}",new{id=x}));
    private static async Task<IResult>CreateStartingRule(CreateStartingEquipmentRuleCommand r,CreateStartingEquipmentRuleUseCase u,CancellationToken ct)=>Result(await u.ExecuteAsync(r,ct),Results.Ok);
    private static async Task<IResult>GetStartingRule(StartingEquipmentOwnerType ownerType,Guid ownerId,GetStartingEquipmentRuleQuery q,CancellationToken ct)=>Result(await q.ExecuteAsync(ownerType,ownerId,ct),Results.Ok);
}
public sealed record CreateEquipmentItemRequest(Guid RuleSourceId,Guid SourceVersionId,string Code,string Name,EquipmentCategory Category,decimal Weight,decimal CostAmount,Currency CostCurrency,string? Description);
public sealed record UpdateEquipmentItemRequest(Guid RuleSourceId,Guid SourceVersionId,string Name,EquipmentCategory Category,decimal Weight,decimal CostAmount,Currency CostCurrency,string? Description);
public sealed record CreateWeaponRequest(Guid EquipmentItemId,WeaponCategory Category,string DamageDice,DamageType DamageType);
public sealed record CreateWeaponPropertyRequest(string Code,string Name,WeaponPropertyValueType ValueType,IReadOnlyCollection<WeaponPropertyRuleCommand> Rules);
public sealed record AttachWeaponPropertyRequest(Guid WeaponPropertyId,IReadOnlyCollection<string> Values);
public sealed record ConfigureArmorRequest(int BaseAc,bool AddsDexterity,int? MaximumDexterityBonus,int Bonus,IReadOnlyCollection<ArmorDrawbackCommand> Drawbacks);

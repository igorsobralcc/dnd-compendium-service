using Compendium.Application.Errors;
using Compendium.Application.Fundamentals;
using Compendium.Application.Sources;
using Compendium.Domain.Equipment;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Equipment;

file static class Ids
{
    public static ApplicationResult<CompendiumEntityId> Parse(Guid value)
    {
        var r = CompendiumEntityId.Create(value);
        return r.IsSuccess ? ApplicationResult<CompendiumEntityId>.Success(r.Value) : ApplicationResult<CompendiumEntityId>.Failure(EquipmentErrors.FromDomain(r.Error));
    }
}

public sealed class CreateEquipmentItemUseCase(IRuleSourceRepository sources, ISourceVersionRepository versions, IEquipmentRepository repository, IClock clock)
{
    public async Task<ApplicationResult<EquipmentItemDto>> ExecuteAsync(CreateEquipmentItemCommand c, CancellationToken ct)
    {
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, c.RuleSourceId, c.SourceVersionId, ct);
        var code = EquipmentCode.Create(c.Code);
        var name = EquipmentName.Create(c.Name);
        var weight = Weight.Create(c.Weight);
        var cost = Cost.Create(c.CostAmount, c.CostCurrency);
        if (source.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(source.Error);
        if (code.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(code.Error));
        if (name.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(name.Error));
        if (weight.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(weight.Error));
        if (cost.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(cost.Error));
        if (await repository.ExistsByCodeAsync(code.Value, ct)) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.Conflict($"Equipment code '{code.Value}' already exists."));
        var item = EquipmentItem.Create(source.Value.RuleSourceId, source.Value.SourceVersionId, code.Value, name.Value, c.Category, weight.Value, cost.Value, c.Description, clock.UtcNow);
        if (item.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(item.Error));
        await repository.AddAsync(item.Value, ct);
        await repository.SaveChangesAsync(ct);
        return ApplicationResult<EquipmentItemDto>.Success(item.Value.ToDto());
    }
}
public sealed class UpdateEquipmentItemUseCase(IRuleSourceRepository sources, ISourceVersionRepository versions, IEquipmentRepository repository, IClock clock)
{
    public async Task<ApplicationResult<EquipmentItemDto>> ExecuteAsync(UpdateEquipmentItemCommand c, CancellationToken ct)
    {
        var code = EquipmentCode.Create(c.Code);
        if (code.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(code.Error));
        var item = await repository.GetByCodeAsync(code.Value, ct);
        if (item is null) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.NotFound("item", c.Code));
        var source = await FundamentalSourceReference.ValidateAsync(sources, versions, c.RuleSourceId, c.SourceVersionId, ct);
        var name = EquipmentName.Create(c.Name);
        var weight = Weight.Create(c.Weight);
        var cost = Cost.Create(c.CostAmount, c.CostCurrency);
        if (source.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(source.Error);
        if (name.IsFailure || weight.IsFailure || cost.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(name.IsFailure ? name.Error : weight.IsFailure ? weight.Error : cost.Error));
        var update = item.Update(source.Value.SourceVersionId, name.Value, c.Category, weight.Value, cost.Value, c.Description, clock.UtcNow);
        if (update.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(update.Error));
        await repository.SaveChangesAsync(ct);
        return ApplicationResult<EquipmentItemDto>.Success(item.ToDto());
    }
}
public sealed class ListEquipmentItemsQuery(IEquipmentRepository repository)
{
    public async Task<ApplicationResult<IReadOnlyCollection<EquipmentItemDto>>> ExecuteAsync(EquipmentCategory? category, CancellationToken ct) =>
        ApplicationResult<IReadOnlyCollection<EquipmentItemDto>>.Success((await repository.ListAsync(category, ct)).Select(x => x.ToDto()).ToArray());
}
public sealed class GetEquipmentItemDetailsQuery(IEquipmentRepository repository)
{
    public async Task<ApplicationResult<EquipmentItemDto>> ExecuteAsync(string code, CancellationToken ct)
    {
        var parsed = EquipmentCode.Create(code);
        if (parsed.IsFailure) return ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.FromDomain(parsed.Error));
        var x = await repository.GetByCodeAsync(parsed.Value, ct);
        return x is null ? ApplicationResult<EquipmentItemDto>.Failure(EquipmentErrors.NotFound("item", code)) : ApplicationResult<EquipmentItemDto>.Success(x.ToDto());
    }
}

public sealed class CreateWeaponUseCase(IEquipmentRepository items, IWeaponRepository weapons)
{
    public async Task<ApplicationResult<WeaponDto>> ExecuteAsync(CreateWeaponCommand c, CancellationToken ct)
    {
        var id = Ids.Parse(c.EquipmentItemId);
        if (id.IsFailure) return ApplicationResult<WeaponDto>.Failure(id.Error);
        var item = await items.GetByIdAsync(id.Value, ct);
        if (item is null) return ApplicationResult<WeaponDto>.Failure(EquipmentErrors.NotFound("item", c.EquipmentItemId.ToString()));
        if (item.Category != EquipmentCategory.Weapon) return ApplicationResult<WeaponDto>.Failure(EquipmentErrors.FromDomain(EquipmentDomainErrors.CategoryMismatch("Weapon")));
        if (await weapons.GetByItemIdAsync(id.Value, ct) is not null) return ApplicationResult<WeaponDto>.Failure(EquipmentErrors.Conflict("Item is already a weapon."));
        var weapon = Weapon.Create(id.Value, c.Category, c.DamageDice, c.DamageType);
        if (weapon.IsFailure) return ApplicationResult<WeaponDto>.Failure(EquipmentErrors.FromDomain(weapon.Error));
        await weapons.AddAsync(weapon.Value, ct);
        await weapons.SaveChangesAsync(ct);
        return ApplicationResult<WeaponDto>.Success(new(weapon.Value.Id.Value, item.ToDto(), weapon.Value.Category, weapon.Value.DamageDice, weapon.Value.DamageType, []));
    }
}
public sealed class AttachWeaponPropertyUseCase(IWeaponRepository weapons, IWeaponPropertyRepository properties)
{
    public async Task<ApplicationResult> ExecuteAsync(AttachWeaponPropertyCommand c, CancellationToken ct)
    {
        var itemId = Ids.Parse(c.EquipmentItemId);
        var propertyId = Ids.Parse(c.WeaponPropertyId);
        if (itemId.IsFailure || propertyId.IsFailure) return ApplicationResult.Failure(itemId.IsFailure ? itemId.Error : propertyId.Error);
        var weapon = await weapons.GetByItemIdAsync(itemId.Value, ct);
        if (weapon is null) return ApplicationResult.Failure(EquipmentErrors.NotFound("weapon", c.EquipmentItemId.ToString()));
        if (await properties.GetByIdAsync(propertyId.Value, ct) is null) return ApplicationResult.Failure(EquipmentErrors.NotFound("weapon-property", c.WeaponPropertyId.ToString()));
        var result = weapon.AttachProperty(propertyId.Value, c.Values);
        if (result.IsFailure) return ApplicationResult.Failure(EquipmentErrors.FromDomain(result.Error));
        await weapons.SaveChangesAsync(ct);
        return ApplicationResult.Success();
    }
}
public sealed class CreateWeaponPropertyUseCase(IWeaponPropertyRepository repository)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(CreateWeaponPropertyCommand c, CancellationToken ct)
    {
        var x = WeaponProperty.Create(c.Code, c.Name, c.ValueType, c.Rules.Select(r => (r.Field, r.Operator, r.Value)).ToArray());
        if (x.IsFailure) return ApplicationResult<Guid>.Failure(EquipmentErrors.FromDomain(x.Error));
        await repository.AddAsync(x.Value, ct);
        await repository.SaveChangesAsync(ct);
        return ApplicationResult<Guid>.Success(x.Value.Id.Value);
    }
}
public sealed class ConfigureWeaponMasteryUseCase(IWeaponMasteryRepository repository)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(ConfigureWeaponMasteryCommand c, CancellationToken ct)
    {
        var x = WeaponMasteryProperty.Create(c.Code, c.Name, c.Effects.Select(e => (e.Type, e.Value)).ToArray(), c.Requirements.Select(r => (r.Type, r.Value)).ToArray());
        if (x.IsFailure) return ApplicationResult<Guid>.Failure(EquipmentErrors.FromDomain(x.Error));
        await repository.AddAsync(x.Value, ct);
        await repository.SaveChangesAsync(ct);
        return ApplicationResult<Guid>.Success(x.Value.Id.Value);
    }
}
public sealed class ListWeaponsQuery(IWeaponRepository weapons, IEquipmentRepository items)
{
    public async Task<ApplicationResult<IReadOnlyCollection<WeaponDto>>> ExecuteAsync(CancellationToken ct)
    {
        var result = new List<WeaponDto>();
        foreach (var w in await weapons.ListAsync(ct))
        {
            var item = await items.GetByIdAsync(w.EquipmentItemId, ct);
            if (item is not null) result.Add(new(w.Id.Value, item.ToDto(), w.Category, w.DamageDice, w.DamageType, []));
        }
        return ApplicationResult<IReadOnlyCollection<WeaponDto>>.Success(result);
    }
}
public sealed class GetWeaponDetailsQuery(IWeaponRepository weapons, IEquipmentRepository items)
{
    public async Task<ApplicationResult<WeaponDto>> ExecuteAsync(Guid itemIdValue, CancellationToken ct)
    {
        var id = Ids.Parse(itemIdValue);
        if (id.IsFailure) return ApplicationResult<WeaponDto>.Failure(id.Error);
        var w = await weapons.GetByItemIdAsync(id.Value, ct);
        var item = await items.GetByIdAsync(id.Value, ct);
        if (w is null || item is null) return ApplicationResult<WeaponDto>.Failure(EquipmentErrors.NotFound("weapon", itemIdValue.ToString()));
        var properties = w.PropertyLinks
            .Select(property => new WeaponPropertyDto(
                property.WeaponPropertyId.Value,
                "",
                "",
                WeaponPropertyValueType.None,
                property.Values.Select(value => value.Value).ToArray()))
            .ToArray();

        return ApplicationResult<WeaponDto>.Success(new(
            w.Id.Value,
            item.ToDto(),
            w.Category,
            w.DamageDice,
            w.DamageType,
            properties));
    }
}

public sealed class CreateArmorUseCase(IEquipmentRepository items, IArmorRepository armors, IArmorTrainingCategoryRepository categories)
{
    public async Task<ApplicationResult<ArmorDto>> ExecuteAsync(CreateArmorCommand c, CancellationToken ct)
    {
        var itemId = Ids.Parse(c.EquipmentItemId);
        var categoryId = Ids.Parse(c.ArmorTrainingCategoryId);
        if (itemId.IsFailure || categoryId.IsFailure) return ApplicationResult<ArmorDto>.Failure(itemId.IsFailure ? itemId.Error : categoryId.Error);
        var item = await items.GetByIdAsync(itemId.Value, ct);
        if (item is null) return ApplicationResult<ArmorDto>.Failure(EquipmentErrors.NotFound("item", c.EquipmentItemId.ToString()));
        if (item.Category != EquipmentCategory.Armor) return ApplicationResult<ArmorDto>.Failure(EquipmentErrors.FromDomain(EquipmentDomainErrors.CategoryMismatch("Armor")));
        if (await categories.GetByIdAsync(categoryId.Value, ct) is null) return ApplicationResult<ArmorDto>.Failure(EquipmentErrors.NotFound("armor-category", c.ArmorTrainingCategoryId.ToString()));
        var armor = Armor.Create(itemId.Value, categoryId.Value);
        await armors.AddAsync(armor, ct);
        await armors.SaveChangesAsync(ct);
        return ApplicationResult<ArmorDto>.Success(new(armor.Id.Value, item.ToDto(), categoryId.Value.Value, [], []));
    }
}
public sealed class ConfigureArmorAcRuleUseCase(IArmorRepository armors)
{
    public async Task<ApplicationResult> ExecuteAsync(ConfigureArmorAcRuleCommand c, CancellationToken ct)
    {
        var id = Ids.Parse(c.EquipmentItemId);
        if (id.IsFailure) return ApplicationResult.Failure(id.Error);
        var armor = await armors.GetByItemIdAsync(id.Value, ct);
        if (armor is null) return ApplicationResult.Failure(EquipmentErrors.NotFound("armor", c.EquipmentItemId.ToString()));
        var ac = armor.ConfigureAcRule(c.BaseAc, c.AddsDexterity, c.MaximumDexterityBonus, c.Bonus);
        if (ac.IsFailure) return ApplicationResult.Failure(EquipmentErrors.FromDomain(ac.Error));
        foreach (var d in c.Drawbacks)
        {
            var r = armor.AddDrawback(d.Type, d.Threshold, d.Description);
            if (r.IsFailure) return ApplicationResult.Failure(EquipmentErrors.FromDomain(r.Error));
        }
        await armors.SaveChangesAsync(ct);
        return ApplicationResult.Success();
    }
}
public sealed class ListArmorsQuery(IArmorRepository armors, IEquipmentRepository items)
{
    public async Task<ApplicationResult<IReadOnlyCollection<ArmorDto>>> ExecuteAsync(CancellationToken ct)
    {
        var result = new List<ArmorDto>();
        foreach (var a in await armors.ListAsync(ct))
        {
            var item = await items.GetByIdAsync(a.EquipmentItemId, ct);
            if (item is not null) result.Add(Map(a, item));
        }
        return ApplicationResult<IReadOnlyCollection<ArmorDto>>.Success(result);
    }
    internal static ArmorDto Map(Armor armor, EquipmentItem item) =>
        new(
            armor.Id.Value,
            item.ToDto(),
            armor.ArmorTrainingCategoryId.Value,
            armor.AcRules
                .Select(rule => new ArmorAcRuleDto(
                    rule.BaseAc,
                    rule.AddsDexterity,
                    rule.MaximumDexterityBonus,
                    rule.Bonus))
                .ToArray(),
            armor.Drawbacks
                .Select(drawback => new ArmorDrawbackDto(
                    drawback.Type,
                    drawback.Threshold,
                    drawback.Description))
                .ToArray());
}
public sealed class GetArmorDetailsQuery(IArmorRepository armors, IEquipmentRepository items)
{
    public async Task<ApplicationResult<ArmorDto>> ExecuteAsync(Guid itemIdValue, CancellationToken ct)
    {
        var id = Ids.Parse(itemIdValue);
        if (id.IsFailure) return ApplicationResult<ArmorDto>.Failure(id.Error);
        var a = await armors.GetByItemIdAsync(id.Value, ct);
        var item = await items.GetByIdAsync(id.Value, ct);
        return a is null || item is null ? ApplicationResult<ArmorDto>.Failure(EquipmentErrors.NotFound("armor", itemIdValue.ToString())) : ApplicationResult<ArmorDto>.Success(ListArmorsQuery.Map(a, item));
    }
}
public sealed class CreateToolUseCase(IEquipmentRepository items, IToolRepository tools, IProficiencyRepository proficiencies)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(CreateToolCommand c, CancellationToken ct)
    {
        var itemId = Ids.Parse(c.EquipmentItemId);
        if (itemId.IsFailure) return ApplicationResult<Guid>.Failure(itemId.Error);
        var item = await items.GetByIdAsync(itemId.Value, ct);
        if (item is null) return ApplicationResult<Guid>.Failure(EquipmentErrors.NotFound("item", c.EquipmentItemId.ToString()));
        if (item.Category != EquipmentCategory.Tool) return ApplicationResult<Guid>.Failure(EquipmentErrors.FromDomain(EquipmentDomainErrors.CategoryMismatch("Tool")));
        CompendiumEntityId? proficiencyId = null;
        if (c.ProficiencyId.HasValue)
        {
            var p = Ids.Parse(c.ProficiencyId.Value);
            if (p.IsFailure) return ApplicationResult<Guid>.Failure(p.Error);
            if (!await proficiencies.ExistsByIdAsync(p.Value, ct)) return ApplicationResult<Guid>.Failure(EquipmentErrors.NotFound("proficiency", c.ProficiencyId.ToString()!));
            proficiencyId = p.Value;
        }
        var tool = Tool.Create(itemId.Value, proficiencyId, c.AbilityCode);
        await tools.AddAsync(tool, ct);
        await tools.SaveChangesAsync(ct);
        return ApplicationResult<Guid>.Success(tool.Id.Value);
    }
}
public sealed class CreateEquipmentPackUseCase(IEquipmentRepository items, IEquipmentPackRepository packs)
{
    public async Task<ApplicationResult<Guid>> ExecuteAsync(CreateEquipmentPackCommand c, CancellationToken ct)
    {
        var packItemId = Ids.Parse(c.EquipmentItemId);
        if (packItemId.IsFailure) return ApplicationResult<Guid>.Failure(packItemId.Error);
        var packItem = await items.GetByIdAsync(packItemId.Value, ct);
        if (packItem is null) return ApplicationResult<Guid>.Failure(EquipmentErrors.NotFound("item", c.EquipmentItemId.ToString()));
        if (packItem.Category != EquipmentCategory.Pack) return ApplicationResult<Guid>.Failure(EquipmentErrors.FromDomain(EquipmentDomainErrors.CategoryMismatch("Pack")));
        var inputs = new List<(CompendiumEntityId, int)>();
        foreach (var x in c.Items)
        {
            var id = Ids.Parse(x.EquipmentItemId);
            if (id.IsFailure) return ApplicationResult<Guid>.Failure(id.Error);
            if (await items.GetByIdAsync(id.Value, ct) is null) return ApplicationResult<Guid>.Failure(EquipmentErrors.NotFound("item", x.EquipmentItemId.ToString()));
            inputs.Add((id.Value, x.Quantity));
        }
        var pack = EquipmentPack.Create(packItemId.Value, inputs);
        if (pack.IsFailure) return ApplicationResult<Guid>.Failure(EquipmentErrors.FromDomain(pack.Error));
        await packs.AddAsync(pack.Value, ct);
        await packs.SaveChangesAsync(ct);
        return ApplicationResult<Guid>.Success(pack.Value.Id.Value);
    }
}
public sealed class CreateStartingEquipmentRuleUseCase(IEquipmentRepository items, IEquipmentPackRepository packs, IStartingEquipmentRuleRepository rules)
{
    public async Task<ApplicationResult<StartingEquipmentRuleDto>> ExecuteAsync(CreateStartingEquipmentRuleCommand c, CancellationToken ct)
    {
        var ownerId = Ids.Parse(c.OwnerEntityId);
        if (ownerId.IsFailure) return ApplicationResult<StartingEquipmentRuleDto>.Failure(ownerId.Error);
        if (await rules.GetAsync(c.OwnerType, ownerId.Value, ct) is not null) return ApplicationResult<StartingEquipmentRuleDto>.Failure(EquipmentErrors.Conflict("A starting equipment rule already exists for this owner."));
        var groups = new List<StartingEquipmentGroupInput>();
        foreach (var g in c.Groups)
        {
            var options = new List<StartingEquipmentOptionInput>();
            foreach (var o in g.Options)
            {
                var id = Ids.Parse(o.ReferenceId);
                if (id.IsFailure) return ApplicationResult<StartingEquipmentRuleDto>.Failure(id.Error);
                var exists = o.Type == StartingEquipmentOptionType.Item ? await items.GetByIdAsync(id.Value, ct) is not null : await packs.GetByIdAsync(id.Value, ct) is not null;
                if (!exists) return ApplicationResult<StartingEquipmentRuleDto>.Failure(EquipmentErrors.NotFound(o.Type.ToString(), o.ReferenceId.ToString()));
                options.Add(new(o.Type, id.Value, o.Quantity));
            }
            groups.Add(new(g.SelectionCount, options));
        }
        var rule = StartingEquipmentRule.Create(c.OwnerType, ownerId.Value, groups);
        if (rule.IsFailure) return ApplicationResult<StartingEquipmentRuleDto>.Failure(EquipmentErrors.FromDomain(rule.Error));
        await rules.AddAsync(rule.Value, ct);
        await rules.SaveChangesAsync(ct);
        return ApplicationResult<StartingEquipmentRuleDto>.Success(rule.Value.ToDto());
    }
}
public sealed class GetStartingEquipmentRuleQuery(IStartingEquipmentRuleRepository rules)
{
    public async Task<ApplicationResult<StartingEquipmentRuleDto>> ExecuteAsync(StartingEquipmentOwnerType ownerType, Guid ownerIdValue, CancellationToken ct)
    {
        var id = Ids.Parse(ownerIdValue);
        if (id.IsFailure) return ApplicationResult<StartingEquipmentRuleDto>.Failure(id.Error);
        var x = await rules.GetAsync(ownerType, id.Value, ct);
        return x is null ? ApplicationResult<StartingEquipmentRuleDto>.Failure(EquipmentErrors.NotFound("starting-rule", ownerIdValue.ToString())) : ApplicationResult<StartingEquipmentRuleDto>.Success(x.ToDto());
    }
}

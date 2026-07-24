using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Equipment;

public sealed class EquipmentItem
{
    private EquipmentItem() { RuleSourceId = SourceVersionId = null!; Code = null!; Name = null!; }
    private EquipmentItem(CompendiumEntityId id, CompendiumEntityId sourceId, CompendiumEntityId versionId, EquipmentCode code,
        EquipmentName name, EquipmentCategory category, Weight weight, Cost cost, string? description)
    {
        Id = id; RuleSourceId = sourceId; SourceVersionId = versionId; Code = code; Name = name; Category = category;
        Weight = weight.Pounds; CostAmount = cost.Amount; CostCurrency = cost.Currency; Description = description;
    }
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId RuleSourceId { get; private set; }
    public CompendiumEntityId SourceVersionId { get; private set; }
    public EquipmentCode Code { get; private set; }
    public EquipmentName Name { get; private set; }
    public EquipmentCategory Category { get; private set; }
    public decimal Weight { get; private set; }
    public decimal CostAmount { get; private set; }
    public Currency CostCurrency { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public static Result<EquipmentItem> Create(CompendiumEntityId sourceId, CompendiumEntityId versionId, EquipmentCode code,
        EquipmentName name, EquipmentCategory category, Weight weight, Cost cost, string? description, DateTimeOffset now)
    {
        if (!Enum.IsDefined(category)) return Result<EquipmentItem>.Failure(EquipmentDomainErrors.Invalid("category"));
        if (description?.Length > 4000) return Result<EquipmentItem>.Failure(EquipmentDomainErrors.TooLong("description", 4000));
        return Result<EquipmentItem>.Success(new EquipmentItem(CompendiumEntityId.New(), sourceId, versionId, code, name, category, weight, cost, description?.Trim())
            { CreatedAtUtc = now, UpdatedAtUtc = now });
    }

    public Result Update(CompendiumEntityId versionId, EquipmentName name, EquipmentCategory category, Weight weight, Cost cost,
        string? description, DateTimeOffset now)
    {
        if (!Enum.IsDefined(category)) return Result.Failure(EquipmentDomainErrors.Invalid("category"));
        SourceVersionId = versionId; Name = name; Category = category; Weight = weight.Pounds; CostAmount = cost.Amount;
        CostCurrency = cost.Currency; Description = description?.Trim(); UpdatedAtUtc = now; return Result.Success();
    }
}

using Compendium.Domain.Equipment;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Equipment;

public interface IEquipmentRepository
{
    Task AddAsync(EquipmentItem item, CancellationToken ct);
    Task<EquipmentItem?> GetByIdAsync(CompendiumEntityId id, CancellationToken ct);
    Task<EquipmentItem?> GetByCodeAsync(EquipmentCode code, CancellationToken ct);
    Task<bool> ExistsByCodeAsync(EquipmentCode code, CancellationToken ct);
    Task<IReadOnlyCollection<EquipmentItem>> ListAsync(EquipmentCategory? category, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
public interface IWeaponRepository
{
    Task AddAsync(Weapon weapon, CancellationToken ct);
    Task<Weapon?> GetByItemIdAsync(CompendiumEntityId itemId, CancellationToken ct);
    Task<IReadOnlyCollection<Weapon>> ListAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
public interface IWeaponPropertyRepository
{
    Task AddAsync(WeaponProperty property, CancellationToken ct);
    Task<WeaponProperty?> GetByIdAsync(CompendiumEntityId id, CancellationToken ct);
    Task<IReadOnlyCollection<WeaponProperty>> ListAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
public interface IWeaponMasteryRepository
{
    Task AddAsync(WeaponMasteryProperty mastery, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
public interface IArmorRepository
{
    Task AddAsync(Armor armor, CancellationToken ct);
    Task<Armor?> GetByItemIdAsync(CompendiumEntityId itemId, CancellationToken ct);
    Task<IReadOnlyCollection<Armor>> ListAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
public interface IToolRepository
{
    Task AddAsync(Tool tool, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
public interface IEquipmentPackRepository
{
    Task AddAsync(EquipmentPack pack, CancellationToken ct);
    Task<EquipmentPack?> GetByIdAsync(CompendiumEntityId id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
public interface IStartingEquipmentRuleRepository
{
    Task AddAsync(StartingEquipmentRule rule, CancellationToken ct);
    Task<StartingEquipmentRule?> GetAsync(StartingEquipmentOwnerType ownerType, CompendiumEntityId ownerId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

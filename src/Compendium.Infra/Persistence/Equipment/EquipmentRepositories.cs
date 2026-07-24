using Compendium.Application.Equipment;
using Compendium.Domain.Equipment;
using Compendium.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence.Equipment;

internal sealed class EquipmentRepository(CompendiumDbContext db):IEquipmentRepository
{
    public async Task AddAsync(EquipmentItem x,CancellationToken ct)=>await db.EquipmentItems.AddAsync(x,ct);
    public Task<EquipmentItem?> GetByIdAsync(CompendiumEntityId id,CancellationToken ct)=>db.EquipmentItems.SingleOrDefaultAsync(x=>x.Id==id,ct);
    public Task<EquipmentItem?> GetByCodeAsync(EquipmentCode code,CancellationToken ct)=>db.EquipmentItems.SingleOrDefaultAsync(x=>x.Code==code,ct);
    public Task<bool> ExistsByCodeAsync(EquipmentCode code,CancellationToken ct)=>db.EquipmentItems.AnyAsync(x=>x.Code==code,ct);
    public async Task<IReadOnlyCollection<EquipmentItem>> ListAsync(EquipmentCategory? category,CancellationToken ct){var q=db.EquipmentItems.AsQueryable();if(category.HasValue)q=q.Where(x=>x.Category==category);return await q.OrderBy(x=>x.Category).ThenBy(x=>x.Code).ToArrayAsync(ct);}
    public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);
}
internal sealed class WeaponRepository(CompendiumDbContext db):IWeaponRepository
{
    private IQueryable<Weapon> Full=>db.Weapons.Include(x=>x.PropertyLinks).ThenInclude(x=>x.Values);
    public async Task AddAsync(Weapon x,CancellationToken ct)=>await db.Weapons.AddAsync(x,ct);public Task<Weapon?> GetByItemIdAsync(CompendiumEntityId id,CancellationToken ct)=>Full.SingleOrDefaultAsync(x=>x.EquipmentItemId==id,ct);
    public async Task<IReadOnlyCollection<Weapon>> ListAsync(CancellationToken ct)=>await Full.ToArrayAsync(ct);public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);
}
internal sealed class WeaponPropertyRepository(CompendiumDbContext db):IWeaponPropertyRepository
{
    public async Task AddAsync(WeaponProperty x,CancellationToken ct)=>await db.WeaponProperties.AddAsync(x,ct);public Task<WeaponProperty?> GetByIdAsync(CompendiumEntityId id,CancellationToken ct)=>db.WeaponProperties.Include(x=>x.Rules).SingleOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<IReadOnlyCollection<WeaponProperty>> ListAsync(CancellationToken ct)=>await db.WeaponProperties.Include(x=>x.Rules).OrderBy(x=>x.Code).ToArrayAsync(ct);public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);
}
internal sealed class WeaponMasteryRepository(CompendiumDbContext db):IWeaponMasteryRepository{public async Task AddAsync(WeaponMasteryProperty x,CancellationToken ct)=>await db.WeaponMasteryProperties.AddAsync(x,ct);public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);}
internal sealed class ArmorRepository(CompendiumDbContext db):IArmorRepository
{
    private IQueryable<Armor> Full=>db.Armors.Include(x=>x.AcRules).Include(x=>x.Drawbacks);public async Task AddAsync(Armor x,CancellationToken ct)=>await db.Armors.AddAsync(x,ct);public Task<Armor?> GetByItemIdAsync(CompendiumEntityId id,CancellationToken ct)=>Full.SingleOrDefaultAsync(x=>x.EquipmentItemId==id,ct);public async Task<IReadOnlyCollection<Armor>> ListAsync(CancellationToken ct)=>await Full.ToArrayAsync(ct);public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);
}
internal sealed class ToolRepository(CompendiumDbContext db):IToolRepository{public async Task AddAsync(Tool x,CancellationToken ct)=>await db.Tools.AddAsync(x,ct);public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);}
internal sealed class EquipmentPackRepository(CompendiumDbContext db):IEquipmentPackRepository{public async Task AddAsync(EquipmentPack x,CancellationToken ct)=>await db.EquipmentPacks.AddAsync(x,ct);public Task<EquipmentPack?> GetByIdAsync(CompendiumEntityId id,CancellationToken ct)=>db.EquipmentPacks.Include(x=>x.Items).SingleOrDefaultAsync(x=>x.Id==id,ct);public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);}
internal sealed class StartingEquipmentRuleRepository(CompendiumDbContext db):IStartingEquipmentRuleRepository
{
    public async Task AddAsync(StartingEquipmentRule x,CancellationToken ct)=>await db.StartingEquipmentRules.AddAsync(x,ct);
    public Task<StartingEquipmentRule?> GetAsync(StartingEquipmentOwnerType type,CompendiumEntityId id,CancellationToken ct)=>db.StartingEquipmentRules.Include(x=>x.Groups).ThenInclude(x=>x.Options).SingleOrDefaultAsync(x=>x.OwnerType==type&&x.OwnerEntityId==id,ct);
    public Task SaveChangesAsync(CancellationToken ct)=>db.SaveChangesAsync(ct);
}

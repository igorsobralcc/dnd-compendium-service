using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Equipment;

public sealed class Armor
{
    private readonly List<ArmorAcRule> acRules = [];
    private readonly List<ArmorDrawback> drawbacks = [];
    private Armor() { EquipmentItemId = ArmorTrainingCategoryId = null!; }
    private Armor(CompendiumEntityId id, CompendiumEntityId itemId, CompendiumEntityId categoryId)
    { Id = id; EquipmentItemId = itemId; ArmorTrainingCategoryId = categoryId; }
    public CompendiumEntityId Id { get; private set; } = null!;
    public CompendiumEntityId EquipmentItemId { get; private set; }
    public CompendiumEntityId ArmorTrainingCategoryId { get; private set; }
    public IReadOnlyCollection<ArmorAcRule> AcRules => acRules;
    public IReadOnlyCollection<ArmorDrawback> Drawbacks => drawbacks;
    public static Armor Create(CompendiumEntityId itemId, CompendiumEntityId categoryId) => new(CompendiumEntityId.New(), itemId, categoryId);
    public Result ConfigureAcRule(int baseAc, bool addsDexterity, int? maximumDexterityBonus, int bonus)
    {
        if (baseAc <= 0 || maximumDexterityBonus < 0 || (!addsDexterity && maximumDexterityBonus.HasValue))
            return Result.Failure(EquipmentDomainErrors.Invalid("armor-ac-rule"));
        acRules.Clear(); acRules.Add(ArmorAcRule.Create(Id, baseAc, addsDexterity, maximumDexterityBonus, bonus)); return Result.Success();
    }
    public Result AddDrawback(ArmorDrawbackType type, int? threshold, string? description)
    {
        if (!Enum.IsDefined(type) || (type == ArmorDrawbackType.StrengthRequirement && threshold is null or <= 0))
            return Result.Failure(EquipmentDomainErrors.Invalid("armor-drawback"));
        drawbacks.Add(ArmorDrawback.Create(Id, type, threshold, description)); return Result.Success();
    }
}

public sealed class ArmorAcRule
{
    private ArmorAcRule() { ArmorId = null!; }
    private ArmorAcRule(CompendiumEntityId id, CompendiumEntityId armorId, int baseAc, bool addsDexterity, int? maximumDexterityBonus, int bonus)
    { Id=id; ArmorId=armorId; BaseAc=baseAc; AddsDexterity=addsDexterity; MaximumDexterityBonus=maximumDexterityBonus; Bonus=bonus; }
    public CompendiumEntityId Id { get; private set; }=null!;
    public CompendiumEntityId ArmorId { get; private set; }
    public int BaseAc { get; private set; }
    public bool AddsDexterity { get; private set; }
    public int? MaximumDexterityBonus { get; private set; }
    public int Bonus { get; private set; }
    internal static ArmorAcRule Create(CompendiumEntityId armorId,int baseAc,bool addsDexterity,int? maximumDexterityBonus,int bonus)=>
        new(CompendiumEntityId.New(),armorId,baseAc,addsDexterity,maximumDexterityBonus,bonus);
}

public sealed class ArmorDrawback
{
    private ArmorDrawback() { ArmorId=null!; }
    private ArmorDrawback(CompendiumEntityId id,CompendiumEntityId armorId,ArmorDrawbackType type,int? threshold,string? description)
    {Id=id;ArmorId=armorId;Type=type;Threshold=threshold;Description=description?.Trim();}
    public CompendiumEntityId Id{get;private set;}=null!;
    public CompendiumEntityId ArmorId{get;private set;}
    public ArmorDrawbackType Type{get;private set;}
    public int? Threshold{get;private set;}
    public string? Description{get;private set;}
    internal static ArmorDrawback Create(CompendiumEntityId armorId,ArmorDrawbackType type,int? threshold,string? description)=>
        new(CompendiumEntityId.New(),armorId,type,threshold,description);
}

using Compendium.Domain.SharedKernel;

namespace Compendium.Domain.Equipment;

public sealed class Tool
{
    private Tool() { EquipmentItemId=null!; }
    private Tool(CompendiumEntityId id,CompendiumEntityId itemId,CompendiumEntityId? proficiencyId,string? abilityCode)
    {Id=id;EquipmentItemId=itemId;ProficiencyId=proficiencyId;AbilityCode=abilityCode?.Trim().ToUpperInvariant();}
    public CompendiumEntityId Id{get;private set;}=null!;
    public CompendiumEntityId EquipmentItemId{get;private set;}
    public CompendiumEntityId? ProficiencyId{get;private set;}
    public string? AbilityCode{get;private set;}
    public static Tool Create(CompendiumEntityId itemId,CompendiumEntityId? proficiencyId,string? abilityCode)=>new(CompendiumEntityId.New(),itemId,proficiencyId,abilityCode);
}

public sealed class EquipmentPack
{
    private readonly List<EquipmentPackItem> items=[];
    private EquipmentPack(){EquipmentItemId=null!;}
    private EquipmentPack(CompendiumEntityId id,CompendiumEntityId equipmentItemId){Id=id;EquipmentItemId=equipmentItemId;}
    public CompendiumEntityId Id{get;private set;}=null!;
    public CompendiumEntityId EquipmentItemId{get;private set;}
    public IReadOnlyCollection<EquipmentPackItem> Items=>items;
    public static Result<EquipmentPack> Create(CompendiumEntityId equipmentItemId,IReadOnlyCollection<(CompendiumEntityId ItemId,int Quantity)> inputs)
    {
        if(inputs.Count==0||inputs.Any(x=>x.Quantity<=0)||inputs.GroupBy(x=>x.ItemId).Any(x=>x.Count()>1))
            return Result<EquipmentPack>.Failure(EquipmentDomainErrors.EmptyPack());
        var pack=new EquipmentPack(CompendiumEntityId.New(),equipmentItemId);
        foreach(var x in inputs)pack.items.Add(EquipmentPackItem.Create(pack.Id,x.ItemId,x.Quantity));
        return Result<EquipmentPack>.Success(pack);
    }
}
public sealed class EquipmentPackItem
{
    private EquipmentPackItem(){EquipmentPackId=EquipmentItemId=null!;}
    private EquipmentPackItem(CompendiumEntityId id,CompendiumEntityId packId,CompendiumEntityId itemId,int quantity){Id=id;EquipmentPackId=packId;EquipmentItemId=itemId;Quantity=quantity;}
    public CompendiumEntityId Id{get;private set;}=null!; public CompendiumEntityId EquipmentPackId{get;private set;} public CompendiumEntityId EquipmentItemId{get;private set;} public int Quantity{get;private set;}
    internal static EquipmentPackItem Create(CompendiumEntityId packId,CompendiumEntityId itemId,int quantity)=>new(CompendiumEntityId.New(),packId,itemId,quantity);
}

public sealed class StartingEquipmentRule
{
    private readonly List<StartingEquipmentGroup> groups=[];
    private StartingEquipmentRule(){OwnerEntityId=null!;}
    private StartingEquipmentRule(CompendiumEntityId id,StartingEquipmentOwnerType ownerType,CompendiumEntityId ownerId){Id=id;OwnerType=ownerType;OwnerEntityId=ownerId;}
    public CompendiumEntityId Id{get;private set;}=null!; public StartingEquipmentOwnerType OwnerType{get;private set;} public CompendiumEntityId OwnerEntityId{get;private set;}
    public IReadOnlyCollection<StartingEquipmentGroup> Groups=>groups;
    public static Result<StartingEquipmentRule> Create(StartingEquipmentOwnerType ownerType,CompendiumEntityId ownerId,IReadOnlyCollection<StartingEquipmentGroupInput> inputs)
    {
        if(!Enum.IsDefined(ownerType)||inputs.Count==0)return Result<StartingEquipmentRule>.Failure(EquipmentDomainErrors.EmptyStartingRule());
        var rule=new StartingEquipmentRule(CompendiumEntityId.New(),ownerType,ownerId);
        var ordinal=0;
        foreach(var input in inputs)
        {
            var group=StartingEquipmentGroup.Create(rule.Id,ordinal++,input.SelectionCount,input.Options);
            if(group.IsFailure)return Result<StartingEquipmentRule>.Failure(group.Error);
            rule.groups.Add(group.Value);
        }
        return Result<StartingEquipmentRule>.Success(rule);
    }
}
public sealed class StartingEquipmentGroup
{
    private readonly List<StartingEquipmentOption> options=[];
    private StartingEquipmentGroup(){StartingEquipmentRuleId=null!;}
    private StartingEquipmentGroup(CompendiumEntityId id,CompendiumEntityId ruleId,int ordinal,int selectionCount){Id=id;StartingEquipmentRuleId=ruleId;Ordinal=ordinal;SelectionCount=selectionCount;}
    public CompendiumEntityId Id{get;private set;}=null!; public CompendiumEntityId StartingEquipmentRuleId{get;private set;} public int Ordinal{get;private set;} public int SelectionCount{get;private set;}
    public IReadOnlyCollection<StartingEquipmentOption> Options=>options;
    internal static Result<StartingEquipmentGroup> Create(CompendiumEntityId ruleId,int ordinal,int selectionCount,IReadOnlyCollection<StartingEquipmentOptionInput> inputs)
    {
        if(selectionCount<=0||inputs.Count<selectionCount)return Result<StartingEquipmentGroup>.Failure(EquipmentDomainErrors.InvalidCardinality());
        var group=new StartingEquipmentGroup(CompendiumEntityId.New(),ruleId,ordinal,selectionCount);
        var optionOrdinal=0;
        foreach(var input in inputs)
        {
            if(!Enum.IsDefined(input.Type)||input.Quantity<=0)return Result<StartingEquipmentGroup>.Failure(EquipmentDomainErrors.Invalid("starting-option"));
            group.options.Add(StartingEquipmentOption.Create(group.Id,optionOrdinal++,input.Type,input.ReferenceId,input.Quantity));
        }
        return Result<StartingEquipmentGroup>.Success(group);
    }
}
public sealed class StartingEquipmentOption
{
    private StartingEquipmentOption(){StartingEquipmentGroupId=ReferenceId=null!;}
    private StartingEquipmentOption(CompendiumEntityId id,CompendiumEntityId groupId,int ordinal,StartingEquipmentOptionType type,CompendiumEntityId referenceId,int quantity)
    {Id=id;StartingEquipmentGroupId=groupId;Ordinal=ordinal;Type=type;ReferenceId=referenceId;Quantity=quantity;}
    public CompendiumEntityId Id{get;private set;}=null!;public CompendiumEntityId StartingEquipmentGroupId{get;private set;}public int Ordinal{get;private set;}
    public StartingEquipmentOptionType Type{get;private set;}public CompendiumEntityId ReferenceId{get;private set;}public int Quantity{get;private set;}
    internal static StartingEquipmentOption Create(CompendiumEntityId groupId,int ordinal,StartingEquipmentOptionType type,CompendiumEntityId referenceId,int quantity)=>new(CompendiumEntityId.New(),groupId,ordinal,type,referenceId,quantity);
}
public sealed record StartingEquipmentGroupInput(int SelectionCount,IReadOnlyCollection<StartingEquipmentOptionInput> Options);
public sealed record StartingEquipmentOptionInput(StartingEquipmentOptionType Type,CompendiumEntityId ReferenceId,int Quantity);

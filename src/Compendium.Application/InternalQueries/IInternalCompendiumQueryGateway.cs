namespace Compendium.Application.InternalQueries;

public interface IInternalCompendiumQueryGateway
{
    Task<CharacterCreationOptionsV1> GetCharacterCreationOptionsAsync(
        CharacterCreationOptionsRequest request,
        CancellationToken cancellationToken);

    Task<MechanicalEntityDetailsV1?> GetMechanicalEntityDetailsAsync(
        string entityType,
        Guid entityId,
        string locale,
        CancellationToken cancellationToken);

    Task<CompendiumChangesV1> ListChangesAsync(
        CompendiumChangesRequest request,
        CancellationToken cancellationToken);
}

public sealed class GetCharacterCreationOptionsQuery(IInternalCompendiumQueryGateway gateway)
{
    public Task<CharacterCreationOptionsV1> ExecuteAsync(CharacterCreationOptionsRequest request, CancellationToken cancellationToken) =>
        gateway.GetCharacterCreationOptionsAsync(request, cancellationToken);
}

public sealed class GetMechanicalEntityDetailsQuery(IInternalCompendiumQueryGateway gateway)
{
    public Task<MechanicalEntityDetailsV1?> ExecuteAsync(string entityType, Guid entityId, string locale, CancellationToken cancellationToken) =>
        gateway.GetMechanicalEntityDetailsAsync(entityType, entityId, locale, cancellationToken);
}

public sealed class ListCompendiumChangesSinceQuery(IInternalCompendiumQueryGateway gateway)
{
    public Task<CompendiumChangesV1> ExecuteAsync(CompendiumChangesRequest request, CancellationToken cancellationToken) =>
        gateway.ListChangesAsync(request, cancellationToken);
}

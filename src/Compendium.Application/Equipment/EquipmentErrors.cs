using Compendium.Application.Errors;
using Compendium.Domain.SharedKernel;

namespace Compendium.Application.Equipment;

internal static class EquipmentErrors
{
    public static ApplicationError FromDomain(DomainError e)=>new(e.Code,e.Message);
    public static ApplicationError NotFound(string entity,string id)=>new($"equipment.{entity}.not-found",$"{entity} '{id}' was not found.",ApplicationErrorKind.NotFound);
    public static ApplicationError Conflict(string message)=>new("equipment.conflict",message,ApplicationErrorKind.Conflict);
}

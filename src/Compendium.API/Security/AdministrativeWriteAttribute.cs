using Compendium.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;

namespace Compendium.API.Security;

public sealed class AdministrativeWriteAttribute : AuthorizeAttribute
{
    public AdministrativeWriteAttribute()
    {
        Policy = CompendiumSecurity.AdministrativeWritePolicy;
    }
}

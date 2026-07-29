using Compendium.CrossCutting.Security;
using Microsoft.AspNetCore.Authorization;

namespace Compendium.API.Security;

public sealed class InternalReadAttribute : AuthorizeAttribute
{
    public InternalReadAttribute()
    {
        Policy = CompendiumSecurity.InternalReadPolicy;
    }
}

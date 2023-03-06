using Microsoft.AspNetCore.Authorization;

namespace Home.API.API.Filters;

public sealed class RequireScope : IAuthorizationRequirement
{
    public List<string> Scopes { get; }

    public RequireScope(params string[] scopeNames)
    {
        Scopes = scopeNames?.ToList() ?? new List<string>();
    }
}

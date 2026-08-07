using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Calendar.Domain.Model;

public readonly record struct UserId(Guid Value) /*: IBindableFromHttpContext<UserId>*/
{
    public static ValueTask<UserId?> BindAsync(HttpContext context, ParameterInfo _)
    {
        var principal = context.User;
        var claimValue = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? principal.FindFirstValue("sub");

        if (Guid.TryParse(claimValue, out var id))
        {
            return ValueTask.FromResult<UserId?>(new UserId(id));
        }

        return ValueTask.FromResult<UserId?>(null);
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
}

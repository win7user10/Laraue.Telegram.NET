using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Static implementation of <see cref="IUserRoleProvider"/> based on const
/// list of groups and users.
/// </summary>
public class StaticUserRoleProvider : IUserRoleProvider
{
    private readonly Dictionary<string, string[]> _userRoles;
    
    /// <summary>
    /// Initializes a new instance of <see cref="StaticUserRoleProvider"/>.
    /// </summary>
    public StaticUserRoleProvider(IOptions<RoleUsers> roleUsers)
    {
        _userRoles = roleUsers
            .Value
            .SelectMany(x => x.Value, (pair, nickName) => (pair.Key, nickName))
            .GroupBy(x => x.nickName, x => x.Key)
            .ToDictionary(x => x.Key, x => x.ToArray());
    }
    
    /// <inheritdoc />
    public Task<IList<UserRole>> GetUserGroupsAsync(User user)
    {
        var result = _userRoles.TryGetValue(user.Username ?? string.Empty, out var groups)
            ? groups.Select(x => new UserRole(x)).ToList()
            : new List<UserRole>();

        return Task.FromResult((IList<UserRole>)result);
    }
}

/// <summary>
/// Map between group name and telegram user names that belongs to this group.
/// </summary>
public sealed class RoleUsers : Dictionary<string, string[]>
{}
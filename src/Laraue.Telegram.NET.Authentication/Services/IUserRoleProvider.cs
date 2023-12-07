using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Authentication.Services;

/// <summary>
/// Provides information about groups the telegram user belongs to.
/// </summary>
public interface IUserRoleProvider
{
    /// <summary>
    /// Returns user groups list for the user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<IList<UserRole>> GetUserGroupsAsync(User user);
}

/// <summary>
/// User group entity.
/// </summary>
/// <param name="Name"></param>
public sealed record UserRole(string Name);
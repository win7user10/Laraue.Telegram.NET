using Laraue.Telegram.NET.Core.Routing;

namespace Laraue.Telegram.NET.Authentication.Attributes;

/// <summary>
/// The attribute for <see cref="TelegramController"/> methods.
/// The method marked with attribute will require from the user to be
/// in one of the specified group.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class RequiresUserGroupAttribute : Attribute
{
    /// <summary>
    /// List of the allowed groups.
    /// </summary>
    public string[] AllowedGroups { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="RequiresUserGroupAttribute"/>.
    /// </summary>
    /// <param name="allowedGroups">List of group names.</param>
    public RequiresUserGroupAttribute(params string[] allowedGroups)
    {
        AllowedGroups = allowedGroups;
    }
}
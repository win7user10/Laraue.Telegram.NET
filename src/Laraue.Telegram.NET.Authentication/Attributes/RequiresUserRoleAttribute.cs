using Laraue.Telegram.NET.Core.Routing;

namespace Laraue.Telegram.NET.Authentication.Attributes;

/// <summary>
/// The attribute for <see cref="TelegramController"/> and its methods.
/// The controller or it's method marked with attribute will require from the user to have
/// of the specified roles.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class RequiresUserRoleAttribute : Attribute
{
    /// <summary>
    /// List of the allowed groups.
    /// </summary>
    public string[] AllowedRoles { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="RequiresUserRoleAttribute"/>.
    /// </summary>
    /// <param name="allowedRoles">List of allowed role names.</param>
    public RequiresUserRoleAttribute(params string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
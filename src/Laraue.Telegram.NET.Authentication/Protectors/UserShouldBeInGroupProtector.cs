using System.Reflection;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Attributes;
using Laraue.Telegram.NET.Authentication.Services;

namespace Laraue.Telegram.NET.Authentication.Protectors;

/// <inheritdoc />
public sealed class UserShouldBeInGroupProtector<TUserKey> : IControllerProtector
    where TUserKey : IEquatable<TUserKey>
{
    private readonly TelegramRequestContext<TUserKey> _requestContext;

    /// <summary>
    /// Initializes a new instance of <see cref="UserShouldBeInGroupProtector{TUserKey}"/>.
    /// </summary>
    /// <param name="requestContext"></param>
    public UserShouldBeInGroupProtector(TelegramRequestContext<TUserKey> requestContext)
    {
        _requestContext = requestContext;
    }
    
    /// <inheritdoc />
    public bool IsExecutionAllowed(MethodInfo controllerMethod)
    {
        var attribute = controllerMethod.GetCustomAttribute<RequiresUserRoleAttribute>();
        if (attribute is null)
        {
            var controllerType = controllerMethod.DeclaringType;
            attribute = controllerType?.GetCustomAttribute<RequiresUserRoleAttribute>();
        }
        
        return attribute is null || attribute.AllowedRoles.Any(x => _requestContext.Groups.Contains(x));
    }
}
using System.Reflection;

namespace Laraue.Telegram.NET.Abstractions;

/// <summary>
/// Protects controller methods based on specified rules.
/// </summary>
public interface IControllerProtector
{
    /// <summary>
    /// Returns true if method execution is allowed for the current user.
    /// </summary>
    public bool IsExecutionAllowed(MethodInfo controllerMethod);
}
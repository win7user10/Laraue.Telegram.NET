namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// <see cref="TelegramController"/> method.
/// </summary>
public enum RouteMethod
{
    /// <summary>
    /// Retrieve resource representation/information only
    /// </summary>
    Get,
    
    /// <summary>
    /// Create new subordinate resources
    /// </summary>
    Post,
    
    /// <summary>
    /// Update an existing resource
    /// </summary>
    Put,
    
    /// <summary>
    /// Delete the resources
    /// </summary>
    Delete,
    
    /// <summary>
    /// Make a partial update
    /// </summary>
    Patch
}
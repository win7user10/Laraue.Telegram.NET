namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// Something wrong with parameters binding.
/// </summary>
public class BindException : Exception
{
    /// <summary>
    /// Property that bind has been failed.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="BindException"/>.
    /// </summary>
    public BindException(string propertyName) : base($"Bind {propertyName} has been failed")
    {
        PropertyName = propertyName;
    }
}
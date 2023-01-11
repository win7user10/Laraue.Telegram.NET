using System.Net;
using Laraue.Core.Exceptions.Web;

namespace Laraue.Telegram.NET.Abstractions.Exceptions;

/// <summary>
/// Unauthorized access.
/// </summary>
public class UnauthorizedRequestException : HttpExceptionWithErrors
{
    /// <summary>
    /// Initializes a <see cref="UnauthorizedRequestException"/>.
    /// </summary>
    /// <param name="errors"></param>
    public UnauthorizedRequestException(IReadOnlyDictionary<string, string[]> errors) 
        : base(HttpStatusCode.Unauthorized, errors)
    {
    }
}
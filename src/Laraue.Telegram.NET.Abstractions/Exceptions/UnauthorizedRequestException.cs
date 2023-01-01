using System.Net;
using Laraue.Core.Exceptions.Web;

namespace Laraue.Telegram.NET.Abstractions.Exceptions;

public class UnauthorizedRequestException : HttpExceptionWithErrors
{
    public UnauthorizedRequestException(IReadOnlyDictionary<string, string[]> errors) 
        : base(HttpStatusCode.Unauthorized, errors)
    {
    }
}
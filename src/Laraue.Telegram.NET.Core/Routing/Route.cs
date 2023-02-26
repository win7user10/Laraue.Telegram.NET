using System.Reflection;
using Laraue.Telegram.NET.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Routing;

internal sealed class Route : IRoute
{
    private readonly Func<Update, bool> _tryMatchRoute;
    private readonly MethodInfo _controllerMethod;

    public Route(Func<Update, bool> tryMatchRoute, MethodInfo controllerMethod)
    {
        _tryMatchRoute = tryMatchRoute;
        _controllerMethod = controllerMethod;
    }
    
    public ValueTask<RouteExecutionResult> TryExecuteAsync(IServiceProvider requestProvider)
    {
        return _tryMatchRoute(requestProvider.GetRequiredService<TelegramRequestContext>().Update)
            ? ExecuteAsync(requestProvider)
            : ValueTask.FromResult(new RouteExecutionResult(false, null));
    }

    private async ValueTask<RouteExecutionResult> ExecuteAsync(IServiceProvider serviceProvider)
    {
        var result = _controllerMethod.Invoke(
            serviceProvider.GetRequiredService(_controllerMethod.DeclaringType!),
            GetRouteParameters(serviceProvider, _controllerMethod));

        if (result is null)
        {
            return new RouteExecutionResult(true, null);
        }
                        
        if ((_controllerMethod.ReturnType.BaseType == typeof(Task) || _controllerMethod.ReturnType.BaseType == typeof(ValueTask))
            && _controllerMethod.ReturnType.GenericTypeArguments.Length == 1)
        {
            result = await (dynamic) result;
        }

        else if (_controllerMethod.ReturnType == typeof(Task) || _controllerMethod.ReturnType == typeof(ValueTask))
        {
            await (dynamic) result;

            return new RouteExecutionResult(true, null);
        }

        return new RouteExecutionResult(true, result);
    }
    
    private static object[] GetRouteParameters(
        IServiceProvider serviceProvider,
        MethodBase methodInfo)
    {
        var methodParameters = methodInfo.GetParameters();
        var parameters = new object[methodParameters.Length];
        
        for (var i = 0; i < methodParameters.Length; i++)
        {
            var methodParameter = methodParameters[i];
            parameters[i] = serviceProvider.GetRequiredService(methodParameter.ParameterType);
        }

        return parameters;
    }

    public override string ToString()
    {
        return _controllerMethod.Name;
    }
}
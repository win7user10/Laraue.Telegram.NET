using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Abstractions.Request;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Routing;

internal sealed class Route : IRoute
{
    private readonly TryMatch _tryMatchDelegate;
    private readonly MethodInfo _controllerMethod;

    public Route(TryMatch tryMatchDelegate, MethodInfo controllerMethod)
    {
        _tryMatchDelegate = tryMatchDelegate;
        _controllerMethod = controllerMethod;
    }

    public delegate bool TryMatch(Update update, [NotNullWhen(true)] out RequestParameters? requestParameters);
    
    public ValueTask<RouteExecutionResult> TryExecuteAsync(IServiceProvider requestProvider, CancellationToken ct = default)
    {
        return _tryMatchDelegate(requestProvider.GetRequiredService<TelegramRequestContext>().Update, out var requestParameters)
            ? ExecuteAsync(requestProvider, requestParameters, ct)
            : ValueTask.FromResult(new RouteExecutionResult(false, null));
    }

    private async ValueTask<RouteExecutionResult> ExecuteAsync(
        IServiceProvider serviceProvider,
        RequestParameters requestParameters,
        CancellationToken ct)
    {
        var result = _controllerMethod.Invoke(
            serviceProvider.GetRequiredService(_controllerMethod.DeclaringType!),
            GetRouteParameters(serviceProvider, _controllerMethod, requestParameters, ct));

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
    
    private static object?[] GetRouteParameters(
        IServiceProvider serviceProvider,
        MethodBase methodInfo,
        RequestParameters requestParameters,
        CancellationToken ct)
    {
        var methodParameters = methodInfo.GetParameters();
        var parameters = new object?[methodParameters.Length];
        
        for (var i = 0; i < methodParameters.Length; i++)
        {
            var methodParameter = methodParameters[i];
            object? parameterValue;
            if (methodParameter.ParameterType == typeof(CancellationToken))
            {
                parameterValue = ct;
            }
            else if (methodParameter.GetCustomAttribute<FromPathAttribute>() is not null)
            {
                parameterValue = requestParameters.GetPathParameter(
                    methodParameter.Name!, methodParameter.ParameterType)
                    ?? throw new InvalidOperationException($"Parameter {methodParameter.Name} hasn't been found in route");
            }
            else if (methodParameter.GetCustomAttribute<FromQueryAttribute>() is not null)
            {
                if (methodParameter.ParameterType.IsValueType || methodParameter.ParameterType == typeof(string))
                {
                    parameterValue = requestParameters.GetQueryParameter(
                        methodParameter.Name!, methodParameter.ParameterType);
                }
                else
                {
                    parameterValue = requestParameters.GetQueryParameters(methodParameter.ParameterType);
                }
            }
            else
            {
                parameterValue = serviceProvider.GetRequiredService(methodParameter.ParameterType);
            }
            
            parameters[i] = parameterValue;
        }

        return parameters;
    }

    public override string ToString()
    {
        return _controllerMethod.Name;
    }
}
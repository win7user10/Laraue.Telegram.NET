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
            : ValueTask.FromResult(new RouteExecutionResult(false));
    }

    private async ValueTask<RouteExecutionResult> ExecuteAsync(
        IServiceProvider serviceProvider,
        RequestParameters requestParameters,
        CancellationToken ct)
    {
        var protectors = serviceProvider.GetRequiredService<IEnumerable<IControllerProtector>>();
        if (protectors.Any(protector => !protector.IsExecutionAllowed(_controllerMethod)))
        {
            return new RouteExecutionResult(false);
        }
        
        var result = _controllerMethod.Invoke(
            serviceProvider.GetRequiredService(_controllerMethod.DeclaringType!),
            GetRouteParameters(serviceProvider, _controllerMethod, requestParameters, ct));

        if (result is null)
        {
            return new RouteExecutionResult(true);
        }
                        
        if ((_controllerMethod.ReturnType.BaseType == typeof(Task) || _controllerMethod.ReturnType.BaseType == typeof(ValueTask))
            && _controllerMethod.ReturnType.GenericTypeArguments.Length == 1)
        {
            _ = await (dynamic) result;
        }

        else if (_controllerMethod.ReturnType == typeof(Task) || _controllerMethod.ReturnType == typeof(ValueTask))
        {
            await (dynamic) result;
        }

        return new RouteExecutionResult(true);
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
            else if (TryGetAttribute<FromPathAttribute>(methodParameter, out var pathAttribute))
            {
                var parameterName = pathAttribute.PropertyName ?? methodParameter.Name!;
                parameterValue = requestParameters.GetPathParameter(parameterName, methodParameter.ParameterType)
                    ?? throw new BindException(parameterName);
            }
            else if (TryGetAttribute<FromQueryAttribute>(methodParameter, out var queryAttribute))
            {
                if (methodParameter.ParameterType.IsValueType || methodParameter.ParameterType == typeof(string))
                {
                    var parameterName = queryAttribute.PropertyName ?? methodParameter.Name!;
                    parameterValue = requestParameters.GetQueryParameter(parameterName, methodParameter.ParameterType);
                    if (queryAttribute.BindRequired && parameterValue is null)
                    {
                        throw new BindException(parameterName);
                    }
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

    private static bool TryGetAttribute<TAttribute>(
        ParameterInfo parameterInfo,
        [NotNullWhen(true)] out TAttribute? attribute)
        where TAttribute : Attribute
    {
        attribute = parameterInfo.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

    public override string ToString()
    {
        return _controllerMethod.Name;
    }
}
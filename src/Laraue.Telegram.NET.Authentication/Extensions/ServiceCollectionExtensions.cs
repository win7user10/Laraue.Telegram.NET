using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Models;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.Authentication.Extensions;

/// <summary>
/// Extensions to add Authentication functionality to the container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add authentication middleware to the container and configure identity.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static IdentityBuilder AddTelegramAuthentication<TUser, TKey>(
        this IServiceCollection serviceCollection)
        where TUser : TelegramIdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        serviceCollection.AddTelegramMiddleware<AuthTelegramMiddleware<TKey>>();
        
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });
        
        serviceCollection.AddScoped<TelegramRequestContext<TKey>>();
        serviceCollection.AddScoped<TelegramRequestContext>(
            sp => sp.GetRequiredService<TelegramRequestContext<TKey>>());
        
        serviceCollection.AddScoped<IUserService<TKey>, UserService<TUser, TKey>>();
        
        return serviceCollection.AddIdentity<TUser, IdentityRole>(opt =>
        {
            opt.Password.RequireDigit = false;
            opt.Password.RequiredLength = 1;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredUniqueChars = 1;
        });
    }
}
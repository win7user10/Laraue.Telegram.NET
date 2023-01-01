using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Models;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.Authentication.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add authentication middleware to the container and configure identity.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="TUser"></typeparam>
    /// <returns></returns>
    public static IdentityBuilder AddTelegramAuthentication<TUser>(
        this IServiceCollection serviceCollection)
        where TUser : TelegramIdentityUser, new()
    {
        serviceCollection.AddTelegramMiddleware<AuthTelegramMiddleware>();
        
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        serviceCollection.AddScoped<IUserService, UserService<TUser>>();
        
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
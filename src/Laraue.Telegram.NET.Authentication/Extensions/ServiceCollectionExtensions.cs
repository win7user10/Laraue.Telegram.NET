using Laraue.Telegram.NET.Abstractions;
using Laraue.Telegram.NET.Authentication.Middleware;
using Laraue.Telegram.NET.Authentication.Models;
using Laraue.Telegram.NET.Authentication.Protectors;
using Laraue.Telegram.NET.Authentication.Services;
using Laraue.Telegram.NET.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.Authentication.Extensions;

/// <summary>
/// Extensions to add Authentication functionality to the container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <param name="serviceCollection"></param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Add authentication middleware with <see cref="TelegramRequestContext{TKey}"/>
        /// to the container and configure identity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TTelegramUserQueryService"></typeparam>
        /// <returns></returns>
        public IServiceCollection AddTelegramAuthentication<TUser, TKey, TTelegramUserQueryService>()
            where TUser : class, ITelegramUser<TKey>, new()
            where TKey : IEquatable<TKey>
            where TTelegramUserQueryService : class, ITelegramUserQueryService<TUser, TKey> 
        {
            return serviceCollection.AddTelegramAuthentication<TUser, TKey, TTelegramUserQueryService, TelegramRequestContext<TKey>>();
        }

        /// <summary>
        /// Add authentication middleware and the passed <see cref="TTelegramRequestContext"/>
        /// to the container and configure identity.
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TTelegramUserQueryService"></typeparam>
        /// <typeparam name="TTelegramRequestContext"></typeparam>
        /// <returns></returns>
        public IServiceCollection AddTelegramAuthentication<TUser, TKey, TTelegramUserQueryService, TTelegramRequestContext>()
            where TUser : class, ITelegramUser<TKey>, new()
            where TKey : IEquatable<TKey>
            where TTelegramRequestContext : TelegramRequestContext<TKey>
            where TTelegramUserQueryService : class, ITelegramUserQueryService<TUser, TKey>
        {
            serviceCollection.AddTelegramMiddleware<AuthTelegramMiddleware<TKey>>();
        
            serviceCollection.AddScoped<TTelegramRequestContext>();
            serviceCollection.AddSingleton<IUserSemaphore, UserSemaphore>();

            if (typeof(TTelegramRequestContext) != typeof(TelegramRequestContext<TKey>))
            {
                serviceCollection.AddScoped<TelegramRequestContext<TKey>>(
                    sp => sp.GetRequiredService<TTelegramRequestContext>());
            }
        
            serviceCollection.AddScoped<TelegramRequestContext>(
                sp => sp.GetRequiredService<TTelegramRequestContext>());
        
            serviceCollection.AddScoped<ITelegramUserQueryService<TUser, TKey>, TTelegramUserQueryService>();
            serviceCollection.AddScoped<IUserService<TKey>, UserService<TUser, TKey>>();

            serviceCollection.UseUserRolesProvider<DefaultUserRoleProvider>();
            return serviceCollection.AddScoped<IControllerProtector, UserShouldBeInGroupProtector<TKey>>();
        }

        /// <summary>
        /// Start use role provider for the user.
        /// </summary>
        public IServiceCollection UseUserRolesProvider<TUserGroupProvider>()
            where TUserGroupProvider : class, IUserRoleProvider
        {
            return serviceCollection.AddScoped<IUserRoleProvider, TUserGroupProvider>();
        }
    }
}
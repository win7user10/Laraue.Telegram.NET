using Laraue.Telegram.NET.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Laraue.Telegram.NET.UpdatesQueue.EFCore.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Store taken telegram requests to queue using EF Core. 
        /// </summary>
        /// <returns></returns>
        public IServiceCollection AddEfCoreUpdatesQueue()
        {
            return serviceCollection
                .AddScoped<IUpdatesQueue, EfCoreUpdatesQueue>();
        }
    }
}
using System.Text.Json;
using Laraue.Telegram.NET.Core.Services;
using Microsoft.AspNetCore.Http;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Middleware;

internal sealed class MapRequestToTelegramCoreMiddleware(IUpdatesQueue updatesQueue) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var update = await JsonSerializer.DeserializeAsync<Update>(
            context.Request.Body,
            JsonBotAPI.Options);
        
        if (update is null)
        {
            return;
        }

        await updatesQueue
            .AddAsync([update], context.RequestAborted)
            .ConfigureAwait(false);
    }
}
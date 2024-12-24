using Laraue.Telegram.NET.Abstractions;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Core.Routing;

/// <summary>
/// Map the request taken from long pooling to the telegram middlewares.
/// </summary>
public interface ILongPoolingRequestsProcessor
{
    /// <summary>
    /// Process the request taken via long pooling.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask ProcessAsync(HttpResponseMessage request, CancellationToken cancellationToken);
}

/// <inheritdoc />
public class LongPoolingRequestsProcessor : ILongPoolingRequestsProcessor
{
    private readonly ITelegramRouter _telegramRouter;

    /// <summary>
    /// Initializes a new instance of <see cref="ITelegramRouter"/>
    /// </summary>
    public LongPoolingRequestsProcessor(ITelegramRouter telegramRouter)
    {
        _telegramRouter = telegramRouter;
    }

    /// <inheritdoc />
    public async ValueTask ProcessAsync(HttpResponseMessage request, CancellationToken cancellationToken)
    {
        var body = await request.Content.ReadAsStringAsync(cancellationToken);
        var update = JsonConvert.DeserializeObject<Update>(body);
        if (update is null)
        {
            return;
        }

        await _telegramRouter.RouteAsync(update, cancellationToken);
    }
}
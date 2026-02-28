using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Testing;

public class TelegramBotClientMock : ITelegramBotClient
{
    public async Task<TResponse> SendRequest<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (OnMakingApiRequest is not null)
            await OnMakingApiRequest.Invoke(this, new ApiRequestEventArgs(request), cancellationToken);

        return default;
    }

    public Task<bool> TestApi(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DownloadFile(TGFile file, Stream destination, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public bool LocalBotServer => false;
    public long BotId => 1;
    public TimeSpan Timeout { get; set; }
    public IExceptionParser ExceptionsParser { get; set; }
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
}
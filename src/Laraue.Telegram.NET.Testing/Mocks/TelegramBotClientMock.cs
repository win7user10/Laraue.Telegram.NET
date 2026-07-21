using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Laraue.Telegram.NET.Testing.Mocks;

public class TelegramBotClientMock : ITelegramBotClient
{
    private readonly ITelegramBotClient? _innerMock;

    public TelegramBotClientMock()
    {}
    
    
    public TelegramBotClientMock(ITelegramBotClient innerMock)
    {
        _innerMock = innerMock;
    }
    
    public async Task<TResponse> SendRequest<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (OnMakingApiRequest is not null)
            await OnMakingApiRequest.Invoke(this, new ApiRequestEventArgs(request), cancellationToken);

        if (_innerMock is null)
            return default;

        return await _innerMock.SendRequest(request, cancellationToken);
    }

    public Task<bool> TestApi(CancellationToken cancellationToken = default)
    {
        return _innerMock is null
            ? Task.FromResult(true)
            : _innerMock.TestApi(cancellationToken);
    }

    public Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = default)
    {
        return _innerMock is null
            ? Task.CompletedTask
            : _innerMock.DownloadFile(filePath, destination, cancellationToken);
    }

    public Task DownloadFile(TGFile file, Stream destination, CancellationToken cancellationToken = default)
    {
        return _innerMock is null
            ? Task.CompletedTask
            : _innerMock.DownloadFile(file, destination, cancellationToken);
    }

    public bool LocalBotServer => _innerMock?.LocalBotServer ?? false;
    public long BotId => _innerMock?.BotId ?? 1;
    public TimeSpan Timeout { get; set; }
    public IExceptionParser ExceptionsParser { get; set; }
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
}
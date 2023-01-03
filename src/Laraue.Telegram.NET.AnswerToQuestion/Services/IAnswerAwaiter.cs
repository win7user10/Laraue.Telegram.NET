namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

public interface IAnswerAwaiter
{
    /// <summary>
    /// Try execute response awaiter if it is suitable for the execution.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    Task<object?> ExecuteAsync(IServiceProvider serviceProvider);
}
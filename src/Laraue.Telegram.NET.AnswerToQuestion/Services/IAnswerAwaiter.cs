namespace Laraue.Telegram.NET.AnswerToQuestion.Services;

/// <summary>
/// The class that will be executed in the request pipeline for the user instead if standard request pipeline. 
/// </summary>
public interface IAnswerAwaiter
{
    /// <summary>
    /// Unique awaiter identifier. Each awaiter Type should have its own unique identifier. 
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Try execute response awaiter if it is suitable for the execution.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    Task<object?> ExecuteAsync(IServiceProvider serviceProvider);
}
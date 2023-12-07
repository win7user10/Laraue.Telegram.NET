using Laraue.Core.Exceptions.Web;
using Laraue.Telegram.NET.Abstractions.Exceptions;
using Laraue.Telegram.NET.Authentication.Models;
using Laraue.Telegram.NET.Authentication.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Laraue.Telegram.NET.Authentication.Services;

public class UserService<TUser, TKey> : IUserService<TKey>
    where TUser : TelegramIdentityUser<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly UserManager<TUser> _userManager;
    private readonly IdentityOptions _identityOptions;

    public UserService(
        UserManager<TUser> userManager,
        IOptions<IdentityOptions> identityOptions)
    {
        _userManager = userManager;
        _identityOptions = identityOptions.Value;
    }
    
    /// <inheritdoc />
    public async Task<LoginResponse<TKey>> LoginOrRegisterAsync(TelegramData telegramData)
    {
        var userName = $"tg_{telegramData.Username}";
        var user = await _userManager.FindByNameAsync(userName);

        if (user is not null)
        {
            return new LoginResponse<TKey>(user.Id);
        }

        var password = RegistrationHelper.GenerateRandomPassword(
            _identityOptions.Password);
        
        var result = await CreateUserInternalAsync(
            userName,
            telegramData.Id,
            password);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.Errors.ToString());
        }
        
        user = await _userManager.FindByNameAsync(userName);
        
        return new LoginResponse<TKey>(user!.Id);
    }

    private Task<IdentityResult> CreateUserInternalAsync(string userName, long? telegramId, string password)
    {
        return _userManager.CreateAsync(
            new TUser
            {
                UserName = userName,
                TelegramId = telegramId,
                CreatedAt = DateTime.UtcNow,
            }, password);
    }
}
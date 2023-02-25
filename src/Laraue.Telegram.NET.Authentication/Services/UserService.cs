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

    public async Task<LoginResponse<TKey>> LoginAsync(LoginData loginData)
    {
        var(login, password) = loginData;
        var user = await _userManager.FindByNameAsync(login);
        
        if (user is null)
        {
            throw new UnauthorizedRequestException(new Dictionary<string, string[]>
            {
                [nameof(loginData.Username)] = new []{"User is missing"},
            });
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            throw new UnauthorizedRequestException(new Dictionary<string, string[]>
            {
                [nameof(loginData.Password)] = new []
                {
                    "The password is not correct"
                },
            });
        }

        return new LoginResponse<TKey>(user.Id);
    }

    public async Task<LoginResponse<TKey>> RegisterAsync(LoginData loginData)
    {
        var (login, password) = loginData;
        var result = await _userManager.CreateAsync(
            new TUser { UserName = login,},
            password);
        
        if (result.Succeeded)
        {
            return await LoginAsync(loginData);
        }
        
        var errorStringsMap = result.Errors
            .Select(x => new
            {
                x.Description,
                Field = RegistrationHelper.MappingIdentityErrorDescriber[x.Code]
            })
            .GroupBy(arg => arg.Field, arg => arg.Description)
            .ToDictionary(x => x.Key, x => x.ToArray());
            
        throw new BadRequestException(errorStringsMap);

    }

    public async Task<LoginResponse<TKey>> LoginOrRegisterAsync(TelegramData telegramData)
    {
        var userName = $"tg_{telegramData.Username}";
        var user = await _userManager.FindByNameAsync(userName);

        if (user is not null)
        {
            return new LoginResponse<TKey>(user.Id);
        }

        var password = RegistrationHelper.GenerateRandomPassword(_identityOptions.Password);
        var result = await _userManager.CreateAsync(
            new TUser
            {
                UserName = userName,
                TelegramId = telegramData.Id
            }, password);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.Errors.ToString());
        }
        
        user = await _userManager.FindByNameAsync(userName);
        
        return new LoginResponse<TKey>(user.Id);
    }
}
using Laraue.Telegram.NET.Authentication.Services;
using Telegram.Bot.Types;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Authentication;

public class StaticUserGroupProviderTests
{
    [Fact]
    public async Task Provider_ShouldReturnGroups_ForTheUser()
    {
        var provider = new StaticUserGroupProvider(new GroupUsers
        {
            ["Root"] = new []{ "NickAbrams", "JackSheppard" },
            ["Telemetry.View"] = new []{ "NickAbrams" }
        });

        var groups = await provider.GetUserGroupsAsync(new User { Username = "NickAbrams" });
        Assert.Equal(new [] { "Root", "Telemetry.View" }, groups.Select(x => x.Name));
    }
}
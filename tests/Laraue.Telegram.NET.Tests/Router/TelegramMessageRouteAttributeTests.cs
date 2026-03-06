using Laraue.Telegram.NET.Core.Routing.Attributes;
using Telegram.Bot.Types;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Router;

public class TelegramMessageRouteAttributeTests
{
    [Fact]
    public void TelegramMessageRoute_ShouldMatch_GreedyStringsAsParameter()
    {
        var attribute = new TelegramMessageRouteAttribute("/category add {name}*");
        
        Assert.True(attribute.TryMatch(new Update
        {
            Message = new Message
            {
                Text = "/category add Alex John"
            }
        }, out var parameters));
        
        Assert.Equal("Alex John", parameters.GetPathParameter("name", typeof(string)));
    }
}
using Laraue.Telegram.NET.Core.Routing;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Router;

public class RouteRegexCreatorTests
{
    [Theory]
    [InlineData("groups/*", "groups/12")]
    [InlineData("groups/*", "groups/abc/asd")]
    [InlineData("groups/*", "groups/")]
    [InlineData("/start", "/start")]
    public void SuitableRoute_ShouldBeMatched(string pathPattern, string route)
    {
        var regex = RouteRegexCreator.ForRoute(pathPattern);
        
        Assert.Matches(regex, route);
    }
    
    [Theory]
    [InlineData("groups/*", "group/")]
    [InlineData("/start", "/start1")]
    [InlineData("/start", "/star")]
    public void SuitableRoute_ShouldNotBeMatched(string pathPattern, string route)
    {
        var regex = RouteRegexCreator.ForRoute(pathPattern);
        
        Assert.DoesNotMatch(regex, route);
    }
}
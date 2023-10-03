using Laraue.Telegram.NET.Core.Routing;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Router;

public class RouteRegexCreatorTests
{
    [Theory]
    [InlineData("groups/{id}", "groups/12")]
    [InlineData("groups/{id}", "groups/12?minParticipantCount")]
    [InlineData("groups/{group1}/{group2}", "groups/abc/asd")]
    [InlineData("groups/{group1}/{group2}", "groups/1/2")]
    [InlineData("/start", "/start")]
    public void SuitableRoute_ShouldBeMatched(string pathPattern, string route)
    {
        var regex = RouteRegexCreator.ForRoute(pathPattern);
        
        Assert.Matches(regex, route);
    }
    
    [Theory]
    [InlineData("groups/{id}", "groups/")]
    [InlineData("groups/", "group/")]
    [InlineData("/start", "/start1")]
    [InlineData("/start", "/star")]
    public void SuitableRoute_ShouldNotBeMatched(string pathPattern, string route)
    {
        var regex = RouteRegexCreator.ForRoute(pathPattern);
        
        Assert.DoesNotMatch(regex, route);
    }
}
using Laraue.Telegram.NET.DataAccess;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Router;

public class RoutePathBuilderTests
{
    [Fact]
    public void IntParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new RoutePathBuilder("/route");
        pathBuilder.WithQueryParameter("p", 1);
        
        Assert.Equal("/route?p=1", pathBuilder.ToString());
    }
    
    [Fact]
    public void DateParameter_ShouldBeAddedCorrectly()
    {
        var dateTime = DateTime.UtcNow;
        
        var pathBuilder = new RoutePathBuilder("/route");
        pathBuilder.WithQueryParameter("from", dateTime);
        
        Assert.Matches($"\\/route\\?from\\={dateTime:yyyy-MM-ddTHH:mm:ss}(.*)Z", pathBuilder.ToString());
    }
    
    [Fact]
    public void StringParameter_ShouldBeAddedCorrectly()
    {
        var dateTime = DateTime.UtcNow;
        
        var pathBuilder = new RoutePathBuilder("/route");
        pathBuilder.WithQueryParameter("from", dateTime.ToString("yyyy-MM-dd"));
        
        Assert.Equal($"/route?from={dateTime:yyyy-MM-dd}", pathBuilder.ToString());
    }
}
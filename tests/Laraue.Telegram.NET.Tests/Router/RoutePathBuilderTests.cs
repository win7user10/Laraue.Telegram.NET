using Laraue.Telegram.NET.Core.Routing;
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
        
        Assert.Matches($"\\/route\\?from\\=\"{dateTime:yyyy-MM-ddTHH:mm:ss}(.*)Z\"", pathBuilder.ToString());
    }
    
    [Fact]
    public void StringParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new RoutePathBuilder("/route");
        pathBuilder.WithQueryParameter("from", "Alex");
        
        Assert.Equal($"/route?from=\"Alex\"", pathBuilder.ToString());
    }
    
    [Fact]
    public void BoolParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new RoutePathBuilder("/route");
        pathBuilder.WithQueryParameter("v", true);
        
        Assert.Equal("/route?v=true", pathBuilder.ToString());
    }
    
    [Fact]
    public void NullableLongParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new RoutePathBuilder("/route");
        pathBuilder.WithQueryParameter("p", new long?(12));
        
        Assert.Equal("/route?p=12", pathBuilder.ToString());
    }
}
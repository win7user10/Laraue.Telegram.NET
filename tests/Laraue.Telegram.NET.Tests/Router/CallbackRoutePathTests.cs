using Laraue.Telegram.NET.Core.Routing;
using Laraue.Telegram.NET.DataAccess;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Router;

public class CallbackRoutePathTests
{
    [Fact]
    public void IntParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new CallbackRoutePath("/route");
        pathBuilder.WithQueryParameter("p", 1);
        
        Assert.Equal("/route?p=1", pathBuilder.ToString());
    }
    
    [Fact]
    public void DateParameter_ShouldBeAddedCorrectly()
    {
        var dateTime = DateTime.UtcNow;
        
        var pathBuilder = new CallbackRoutePath("/route");
        pathBuilder.WithQueryParameter("from", dateTime);
        
        Assert.Matches($"\\/route\\?from\\=\"{dateTime:yyyy-MM-ddTHH:mm:ss}(.*)Z\"", pathBuilder.ToString());
    }
    
    [Fact]
    public void StringParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new CallbackRoutePath("/route");
        pathBuilder.WithQueryParameter("from", "Alex");
        
        Assert.Equal($"/route?from=\"Alex\"", pathBuilder.ToString());
    }
    
    [Fact]
    public void BoolParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new CallbackRoutePath("/route");
        pathBuilder.WithQueryParameter("v", true);
        
        Assert.Equal("/route?v=true", pathBuilder.ToString());
    }
    
    [Fact]
    public void NullableLongParameter_ShouldBeAddedCorrectly()
    {
        var pathBuilder = new CallbackRoutePath("/route");
        pathBuilder.WithQueryParameter("p", new long?(12));
        
        Assert.Equal("/route?p=12", pathBuilder.ToString());
    }
    
    [Fact]
    public void FreezeBuilder_ShouldPreventBuilderFromModification()
    {
        var pathBuilder = new CallbackRoutePath("/route");
        pathBuilder.WithQueryParameter("p", 10)
            .Freeze();

        var builder2 = pathBuilder.WithQueryParameter("x", 12);
        
        Assert.Equal("/route?p=10", pathBuilder.ToString());
        Assert.Equal("/route?p=10&x=12", builder2.ToString());
    }
}
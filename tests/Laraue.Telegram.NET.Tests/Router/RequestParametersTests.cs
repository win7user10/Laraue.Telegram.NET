using Laraue.Telegram.NET.Abstractions.Request;
using Xunit;

namespace Laraue.Telegram.NET.Tests.Router;

public class RequestParametersTests
{
    [Fact]
    public void IntParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<int>("1");
        
        Assert.Equal(1, value);
    }
    
    [Fact]
    public void StringParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<string>("Alex");
        
        Assert.Equal("Alex", value);
    }
    
    [Fact]
    public void DateParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<DateTime>("2022-01-01T15:00:00");
        
        Assert.Equal(new DateTime(2022, 01, 01, 15, 0, 0), value);
    }

    private static T? TestParameter<T>(string stringValue)
    {
        var parameters = new RequestParameters(
            pathParameters: new Dictionary<string, string?>(),
            queryParameters: new Dictionary<string, string?> { ["p"] = stringValue });

        return (T?)parameters.GetQueryParameter("p", typeof(T));
    }
}
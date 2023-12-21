using Laraue.Telegram.NET.Abstractions.Request;
using Laraue.Telegram.NET.Core.Routing;
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
    public void NullableIntParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<int?>("1");
        
        Assert.Equal(1, value);
    }
    
    [Fact]
    public void StringParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<string>("\"Alex\"");
        
        Assert.Equal("Alex", value);
    }
    
    [Fact]
    public void BoolParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<bool>("true");
        
        Assert.True(value);
    }
    
    [Fact]
    public void DateParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<DateTime>("\"2022-01-01T15:00:00\"");
        
        Assert.Equal(new DateTime(2022, 01, 01, 15, 0, 0), value);
    }
    
    [Fact]
    public void NullableEnumParameter_ShouldBeParsedCorrectly()
    {
        var value = TestParameter<TestEnum?>("1");
        
        Assert.Equal(TestEnum.Value1, value);
    }
    
    [Fact]
    public void ClassParameter_ShouldBeParsedCorrectly()
    {
        var parameters = new RequestParameters(
            pathParameters: new Dictionary<string, string?>(),
            queryParameters: new Dictionary<string, string?>
            {
                ["enumValue"] = "1",
                ["s"] = "\"Alex\""
            });
        
        var result = (TestClass)parameters.GetQueryParameters(typeof(TestClass));
        
        Assert.Equal("Alex", result.StringValue);
        Assert.Equal(TestEnum.Value1, result.EnumValue);
    }

    private static T? TestParameter<T>(string stringValue)
    {
        var parameters = new RequestParameters(
            pathParameters: new Dictionary<string, string?>(),
            queryParameters: new Dictionary<string, string?> { ["p"] = stringValue });

        return (T?)parameters.GetQueryParameter("p", typeof(T));
    }

    private enum TestEnum
    {
        Value1 = 1
    }

    private class TestClass
    {
        public TestEnum EnumValue { get; set; }
        
        [FromQuery("s")]
        public string? StringValue { get; set; }
    }
}
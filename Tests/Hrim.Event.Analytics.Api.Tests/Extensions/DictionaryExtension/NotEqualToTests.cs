using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Extensions;

namespace Hrim.Event.Analytics.Api.Tests.Extensions.DictionaryExtension;

public class NotNotEqualToTests
{
    [Fact]
    public void Given_Two_Nulls_Should_Return_True() {
        IDictionary<string, string>? left  = null;
        IDictionary<string, string>? right = null;
        left.NotEqualTo(right).Should().BeFalse();
    }

    [Fact]
    public void Given_Left_Null_Right_NotNull_But_Empty_Should_Return_True() {
        IDictionary<string, string>? left  = null;
        IDictionary<string, string> right = new Dictionary<string, string>();
        left.NotEqualTo(right).Should().BeFalse();
    }

    [Fact]
    public void Given_Right_Null_Left_NotNull_But_Empty_Should_Return_True() {
        IDictionary<string, string> left  = new Dictionary<string, string>();
        IDictionary<string, string>? right = null;
        left.NotEqualTo(right).Should().BeFalse();
    }

    [Fact]
    public void Given_Right_Null_Left_NotEmpty_Should_Return_False() {
        IDictionary<string, string> left  = new Dictionary<string, string>() { { "k", "v" } };
        IDictionary<string, string>? right = null;
        left.NotEqualTo(right).Should().BeTrue();
    }

    [Fact]
    public void Given_Left_Null_Right_NotEmpty_Should_Return_False() {
        IDictionary<string, string>? left = null;
        IDictionary<string, string> right = new Dictionary<string, string>() { { "k", "v" } };
        left.NotEqualTo(right).Should().BeTrue();
    }

    [Fact]
    public void Given_Both_NotEmpty_But_Different_Keys_Should_Return_False() {
        IDictionary<string, string> left  = new Dictionary<string, string>() { { "k1", "v" } };
        IDictionary<string, string> right = new Dictionary<string, string>() { { "k2", "v" } };
        left.NotEqualTo(right).Should().BeTrue();
    }

    [Fact]
    public void Given_Both_NotEmpty_But_Different_Values_Should_Return_False() {
        IDictionary<string, string> left  = new Dictionary<string, string>() { { "k", "v1" } };
        IDictionary<string, string> right = new Dictionary<string, string>() { { "k", "v2" } };
        left.NotEqualTo(right).Should().BeTrue();
    }

    [Fact]
    public void Given_Both_Same_Keys_Same_Values_Should_Return_True() {
        IDictionary<string, string> left = new Dictionary<string, string>() { { "k", "v" } };
        IDictionary<string, string> right = new Dictionary<string, string>() { { "k", "v" } };
        left.NotEqualTo(right).Should().BeFalse();
    }
}
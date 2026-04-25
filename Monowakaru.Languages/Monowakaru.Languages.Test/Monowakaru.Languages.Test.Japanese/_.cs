using System.Reflection;
using Monowakaru.Languages.Grammar;

// ReSharper disable UseNameofExpression - not possible, the members are defined by derived types

namespace Monowakaru.Languages.Test.Japanese;

#pragma warning disable S101
/// <summary>
///     We use this abstract base class for all rule table tests to reduce code duplication. Derived classes merely need to
///     implement the 6 static test case data fields
///     and the tests will automagically work.
/// </summary>
/// <typeparam name="TTestee">The concrete type of the rule table.</typeparam>
public abstract class _<TTestee> where TTestee : IRuleSet, new()
#pragma warning restore S101
{
    private TTestee _testee;

    [SetUp]
    public void Setup()
    {
        _testee = new TTestee();
    }

    private void EnsureStaticMember(string member)
    {
        var field = GetType().GetField(member, BindingFlags.Static | BindingFlags.Public);
        Assume.That(field is not null, "Expected static member is not implemented on derived test class");
        Assume.That(field.FieldType == typeof(IEnumerable<TestCaseData>), $"Expected return type of Entities() does not match expected IEnumerable<TestCaseData>");
    }

    private void CheckInflectInternal(string input, string expected)
    {
        var results = _testee.Inflect(input);
        Assert.That(results, Has.Some.Matches<InflectionResult>(x => x.Result.Equals(expected, StringComparison.OrdinalIgnoreCase)));
    }

    private void CheckDeinflectInternal(string input, string expected)
    {
        var results = _testee.Deinflect(input);
        Assert.That(results, Has.Some.Matches<DeinflectionResult>(x => x.Result.Equals(expected, StringComparison.OrdinalIgnoreCase)));
    }

#pragma warning disable NUnit1011

    [Test]
    [TestCaseSource("SpecialFormsTestData")]
    [Category("Inflection")]
    [Category("Special Forms")]
    public void InflectSpecialFormsCorrectly(string input, string expected)
    {
        EnsureStaticMember("SpecialFormsTestData");
        CheckInflectInternal(input, expected);
    }

    [Test]
    [TestCaseSource("GodanTestData")]
    [Category("Inflection")]
    [Category("Godan")]
    public void InflectGodanCorrectly(string input, string expected)
    {
        EnsureStaticMember("GodanTestData");
        CheckInflectInternal(input, expected);
    }

    [Test]
    [TestCaseSource("IchidanTestData")]
    [Category("Inflection")]
    [Category("Ichidan")]
    public void InflectIchidanCorrectly(string input, string expected)
    {
        EnsureStaticMember("IchidanTestData");
        CheckInflectInternal(input, expected);
    }

    [Test]
    [TestCaseSource("SpecialFormsDeinflectionTestData")]
    [Category("Deinflection")]
    [Category("Special Forms")]
    public void DeinflectSpecialFormsCorrectly(string input, string expected)
    {
        EnsureStaticMember("SpecialFormsDeinflectionTestData");
        CheckDeinflectInternal(input, expected);
    }

    [Test]
    [TestCaseSource("GodanDeinflectionTestData")]
    [Category("Deinflection")]
    [Category("Godan")]
    public void DeinflectGodanCorrectly(string input, string expected)
    {
        EnsureStaticMember("GodanDeinflectionTestData");
        CheckDeinflectInternal(input, expected);
    }

    [Test]
    [TestCaseSource("IchidanDeinflectionTestData")]
    [Category("Deinflection")]
    [Category("Ichidan")]
    public void DeinflectIchidanCorrectly(string input, string expected)
    {
        EnsureStaticMember("IchidanDeinflectionTestData");
        CheckDeinflectInternal(input, expected);
    }
#pragma warning restore NUnit1011
}

public static class TestCaseDataExtensions
{
    public static IEnumerable<TestCaseData> ReverseArguments(this IEnumerable<TestCaseData> testCases)
    {
        return testCases.Select(t => new TestCaseData(t.Arguments.Reverse().ToArray()));
    }
}
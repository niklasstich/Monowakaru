using Monowakaru.Languages.Japanese.Grammar;

namespace Monowakaru.Languages.Test.Japanese;

[TestFixture]
[TestOf(typeof(ShushikeiTeFormRulesShould))]
public class ShushikeiTeFormRulesShould : _<ShushikeiTeFormRules>
{
    public static readonly IEnumerable<TestCaseData> SpecialFormsTestData =
    [
        new("行く", "行って")
    ];

    public static readonly IEnumerable<TestCaseData> GodanTestData =
    [
        new("飲む", "飲んで")
    ];

    public static readonly IEnumerable<TestCaseData> IchidanTestData =
    [
        new("食べる", "食べて")
    ];

    public static readonly IEnumerable<TestCaseData> SpecialFormsDeinflectionTestData = SpecialFormsTestData.ReverseArguments();
    public static readonly IEnumerable<TestCaseData> GodanDeinflectionTestData = GodanTestData.ReverseArguments();
    public static readonly IEnumerable<TestCaseData> IchidanDeinflectionTestData = IchidanTestData.ReverseArguments();
}
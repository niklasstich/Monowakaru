using Monowakaru.Languages.Japanese.Grammar;

namespace Monowakaru.Languages.Test.Japanese;

[TestFixture]
[TestOf(typeof(ShushikeiTeFormRulesShould))]
public class ShushikeiTeFormRulesShould : _<ShushikeiTeFormRules>
{
    /// <summary>
    ///     Irregular verbs and exceptions: <c>来る</c>/<c>する</c> (plus a compound suru) and the
    ///     <c>行く → 行って</c> sokuon exception that can't be derived from the godan-k rule.
    /// </summary>
    public static readonly IEnumerable<TestCaseData> SpecialFormsTestData =
    [
        new("行く", "行って"),
        new("くる", "きて"),
        new("する", "して"),
        new("勉強する", "勉強して"),
        new("運動する", "運動して")
    ];

    /// <summary>
    ///     One sample per 五段 row (カ ガ サ タ ナ バ マ ラ ワ), with extra cases for the more common
    ///     rows so onbin variations (い音便 / 促音便 / 撥音便) are each exercised more than once.
    /// </summary>
    public static readonly IEnumerable<TestCaseData> GodanTestData =
    [
        // カ行 — く → いて (い音便)
        new("書く", "書いて"),
        new("焼く", "焼いて"),
        new("開く", "開いて"),
        // ガ行 — ぐ → いで (い音便, voiced)
        new("泳ぐ", "泳いで"),
        new("急ぐ", "急いで"),
        // サ行 — す → して (no onbin)
        new("話す", "話して"),
        new("押す", "押して"),
        new("出す", "出して"),
        // タ行 — つ → って (促音便)
        new("待つ", "待って"),
        new("持つ", "持って"),
        new("立つ", "立って"),
        // ナ行 — ぬ → んで (撥音便); 死ぬ is the only surviving modern verb
        new("死ぬ", "死んで"),
        // バ行 — ぶ → んで (撥音便)
        new("飛ぶ", "飛んで"),
        new("遊ぶ", "遊んで"),
        new("呼ぶ", "呼んで"),
        // マ行 — む → んで (撥音便)
        new("読む", "読んで"),
        new("飲む", "飲んで"),
        new("住む", "住んで"),
        new("噛む", "噛んで"),
        // ラ行 — る → って (促音便). Distinguished from ichidan by the る-rule's accepts="v5".
        new("走る", "走って"),
        new("帰る", "帰って"),
        new("降る", "降って"),
        new("取る", "取って"),
        // ワ行 — う → って (促音便)
        new("買う", "買って"),
        new("会う", "会って"),
        new("使う", "使って"),
        new("言う", "言って")
    ];

    /// <summary>
    ///     Ichidan: る → て. Mix of 上一段 (-iる) and 下一段 (-eる) — both conjugate identically.
    /// </summary>
    public static readonly IEnumerable<TestCaseData> IchidanTestData =
    [
        // 上一段 (i-stem)
        new("見る", "見て"),
        new("着る", "着て"),
        new("起きる", "起きて"),
        new("落ちる", "落ちて"),
        new("信じる", "信じて"),
        // 下一段 (e-stem)
        new("食べる", "食べて"),
        new("出る", "出て"),
        new("入れる", "入れて"),
        new("寝る", "寝て"),
        new("教える", "教えて"),
        new("逃げる", "逃げて")
    ];

    public static readonly IEnumerable<TestCaseData> SpecialFormsDeinflectionTestData = SpecialFormsTestData.ReverseArguments();
    public static readonly IEnumerable<TestCaseData> GodanDeinflectionTestData = GodanTestData.ReverseArguments();
    public static readonly IEnumerable<TestCaseData> IchidanDeinflectionTestData = IchidanTestData.ReverseArguments();
}
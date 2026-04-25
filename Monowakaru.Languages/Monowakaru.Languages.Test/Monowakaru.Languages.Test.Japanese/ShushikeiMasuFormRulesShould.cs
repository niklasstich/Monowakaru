using Monowakaru.Languages.Japanese.Grammar;

namespace Monowakaru.Languages.Test.Japanese;

[TestFixture]
[TestOf(typeof(ShushikeiMasuFormRulesShould))]
public class ShushikeiMasuFormRulesShould : _<ShushikeiMasuFormRules>
{
    /// <summary>
    ///     Irregular verbs: 来る / する and a couple of compound suru-verbs.
    ///     -ます has no exceptions analogous to -て's 行って — 行く inflects regularly (see godan).
    /// </summary>
    public static readonly IEnumerable<TestCaseData> SpecialFormsTestData =
    [
        new("くる", "きます"),
        new("する", "します"),
        new("勉強する", "勉強します"),
        new("運動する", "運動します")
    ];

    /// <summary>
    ///     One sample per 五段 row (カ ガ サ タ ナ バ マ ラ ワ); 行く is included to verify the
    ///     regular godan-k path applies (no exception in this table).
    /// </summary>
    public static readonly IEnumerable<TestCaseData> GodanTestData =
    [
        // カ行 — く → きます
        new("書く", "書きます"),
        new("焼く", "焼きます"),
        new("開く", "開きます"),
        new("行く", "行きます"),
        // ガ行 — ぐ → ぎます
        new("泳ぐ", "泳ぎます"),
        new("急ぐ", "急ぎます"),
        // サ行 — す → します
        new("話す", "話します"),
        new("押す", "押します"),
        new("出す", "出します"),
        // タ行 — つ → ちます
        new("待つ", "待ちます"),
        new("持つ", "持ちます"),
        new("立つ", "立ちます"),
        // ナ行 — ぬ → にます
        new("死ぬ", "死にます"),
        // バ行 — ぶ → びます
        new("飛ぶ", "飛びます"),
        new("遊ぶ", "遊びます"),
        new("呼ぶ", "呼びます"),
        // マ行 — む → みます
        new("読む", "読みます"),
        new("飲む", "飲みます"),
        new("住む", "住みます"),
        new("噛む", "噛みます"),
        // ラ行 — る → ります
        new("走る", "走ります"),
        new("帰る", "帰ります"),
        new("降る", "降ります"),
        new("取る", "取ります"),
        // ワ行 — う → います
        new("買う", "買います"),
        new("会う", "会います"),
        new("使う", "使います"),
        new("言う", "言います")
    ];

    /// <summary>
    ///     Ichidan: る → ます. Mix of 上一段 (-iる) and 下一段 (-eる) — both conjugate identically.
    /// </summary>
    public static readonly IEnumerable<TestCaseData> IchidanTestData =
    [
        // 上一段 (i-stem)
        new("見る", "見ます"),
        new("着る", "着ます"),
        new("起きる", "起きます"),
        new("落ちる", "落ちます"),
        new("信じる", "信じます"),
        // 下一段 (e-stem)
        new("食べる", "食べます"),
        new("出る", "出ます"),
        new("入れる", "入れます"),
        new("寝る", "寝ます"),
        new("教える", "教えます"),
        new("逃げる", "逃げます")
    ];

    public static readonly IEnumerable<TestCaseData> SpecialFormsDeinflectionTestData = SpecialFormsTestData.ReverseArguments();
    public static readonly IEnumerable<TestCaseData> GodanDeinflectionTestData = GodanTestData.ReverseArguments();
    public static readonly IEnumerable<TestCaseData> IchidanDeinflectionTestData = IchidanTestData.ReverseArguments();
}
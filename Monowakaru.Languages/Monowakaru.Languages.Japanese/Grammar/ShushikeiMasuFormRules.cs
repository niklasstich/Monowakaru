using Monowakaru.Languages.Generators.Attributes;

namespace Monowakaru.Languages.Japanese.Grammar;

/// <summary>
///     All suffix rules that transform a verb's 終止形 (shūshikei, dictionary form) into its
///     polite -ます form. Mechanically: the godan u-row final kana shifts to the i-row, then ます
///     is appended; ichidan drops る and appends ます; 来る / する have their own irregular forms.
/// </summary>
/// <remarks>
///     <para>
///         <b>Condition tags.</b> Table defaults are <c>"v5"</c> / <c>"v5-masu"</c>. Ichidan, kuru,
///         and suru override per-rule with their terminal class (<c>v1</c> / <c>vk</c> / <c>vs</c>)
///         and intermediate (<c>v1-masu</c> / <c>vk-masu</c> / <c>vs-masu</c>).
///     </para>
///     <para>
///         <b>No 行く exception.</b> Unlike the -て form, 行く inflects regularly to 行きます via
///         the godan-k rule. No special-case suffix needed.
///     </para>
///     <para>
///         <b>Ambiguity.</b> The る suffix is shared by godan ラ行 (る → ります) and ichidan
///         (る → ます); both candidates are emitted and disambiguated downstream by the
///         deinflection engine + dictionary class tag.
///     </para>
/// </remarks>
[InflectionTable(DefaultAccepts = "v5", DefaultProduces = "v5-masu")]
// Godan conjugation (one rule per 行) — all inherit table-level v5 / v5-masu
[SuffixRule("く", "きます", "JapaneseForms.MasuForm")]
[SuffixRule("ぐ", "ぎます", "JapaneseForms.MasuForm")]
[SuffixRule("す", "します", "JapaneseForms.MasuForm")]
[SuffixRule("つ", "ちます", "JapaneseForms.MasuForm")]
[SuffixRule("ぬ", "にます", "JapaneseForms.MasuForm")]
[SuffixRule("ぶ", "びます", "JapaneseForms.MasuForm")]
[SuffixRule("む", "みます", "JapaneseForms.MasuForm")]
[SuffixRule("る", "ります", "JapaneseForms.MasuForm")]
[SuffixRule("う", "います", "JapaneseForms.MasuForm")]
// Ichidan
[SuffixRule("る", "ます", "JapaneseForms.MasuForm", Accepts = "v1", Produces = "v1-masu")]
// Irregulars (hiragana; kanji forms 来る / (compound)する handled at the reading layer)
[SuffixRule("くる", "きます", "JapaneseForms.MasuForm", Accepts = "vk", Produces = "vk-masu")]
[SuffixRule("する", "します", "JapaneseForms.MasuForm", Accepts = "vs", Produces = "vs-masu")]
public sealed partial class ShushikeiMasuFormRules;
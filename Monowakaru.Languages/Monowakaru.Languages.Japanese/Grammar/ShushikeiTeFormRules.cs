using Monowakaru.Languages.Generators.Attributes;

namespace Monowakaru.Languages.Japanese.Grammar;

/// <summary>
///     All suffix rules that transform a verb's 終止形 (shūshikei, dictionary form) into its
///     -て form, covering every godan row, ichidan, and the two irregulars 来る / する.
/// </summary>
/// <remarks>
///     <para>
///         <b>Condition tags.</b> Table defaults are <c>"v5"</c> / <c>"v5-te"</c> — godan rows are by
///         far the most common case. Ichidan, kuru, and suru rules override per-rule with their
///         specific terminal class (<c>v1</c>/<c>vk</c>/<c>vs</c>) and intermediate
///         (<c>v1-te</c>/<c>vk-te</c>/<c>vs-te</c>). Tag IDs match Yomitan / JMdict POS strings so a
///         deinflection candidate's terminal condition compares directly against a dictionary entry's
///         class tag with no translation layer.
///     </para>
///     <para>
///         <b>Ambiguity is intentional.</b> The る suffix appears twice (godan ラ行 → って, ichidan → て),
///         and んで / って each cover three godan rows. Both candidates are emitted; downstream
///         disambiguation is done by the deinflection engine using the produces tag and the
///         dictionary lookup result.
///     </para>
///     <para>
///         <b>Known exception.</b> 行く → 行って (not 行いて) is modelled as a longer-suffix rule that
///         wins via the indexer's longest-suffix-first probe. The shorter く → いて rule still fires
///         on other godan-k verbs such as 書く / 焼く.
///     </para>
/// </remarks>
[InflectionTable(DefaultAccepts = "v5", DefaultProduces = "v5-te")]
// Godan conjugation (one rule per 行) — all inherit table-level v5 / v5-te
[SuffixRule("く", "いて", "JapaneseForms.TeForm")]
[SuffixRule("ぐ", "いで", "JapaneseForms.TeForm")]
[SuffixRule("す", "して", "JapaneseForms.TeForm")]
[SuffixRule("つ", "って", "JapaneseForms.TeForm")]
[SuffixRule("ぬ", "んで", "JapaneseForms.TeForm")]
[SuffixRule("ぶ", "んで", "JapaneseForms.TeForm")]
[SuffixRule("む", "んで", "JapaneseForms.TeForm")]
[SuffixRule("る", "って", "JapaneseForms.TeForm")]
[SuffixRule("う", "って", "JapaneseForms.TeForm")]
// Ichidan
[SuffixRule("る", "て", "JapaneseForms.TeForm", Accepts = "v1", Produces = "v1-te")]
// Irregulars (hiragana; kanji forms 来る / (compound)する handled at the reading layer)
[SuffixRule("くる", "きて", "JapaneseForms.TeForm", Accepts = "vk", Produces = "vk-te")]
[SuffixRule("する", "して", "JapaneseForms.TeForm", Accepts = "vs", Produces = "vs-te")]
// 行く exception — longer suffix wins via index probe ordering, default v5/v5-te applies
[SuffixRule("行く", "行って", "JapaneseForms.TeForm")]
public sealed partial class ShushikeiTeFormRules;
using Monowakaru.Languages.Generators.Attributes;

namespace Monowakaru.Languages.Japanese.Grammar;

/// <summary>
///     All suffix rules that transform a verb's 終止形 (shūshikei, dictionary form) into its
///     -て form, covering every godan row, ichidan, and the two irregulars 来る / する.
///     Ambiguous suffixes (e.g. る for godan ラ行 and ichidan) produce multiple candidates —
///     disambiguation belongs to the downstream deinflection engine.
/// </summary>
/// <remarks>
///     Known exception not modelled: 行く → 行って (not 行いて). Handled via a later per-verb
///     exception rule table.
/// </remarks>
[InflectionTable]
// Godan conjugation (one rule per 行)
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
[SuffixRule("る", "て", "JapaneseForms.TeForm")]
// Irregulars (hiragana; kanji forms 来る / (compound)する handled at the reading layer)
[SuffixRule("くる", "きて", "JapaneseForms.TeForm")]
[SuffixRule("する", "して", "JapaneseForms.TeForm")]
[SuffixRule("行く", "行って", "JapaneseForms.TeForm")]
public sealed partial class ShushikeiTeFormRules;
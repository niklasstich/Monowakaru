using Monowakaru.Languages.Grammar;
using Monowakaru.Languages.Grammar.Inflection;

namespace Monowakaru.Languages.Japanese.Grammar;

/// <summary>
///     Static <see cref="IInflectionForm" /> singletons referenced by name in
///     <c>[SuffixRule(..., form: "JapaneseForms.<i>Foo</i>")]</c> attributes. The generator emits
///     the path verbatim, so adding a new form here unlocks <c>nameof(JapaneseForms.<i>Foo</i>)</c>
///     usages in rule tables.
/// </summary>
public static class JapaneseForms
{
    /// <summary>The <c>-て</c> form (e.g. 書いて, 食べて) — covers all 五段 onbin variants and ichidan.</summary>
    public static readonly IInflectionForm TeForm = new InflectionForm(
        JapaneseWordClasses.Verb,
        null,
        new HashSet<IGrammaticalFeatureValue>
        {
            JapaneseGrammar.TeForm
        });

    /// <summary>The polite <c>-ます</c> form (e.g. 書きます, 食べます).</summary>
    public static readonly IInflectionForm MasuForm = new InflectionForm(
        JapaneseWordClasses.Verb,
        null,
        new HashSet<IGrammaticalFeatureValue>
        {
            JapaneseGrammar.MasuForm
        });
}
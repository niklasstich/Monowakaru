using Monowakaru.Languages.Grammar;

namespace Monowakaru.Languages.Japanese.Grammar;

/// <summary>
///     Static catalogue of Japanese grammatical features (categories) and the values they take,
///     used to tag generated <see cref="Monowakaru.Languages.Grammar.Inflection.IInflectionForm" />
///     instances. New forms reference these singletons so equality is by reference.
/// </summary>
public static class JapaneseGrammar
{
    /// <summary>接続形 — covers て-form (テ形) and similar conjunctive constructions.</summary>
    public static readonly IGrammaticalFeature ConjunctiveForm =
        new JapaneseGrammaticalFeature("Conjunctive form", "接続形");

    /// <summary>テ形 — verb in -て / -で form.</summary>
    public static readonly IGrammaticalFeatureValue TeForm =
        new JapaneseGrammaticalFeatureValue(ConjunctiveForm, "Te-form", "テ形");

    /// <summary>丁寧度 — politeness register feature; today only the polite (ます) value is modelled.</summary>
    public static readonly IGrammaticalFeature Politeness =
        new JapaneseGrammaticalFeature("Politeness", "丁寧度");

    /// <summary>ます形 — polite verb form built on 連用形 + ます.</summary>
    public static readonly IGrammaticalFeatureValue MasuForm =
        new JapaneseGrammaticalFeatureValue(Politeness, "Masu form", "ます形");
}
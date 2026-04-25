using Monowakaru.Languages.Grammar;

namespace Monowakaru.Languages.Japanese.Grammar;

public static class JapaneseGrammar
{
    public static readonly IGrammaticalFeature ConjunctiveForm =
        new JapaneseGrammaticalFeature("Conjunctive form", "接続形");

    public static readonly IGrammaticalFeatureValue TeForm =
        new JapaneseGrammaticalFeatureValue(ConjunctiveForm, "Te-form", "テ形");
}
using Monowakaru.Languages.Grammar;
using Monowakaru.Languages.Grammar.Inflection;

namespace Monowakaru.Languages.Japanese.Grammar;

public static class JapaneseForms
{
    public static readonly IInflectionForm TeForm = new InflectionForm(
        JapaneseWordClasses.Verb,
        null,
        new HashSet<IGrammaticalFeatureValue>
        {
            JapaneseGrammar.TeForm
        });
}
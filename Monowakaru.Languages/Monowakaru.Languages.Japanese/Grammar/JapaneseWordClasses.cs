using Monowakaru.Languages.Grammar;

namespace Monowakaru.Languages.Japanese.Grammar;

public static class JapaneseWordClasses
{
    public static readonly IWordClass Verb = new JapaneseWordClass("Verb", "動詞");
    public static readonly IWordClass IAdjective = new JapaneseWordClass("I-adjective", "形容詞");
    public static readonly IWordClass NaAdjective = new JapaneseWordClass("Na-adjective", "形容動詞");
}
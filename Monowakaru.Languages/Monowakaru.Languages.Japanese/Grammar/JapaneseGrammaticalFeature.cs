using Monowakaru.Languages.Grammar;

namespace Monowakaru.Languages.Japanese.Grammar;

public sealed record JapaneseGrammaticalFeature(string Name, string? NativeName) : IGrammaticalFeature;
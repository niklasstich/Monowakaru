using Monowakaru.Languages.Grammar;

namespace Monowakaru.Languages.Japanese.Grammar;

public sealed record JapaneseGrammaticalFeatureValue(IGrammaticalFeature Feature, string Name, string? NativeName)
    : IGrammaticalFeatureValue;
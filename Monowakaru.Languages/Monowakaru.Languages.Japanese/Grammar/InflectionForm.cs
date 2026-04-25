using Monowakaru.Languages.Grammar;
using Monowakaru.Languages.Grammar.Inflection;

namespace Monowakaru.Languages.Japanese.Grammar;

public sealed record InflectionForm(
    IWordClass InputClass,
    IWordClass? OutputClass,
    IReadOnlySet<IGrammaticalFeatureValue> Features) : IInflectionForm;
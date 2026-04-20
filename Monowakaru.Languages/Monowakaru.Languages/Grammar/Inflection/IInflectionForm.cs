namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     Describes an inflection form: the word class it applies to, the class it
///     produces (if different), and the grammatical features that characterize it.
/// </summary>
public interface IInflectionForm
{
    IWordClass InputClass { get; }
    IWordClass? OutputClass { get; }
    IReadOnlySet<IGrammaticalFeatureValue> Features { get; }
}
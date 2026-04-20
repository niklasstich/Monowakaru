namespace Monowakaru.Languages.Grammar;

/// <summary>
///     Describes a grammatical feature such as Tense, Polarity, Formality, Voice, etc.
///     Specific values of a feature must be declared as <see cref="IGrammaticalFeatureValue" />s.
///     For each occurrence of a grammatical feature, the values are mutually exclusive. For example,
///     a verb cannot be in both past and present form at the same time.
/// </summary>
public interface IGrammaticalFeature
{
    /// <summary>
    ///     The english name of the grammatical feature.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The name of the grammatical feature in the native language.
    /// </summary>
    string? NativeName { get; }
}
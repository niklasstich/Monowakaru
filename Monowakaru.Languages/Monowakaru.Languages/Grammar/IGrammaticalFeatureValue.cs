namespace Monowakaru.Languages.Grammar;

/// <summary>
///     Represents a specific value within a <see cref="IGrammaticalFeature" />.
/// </summary>
public interface IGrammaticalFeatureValue
{
    /// <summary>
    ///     The grammatical feature that this feature value is a value of.
    /// </summary>
    IGrammaticalFeature Feature { get; }

    /// <summary>
    ///     The english name of the grammatical feature value.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The name of the grammatical feature value in the native language.
    /// </summary>
    string? NativeName { get; }
}
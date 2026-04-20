namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     Suffix-swap strategy. Strips <see cref="RequiredSuffix" /> from the input
///     and appends <see cref="ReplacementSuffix" />.
/// </summary>
public interface ISuffixRule
{
    string RequiredSuffix { get; }
    string ReplacementSuffix { get; }
}
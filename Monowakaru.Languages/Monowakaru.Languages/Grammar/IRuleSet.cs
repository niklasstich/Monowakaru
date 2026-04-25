using Monowakaru.Languages.Grammar.Inflection;

namespace Monowakaru.Languages.Grammar;

/// <summary>
///     A declarative set of inflection and deinflection rules for a single grammatical
///     context (e.g. one conjugation group, one compound form).
/// </summary>
public interface IRuleSet
{
    IReadOnlyList<IInflectionRule> InflectionRules { get; }
    IReadOnlyList<IDeinflectionRule> DeinflectionRules { get; }

    /// <summary>
    ///     Returns every applicable inflection applied to <paramref name="term" />.
    ///     Each result pairs the matched rule with the inflected string it produced.
    ///     Multiple candidates are returned when more than one rule matches (e.g. different
    ///     conjugation classes sharing a suffix).
    /// </summary>
    IReadOnlyList<InflectionResult> Inflect(string term);

    /// <summary>
    ///     Returns every applicable deinflection applied to <paramref name="term" />.
    ///     Each result pairs the matched rule with the deinflected string it produced.
    ///     Probes every suffix length up to the longest rule suffix — a term may match
    ///     several rules simultaneously (short and long suffixes).
    /// </summary>
    IReadOnlyList<DeinflectionResult> Deinflect(string term);
}

public readonly record struct InflectionResult(IInflectionRule Rule, string Result);

public readonly record struct DeinflectionResult(IDeinflectionRule Rule, string Result);
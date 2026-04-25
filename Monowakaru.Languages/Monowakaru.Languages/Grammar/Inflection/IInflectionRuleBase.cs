namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     Common base for inflection and deinflection rules.
/// </summary>
public interface IInflectionRuleBase
{
    /// <summary>
    ///     The reverse rule. For an <see cref="IInflectionRule" /> this is the corresponding
    ///     <see cref="IDeinflectionRule" /> and vice versa.
    /// </summary>
    IInflectionRuleBase Inverse { get; }

    /// <summary>
    ///     For <see cref="IInflectionRule" />: the form this rule produces.<br />
    ///     For <see cref="IDeinflectionRule" />: the form this rule recognizes and removes.
    /// </summary>
    IInflectionForm Form { get; }

    /// <summary>
    ///     Conditions the term must currently be tagged with for this rule to apply, evaluated by
    ///     the deinflection engine when chaining rules. An empty list means "no constraint";
    ///     by convention the engine also treats an empty <em>currentRules</em> set (i.e. raw
    ///     untagged input at the start of a chain) as matching any rule, so first-step rules with
    ///     non-empty <see cref="AcceptsConditions" /> still fire on raw input.
    /// </summary>
    /// <remarks>
    ///     For an <see cref="IInflectionRule" /> these are the conditions of the input (pre-state).<br />
    ///     For an <see cref="IDeinflectionRule" /> these are the conditions of the inflected input.
    ///     The generator emits the deinflection inverse with <see cref="AcceptsConditions" /> /
    ///     <see cref="ProducesConditions" /> swapped relative to the inflection rule.
    /// </remarks>
    IReadOnlyList<IFormCondition> AcceptsConditions { get; }

    /// <summary>
    ///     Conditions the term will be tagged with after this rule is applied. Terminal IDs
    ///     (e.g. <c>"v5"</c>, <c>"v1"</c>, <c>"adj-i"</c>) end the deinflection chain and signal
    ///     that the result is a dictionary-form candidate.
    /// </summary>
    /// <remarks>
    ///     For an <see cref="IInflectionRule" /> these are the conditions of the produced form
    ///     (e.g. <c>"v5-te"</c>).<br />
    ///     For an <see cref="IDeinflectionRule" /> these are the conditions of the recovered base
    ///     form (e.g. <c>"v5"</c>).
    /// </remarks>
    IReadOnlyList<IFormCondition> ProducesConditions { get; }

    /// <summary>Whether <paramref name="term" />'s suffix matches this rule's required suffix.</summary>
    bool IsApplicable(string term);

    /// <summary>Apply the rule, returning the transformed term. Caller should ensure <see cref="IsApplicable" />.</summary>
    string Apply(string term);
}
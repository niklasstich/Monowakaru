namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     A tag describing the grammatical state of a term within an inflection / deinflection chain.
///     Conditions identify both terminal states (dictionary forms — <c>"v5"</c>, <c>"v1"</c>,
///     <c>"adj-i"</c>, ...) and intermediate states (<c>"v5-te"</c>, <c>"v5-mizen"</c>, ...) so the
///     deinflection engine can chain rules correctly: a rule applies only when the term's current
///     condition matches what the rule accepts.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="Id" /> values deliberately match Yomitan / JMdict POS &amp; deinflector tags
///         verbatim for terminal conditions. That way a deinflection candidate's terminal condition
///         can be compared directly against a dictionary entry's class tag, with no translation layer.
///     </para>
///     <para>
///         Intermediate conditions follow the convention <c>"{terminal}-{state}"</c>, e.g.
///         <c>"v5-te"</c> for "a v5 verb currently in te-form, awaiting further deinflection or
///         used as the connection point for compound forms".
///     </para>
///     <para>
///         See <c>compound_deinflection_design.md</c> for the broader chain model.
///     </para>
/// </remarks>
public interface IFormCondition
{
    /// <summary>
    ///     Stable string identifier for this condition.
    /// </summary>
    string Id { get; }
}
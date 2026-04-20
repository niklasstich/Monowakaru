namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     Represents a grammatical inflection rule.
/// </summary>
public interface IInflectionRule : IInflectionRuleBase
{
    /// <inheritdoc cref="IInflectionRuleBase.Inverse" />
    new IDeinflectionRule Inverse { get; }
}
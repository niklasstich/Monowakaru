namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     Represents a grammatical deinflection rule.
/// </summary>
public interface IDeinflectionRule : IInflectionRuleBase
{
    /// <inheritdoc cref="IInflectionRuleBase.Inverse" />
    new IInflectionRule Inverse { get; }
}
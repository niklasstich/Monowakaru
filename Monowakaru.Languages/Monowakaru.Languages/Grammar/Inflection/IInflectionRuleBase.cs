namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     Common base for inflection and deinflection rules.
/// </summary>
public interface IInflectionRuleBase
{
    IInflectionRuleBase Inverse { get; }

    /// <summary>
    ///     For <see cref="IInflectionRule" />: the form this rule produces.<br />
    ///     For <see cref="IDeinflectionRule" />: the form this rule recognizes and removes.
    /// </summary>
    IInflectionForm Form { get; }

    bool IsApplicable(string term);
    string Apply(string term);
}
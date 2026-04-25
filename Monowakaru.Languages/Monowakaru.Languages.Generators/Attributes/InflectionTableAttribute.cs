using System;

namespace Monowakaru.Languages.Generators.Attributes;

/// <summary>
///     Marks a partial class as an inflection rule table. Every <see cref="SuffixRuleAttribute" /> on
///     the class contributes one inflection / deinflection rule pair to the generated implementation,
///     and the class itself is given a generated <c>IRuleSet</c> body (rule arrays, frozen suffix
///     indices, span-based <c>Inflect</c> / <c>Deinflect</c> lookups).
/// </summary>
/// <remarks>
///     <see cref="DefaultAccepts" /> and <see cref="DefaultProduces" /> are convenience defaults
///     applied to any <see cref="SuffixRuleAttribute" /> whose own <c>Accepts</c> / <c>Produces</c>
///     are empty. Use them when most rules in a table share a class — e.g. a "shūshikei → te-form"
///     godan table sets <c>DefaultAccepts = "v5"</c> and <c>DefaultProduces = "v5-te"</c>, and only
///     the ichidan / kuru / suru rules override per-rule.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InflectionTableAttribute : Attribute
{
    /// <summary>
    ///     CSV of condition IDs used as <see cref="SuffixRuleAttribute.Accepts" /> when a rule omits its own.
    /// </summary>
    public string DefaultAccepts { get; init; } = "";

    /// <summary>
    ///     CSV of condition IDs used as <see cref="SuffixRuleAttribute.Produces" /> when a rule omits its own.
    /// </summary>
    public string DefaultProduces { get; init; } = "";
}
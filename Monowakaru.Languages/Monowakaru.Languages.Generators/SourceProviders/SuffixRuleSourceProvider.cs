using System.Linq;
using Monowakaru.Languages.Generators.GenerationTargets;

namespace Monowakaru.Languages.Generators.SourceProviders;

/// <summary>
///     Renders one <see cref="SuffixRuleGenerationTarget" /> as a pair of records: a private
///     <c>ISuffixInflectionRule</c> and its mirrored <c>ISuffixDeinflectionRule</c>. Both records
///     hold static singleton instances and pre-allocated arrays for accepts / produces conditions
///     so lookups remain allocation-free.
/// </summary>
public static class SuffixRuleSourceProvider
{
    public static GenerationResult GetImplementationSource(SuffixRuleGenerationTarget target, string recordBaseName)
    {
        // Inflection direction reads accepts/produces verbatim. Deinflection swaps them: the
        // deinflected term is the inflected input, so its 'accepts' is the inflection rule's
        // 'produces' and vice versa.
        var inflAccepts = FormatConditionArrayLiteral(target.AcceptsCsv);
        var inflProduces = FormatConditionArrayLiteral(target.ProducesCsv);
        var deinflAccepts = inflProduces;
        var deinflProduces = inflAccepts;

        var source = $$"""
                       private sealed record {{recordBaseName}}Inflection : ISuffixInflectionRule
                       {
                           public static readonly {{recordBaseName}}Inflection Instance = new();
                           private static readonly IFormCondition[] _accepts = {{inflAccepts}};
                           private static readonly IFormCondition[] _produces = {{inflProduces}};
                           public string RequiredSuffix => "{{target.InputSuffix}}";
                           public string ReplacementSuffix => "{{target.OutputSuffix}}";
                           public IInflectionForm Form => {{target.Form}};
                           public IReadOnlyList<IFormCondition> AcceptsConditions => _accepts;
                           public IReadOnlyList<IFormCondition> ProducesConditions => _produces;
                           public IDeinflectionRule Inverse => {{recordBaseName}}Deinflection.Instance;
                           IInflectionRuleBase IInflectionRuleBase.Inverse => Inverse;
                           public bool IsApplicable(string term) => term.EndsWith("{{target.InputSuffix}}", System.StringComparison.Ordinal);
                           public string Apply(string term) => string.Concat(term.AsSpan(0, term.Length - {{target.InputSuffix.Length}}), "{{target.OutputSuffix}}");
                           public override string ToString() => $"{Form}: {RequiredSuffix}->{ReplacementSuffix}";
                       }

                       private sealed record {{recordBaseName}}Deinflection : ISuffixDeinflectionRule
                       {
                           public static readonly {{recordBaseName}}Deinflection Instance = new();
                           private static readonly IFormCondition[] _accepts = {{deinflAccepts}};
                           private static readonly IFormCondition[] _produces = {{deinflProduces}};
                           public string RequiredSuffix => "{{target.OutputSuffix}}";
                           public string ReplacementSuffix => "{{target.InputSuffix}}";
                           public IInflectionForm Form => {{target.Form}};
                           public IReadOnlyList<IFormCondition> AcceptsConditions => _accepts;
                           public IReadOnlyList<IFormCondition> ProducesConditions => _produces;
                           public IInflectionRule Inverse => {{recordBaseName}}Inflection.Instance;
                           IInflectionRuleBase IInflectionRuleBase.Inverse => Inverse;
                           public bool IsApplicable(string term) => term.EndsWith("{{target.OutputSuffix}}", System.StringComparison.Ordinal);
                           public string Apply(string term) => string.Concat(term.AsSpan(0, term.Length - {{target.OutputSuffix.Length}}), "{{target.InputSuffix}}");
                           public override string ToString() => $"{Form}: {RequiredSuffix}->{ReplacementSuffix}";
                       }
                       """;
        return GenerationResult.Ok(source);
    }

    /// <summary>
    ///     Formats a CSV of condition IDs as a C# array-creation expression. Empty / whitespace-only
    ///     input produces <c>System.Array.Empty&lt;IFormCondition&gt;()</c>; non-empty input emits
    ///     <c>new IFormCondition[] { FormCondition.Of("id1"), ... }</c>. Items are trimmed and
    ///     properly C#-escaped for embedding in a string literal.
    /// </summary>
    private static string FormatConditionArrayLiteral(string csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
            return "global::System.Array.Empty<IFormCondition>()";

        var ids = csv.Split(',')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToList();
        if (ids.Count == 0)
            return "global::System.Array.Empty<IFormCondition>()";

        var elements = string.Join(", ", ids.Select(id => $"FormCondition.Of(\"{Escape(id)}\")"));
        return $"new IFormCondition[] {{ {elements} }}";
    }

    private static string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
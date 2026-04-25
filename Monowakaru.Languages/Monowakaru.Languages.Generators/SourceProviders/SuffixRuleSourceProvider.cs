using Monowakaru.Languages.Generators.GenerationTargets;

namespace Monowakaru.Languages.Generators.SourceProviders;

public static class SuffixRuleSourceProvider
{
    public static GenerationResult GetImplementationSource(SuffixRuleGenerationTarget target, string recordBaseName)
    {
        var source = $$"""
                       private sealed record {{recordBaseName}}Inflection : ISuffixInflectionRule
                       {
                           public static readonly {{recordBaseName}}Inflection Instance = new();
                           public string RequiredSuffix => "{{target.InputSuffix}}";
                           public string ReplacementSuffix => "{{target.OutputSuffix}}";
                           public IInflectionForm Form => {{target.Form}};
                           public IDeinflectionRule Inverse => {{recordBaseName}}Deinflection.Instance;
                           IInflectionRuleBase IInflectionRuleBase.Inverse => Inverse;
                           public bool IsApplicable(string term) => term.EndsWith("{{target.InputSuffix}}", StringComparison.Ordinal);
                           public string Apply(string term) => string.Concat(term.AsSpan(0, term.Length - {{target.InputSuffix.Length}}), "{{target.OutputSuffix}}");
                           public override string ToString() => $"{Form}: {RequiredSuffix}->{ReplacementSuffix}";
                       }

                       private sealed record {{recordBaseName}}Deinflection : ISuffixDeinflectionRule
                       {
                           public static readonly {{recordBaseName}}Deinflection Instance = new();
                           public string RequiredSuffix => "{{target.OutputSuffix}}";
                           public string ReplacementSuffix => "{{target.InputSuffix}}";
                           public IInflectionForm Form => {{target.Form}};
                           public IInflectionRule Inverse => {{recordBaseName}}Inflection.Instance;
                           IInflectionRuleBase IInflectionRuleBase.Inverse => Inverse;
                           public bool IsApplicable(string term) => term.EndsWith("{{target.OutputSuffix}}", StringComparison.Ordinal);
                           public string Apply(string term) => string.Concat(term.AsSpan(0, term.Length - {{target.OutputSuffix.Length}}), "{{target.InputSuffix}}");
                           public override string ToString() => $"{Form}: {RequiredSuffix}->{ReplacementSuffix}";
                       }
                       """;
        return GenerationResult.Ok(source);
    }
}
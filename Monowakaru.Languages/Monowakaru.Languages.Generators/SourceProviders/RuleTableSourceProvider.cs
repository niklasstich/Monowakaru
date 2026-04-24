using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Monowakaru.Languages.Generators.Diagnostics;
using Monowakaru.Languages.Generators.GenerationTargets;

namespace Monowakaru.Languages.Generators.SourceProviders;

public static class RuleTableSourceProvider
{
    public static GenerationResult GetImplementationSource(RuleTableGenerationTarget target)
    {
        return GetRuleImplementationRecords(target.RuleGenerationTargets).Map(rules =>
            $$"""
              namespace {{target.Namespace}}

              public sealed partial class {{target.ClassName}} : IRuleSet
              {
                  {{rules}}
              }
              """
        );
    }

    private static GenerationResult GetRuleImplementationRecords(ImmutableArray<RuleGenerationTarget> targets)
    {
        var names = ComputeUniqueRecordBaseNames(targets);
        return targets
            .Zip(names, (target, name) => GetRuleImplementationRecord(target, name))
            .JoinResults("\n\n");
    }

    private static GenerationResult GetRuleImplementationRecord(RuleGenerationTarget target, string recordBaseName)
    {
        return target switch
        {
            SuffixRuleGenerationTarget s => SuffixRuleSourceProvider.GetImplementationSource(s, recordBaseName),
            _ => GenerationResult.Fail(GeneratorDiagnostics.UnknownRuleType, Location.None, target.GetType().Name)
        };
    }

    private static IEnumerable<string> ComputeUniqueRecordBaseNames(ImmutableArray<RuleGenerationTarget> targets)
    {
        var baseNames = targets.Select(GetBaseNameForTarget).ToList();
        var counts = baseNames.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());
        var counters = new Dictionary<string, int>();

        return baseNames.Select(name =>
        {
            if (counts[name] == 1) return name;
            counters.TryGetValue(name, out var i);
            counters[name] = i + 1;
            return $"{name}{i}";
        });
    }

    private static string GetBaseNameForTarget(RuleGenerationTarget target)
    {
        return target switch
        {
            SuffixRuleGenerationTarget s => GetFormName(s.Form),
            _ => target.GetType().Name
        };
    }

    private static string GetFormName(string form)
    {
        var lastDot = form.LastIndexOf('.');
        return lastDot >= 0 ? form[(lastDot + 1)..] : form;
    }
}
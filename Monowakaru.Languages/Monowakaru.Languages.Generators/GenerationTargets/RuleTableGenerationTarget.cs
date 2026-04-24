using System.Collections.Immutable;

namespace Monowakaru.Languages.Generators.GenerationTargets;

public record RuleTableGenerationTarget(
    string Namespace,
    string ClassName,
    ImmutableArray<RuleGenerationTarget> RuleGenerationTargets)
{
}
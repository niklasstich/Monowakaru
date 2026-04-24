namespace Monowakaru.Languages.Generators.GenerationTargets;

public record SuffixRuleGenerationTarget(string InputSuffix, string OutputSuffix, string Form)
    : RuleGenerationTarget(Form);
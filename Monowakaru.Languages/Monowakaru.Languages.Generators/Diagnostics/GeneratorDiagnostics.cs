using Microsoft.CodeAnalysis;

namespace Monowakaru.Languages.Generators.Diagnostics;

internal static class GeneratorDiagnostics
{
    internal static readonly DiagnosticDescriptor UnknownRuleType = new(
        "MWG001",
        "Unknown rule type",
        "No source provider for rule type '{0}'. Ensure the generator handles this type.",
        "RuleTableGenerator",
        DiagnosticSeverity.Error,
        true);
}
namespace Monowakaru.Languages.Generators.GenerationTargets;

/// <summary>
///     DTO describing one suffix-swap rule, threaded through the incremental generator pipeline.
/// </summary>
/// <remarks>
///     Conditions are stored as comma-separated normalised strings rather than collections so the
///     record's structural equality (used by Roslyn's incremental cache) compares by value cheaply.
///     The source provider splits these into individual IDs at codegen time.
/// </remarks>
/// <param name="InputSuffix">Inflection-direction input suffix (the dictionary / base form suffix).</param>
/// <param name="OutputSuffix">Inflection-direction output suffix.</param>
/// <param name="Form">Fully-qualified expression resolving to a static <c>IInflectionForm</c>.</param>
/// <param name="AcceptsCsv">Resolved accepts condition IDs (CSV; empty = no constraint).</param>
/// <param name="ProducesCsv">Resolved produces condition IDs (CSV; empty = no produced tag).</param>
public record SuffixRuleGenerationTarget(
    string InputSuffix,
    string OutputSuffix,
    string Form,
    string AcceptsCsv,
    string ProducesCsv) : RuleGenerationTarget(Form);
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monowakaru.Languages.Generators.Attributes;
using Monowakaru.Languages.Generators.GenerationTargets;
using Monowakaru.Languages.Generators.SourceProviders;

namespace Monowakaru.Languages.Generators;

/// <summary>
///     Incremental source generator that materialises every <c>[InflectionTable]</c>-decorated partial
///     class into a full <c>IRuleSet</c> implementation. Reads <see cref="SuffixRuleAttribute" /> data
///     plus table-level defaults from <see cref="InflectionTableAttribute" />, resolves accepts/produces
///     condition CSVs, and delegates source rendering to <see cref="RuleTableSourceProvider" />.
/// </summary>
[Generator]
public class RuleTableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var ruleTablesToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(InflectionTableAttribute).FullName!,
            static (node, _) => IsClass(node),
            static (ctx, _) => TryGetRuleTableGenerationTarget(ctx)
        );

        context.RegisterImplementationSourceOutput(ruleTablesToGenerate, Execute);
    }

    private static void Execute(SourceProductionContext context, RuleTableGenerationTarget? target)
    {
        if (target is null) return;

        var result = RuleTableSourceProvider.GetImplementationSource(target);

        foreach (var diagnostic in result.Errors)
            context.ReportDiagnostic(diagnostic);

        if (!result.Success) return;

        var hintName = $"{target.Namespace}.{target.ClassName}.g.cs";
        context.AddSource(hintName, result.GeneratedSource!);
    }

    /// <summary>
    ///     Verifies the decorated class is annotated with <see cref="InflectionTableAttribute" />
    ///     (matched by full type name to avoid name-collision false positives) and, if so, builds the
    ///     <see cref="RuleTableGenerationTarget" /> for it.
    /// </summary>
    private static RuleTableGenerationTarget? TryGetRuleTableGenerationTarget(GeneratorAttributeSyntaxContext ctx)
    {
        return ((ClassDeclarationSyntax)ctx.TargetNode).AttributeLists
            .SelectMany(list => list.Attributes)
            .Select(attributeSyntax =>
                ctx.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol
            )
            .Where(symbol => symbol is IMethodSymbol)
            .Select(symbol => symbol!.ContainingType)
            .Where(containingType => containingType.ToDisplayString() == typeof(InflectionTableAttribute).FullName)
            .Select(_ => GetRuleTableGenerationTargetFor((INamedTypeSymbol)ctx.TargetSymbol))
            .FirstOrDefault();
    }

    private static RuleTableGenerationTarget GetRuleTableGenerationTargetFor(INamedTypeSymbol targetClass)
    {
        var containingNamespace = targetClass.ContainingNamespace.ToDisplayString();
        var className = targetClass.Name;
        var (defaultAccepts, defaultProduces) = ExtractTableDefaults(targetClass);
        var rules = ExtractRules(targetClass, defaultAccepts, defaultProduces).ToImmutableArray();
        return new RuleTableGenerationTarget(containingNamespace, className, rules);
    }

    /// <summary>
    ///     Reads <see cref="InflectionTableAttribute.DefaultAccepts" /> and
    ///     <see cref="InflectionTableAttribute.DefaultProduces" /> off the table class. Both default to
    ///     empty when the attribute or properties are missing.
    /// </summary>
    private static (string Accepts, string Produces) ExtractTableDefaults(INamedTypeSymbol targetClass)
    {
        var attr = targetClass.GetAttributes().FirstOrDefault(a =>
            a.AttributeClass is not null
            && a.AttributeClass.ToDisplayString() == typeof(InflectionTableAttribute).FullName);
        if (attr is null) return ("", "");

        var accepts = ReadStringNamedArg(attr, nameof(InflectionTableAttribute.DefaultAccepts));
        var produces = ReadStringNamedArg(attr, nameof(InflectionTableAttribute.DefaultProduces));
        return (accepts, produces);
    }

    private static IEnumerable<RuleGenerationTarget> ExtractRules(
        INamedTypeSymbol targetClass,
        string defaultAccepts,
        string defaultProduces)
    {
        return ExtractSuffixRules(targetClass, defaultAccepts, defaultProduces);
    }

    private static IEnumerable<SuffixRuleGenerationTarget> ExtractSuffixRules(
        INamedTypeSymbol targetClass,
        string defaultAccepts,
        string defaultProduces)
    {
        var suffixRuleAttributes = targetClass.GetAttributes()
            .Where(attribute => attribute.AttributeClass is not null)
            .Where(attribute =>
                $"{attribute.AttributeClass!.ContainingNamespace.ToDisplayString()}.{attribute.AttributeClass.Name}" ==
                typeof(SuffixRuleAttribute).FullName);

        return suffixRuleAttributes.Select(AttributeDataToSuffixRuleTarget);

        SuffixRuleGenerationTarget AttributeDataToSuffixRuleTarget(AttributeData data)
        {
            var inputSuffix = (string)data.ConstructorArguments[0].Value!;
            var outputSuffix = (string)data.ConstructorArguments[1].Value!;
            var form = (string)data.ConstructorArguments[2].Value!;

            // Per-rule Accepts/Produces are init-only properties → exposed via NamedArguments.
            // Empty (or unset) falls back to the table-level default.
            var accepts = ResolveOrDefault(
                ReadStringNamedArg(data, nameof(SuffixRuleAttribute.Accepts)),
                defaultAccepts);
            var produces = ResolveOrDefault(
                ReadStringNamedArg(data, nameof(SuffixRuleAttribute.Produces)),
                defaultProduces);

            return new SuffixRuleGenerationTarget(inputSuffix, outputSuffix, form, accepts, produces);
        }
    }

    private static string ResolveOrDefault(string ruleValue, string tableDefault)
    {
        return string.IsNullOrWhiteSpace(ruleValue) ? tableDefault : ruleValue;
    }

    /// <summary>
    ///     Reads a string-typed <c>NamedArgument</c> from an <see cref="AttributeData" />. Returns
    ///     empty string when missing or null-valued, so callers do not need to handle nullability.
    /// </summary>
    private static string ReadStringNamedArg(AttributeData data, string name)
    {
        var entry = data.NamedArguments.FirstOrDefault(kv => kv.Key == name);
        if (entry.Key is null) return "";
        return entry.Value.Value as string ?? "";
    }

    private static bool IsClass(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax;
    }
}
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monowakaru.Languages.Generators.Attributes;
using Monowakaru.Languages.Generators.GenerationTargets;
using Monowakaru.Languages.Generators.SourceProviders;

namespace Monowakaru.Languages.Generators;

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
        var rules = ExtractRules(targetClass).ToImmutableArray();
        return new RuleTableGenerationTarget(containingNamespace, className, rules);
    }

    private static IEnumerable<RuleGenerationTarget> ExtractRules(INamedTypeSymbol targetClass)
    {
        return ExtractSuffixRules(targetClass);
    }

    private static IEnumerable<SuffixRuleGenerationTarget> ExtractSuffixRules(INamedTypeSymbol targetClass)
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
            return new SuffixRuleGenerationTarget(inputSuffix, outputSuffix, form);
        }
    }

    private static bool IsClass(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax;
    }
}
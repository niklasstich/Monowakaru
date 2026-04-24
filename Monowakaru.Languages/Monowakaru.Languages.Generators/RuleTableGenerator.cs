using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Monowakaru.Languages.Generators.Attributes;
using Monowakaru.Languages.Generators.GenerationTargets;

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

    private static void Execute(SourceProductionContext arg1, RuleTableGenerationTarget? arg2)
    {
        throw new NotImplementedException();
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
            .Select(GetRuleTableGenerationTargetFor)
            .FirstOrDefault();
    }

    private static RuleTableGenerationTarget GetRuleTableGenerationTargetFor(INamedTypeSymbol containingType)
    {
        var containingNamespace = containingType.ContainingNamespace.ToDisplayString();
        var className = containingType.Name;
        var rules = ExtractRules(containingType).ToImmutableArray();
        return new RuleTableGenerationTarget(containingNamespace, className, rules);
    }

    private static IEnumerable<RuleGenerationTarget> ExtractRules(INamedTypeSymbol containingType)
    {
        return ExtractSuffixRules(containingType);
    }

    private static IEnumerable<SuffixRuleGenerationTarget> ExtractSuffixRules(INamedTypeSymbol containingType)
    {
        var suffixRuleAttributes = containingType.GetAttributes()
            .Where(attribute => attribute.AttributeClass is not null)
            .Where(attribute =>
                $"{attribute.AttributeClass!.ContainingNamespace.Name}.{attribute.AttributeClass.Name}" ==
                typeof(SuffixRuleAttribute).FullName);

        return suffixRuleAttributes.Select(AttributeDataToSuffixRuleTarget);

        SuffixRuleGenerationTarget AttributeDataToSuffixRuleTarget(AttributeData arg1)
        {
            var inputSuffix = (string)arg1.ConstructorArguments[0].Value!;
            var outputSuffix = (string)arg1.ConstructorArguments[1].Value!;
            var form = (string)arg1.ConstructorArguments[2].Value!;
            return new SuffixRuleGenerationTarget(inputSuffix, outputSuffix, form);
        }
    }

    private static bool IsClass(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Monowakaru.Languages.Generators.SourceProviders;

public readonly struct GenerationResult
{
    private readonly List<Diagnostic> _errors;
    public string? GeneratedSource { get; }

    public IEnumerable<Diagnostic> Errors => _errors;

    public bool Success => GeneratedSource is not null;

    private GenerationResult(string source)
    {
        GeneratedSource = source;
        _errors = [];
    }

    private GenerationResult(string? source, List<Diagnostic> errors)
    {
        GeneratedSource = source;
        _errors = errors;
    }

    private GenerationResult(string? source, Diagnostic? error)
    {
        GeneratedSource = source;
        _errors = error is not null ? [error] : [];
    }

    public static GenerationResult Ok(string source)
    {
        return new GenerationResult(source);
    }

    public static GenerationResult PartiallyOk(string source, Diagnostic error)
    {
        return new GenerationResult(source, error);
    }

    public static GenerationResult PartiallyOk(string source, DiagnosticDescriptor descriptor, Location location, params object[] args)
    {
        return new GenerationResult(source, Diagnostic.Create(descriptor, location, args));
    }

    public static GenerationResult Fail(Diagnostic error)
    {
        return new GenerationResult(null, error);
    }

    public static GenerationResult Fail(DiagnosticDescriptor descriptor, Location location, params object[] args)
    {
        return Fail(Diagnostic.Create(descriptor, location, args));
    }

    public static GenerationResult Join(IEnumerable<GenerationResult> results, string separator)
    {
        var resultList = results as IList<GenerationResult> ?? results.ToList();
        var errors = resultList.SelectMany(r => r.Errors).ToList();
        var sources = resultList.Where(r => r.Success).Select(r => r.GeneratedSource!).ToList();
        var source = sources.Count > 0 ? string.Join(separator, sources) : null;
        return new GenerationResult(source, errors);
    }

    public GenerationResult Map(Func<string, string> transform)
    {
        if (!Success) return this;
        return new GenerationResult(transform(GeneratedSource!), _errors);
    }

    public override string ToString()
    {
        return !Success ? "" : GeneratedSource!;
    }
}

public static class GenerationResultExtensions
{
    public static GenerationResult JoinResults(this IEnumerable<GenerationResult> results, string separator)
    {
        return GenerationResult.Join(results, separator);
    }
}
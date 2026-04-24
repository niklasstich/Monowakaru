using System;

namespace Monowakaru.Languages.Generators.Attributes;

/// <summary>
///     TODO
/// </summary>
/// <param name="inputSuffix">TODO </param>
/// <param name="outputSuffix">TODO </param>
/// <param name="form">TODO </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class SuffixRuleAttribute(string inputSuffix, string outputSuffix, string form) : Attribute
{
    public string InputSuffix { get; } = inputSuffix;
    public string OutputSuffix { get; } = outputSuffix;
    public string Form { get; } = form;
}
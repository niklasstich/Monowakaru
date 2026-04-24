using System;

namespace Monowakaru.Languages.Generators.Attributes;

/// <summary>
///     Marks a partial class as an inflection rule table. All implementations of <see cref="SuffixRuleAttribute" />
///     on the class will be emitted in this class.
/// </summary>
// ReSharper disable once RedundantAttributeUsageProperty - for explicitness
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class InflectionTableAttribute : Attribute;
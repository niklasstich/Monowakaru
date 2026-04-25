using System;

namespace Monowakaru.Languages.Generators.Attributes;

/// <summary>
///     Declares one suffix-swap rule pair within a class marked with <see cref="InflectionTableAttribute" />.
///     Every attribute corresponds to a generated <c>ISuffixInflectionRule</c> + <c>ISuffixDeinflectionRule</c>
///     pair; the deinflection inverse is emitted automatically by swapping <see cref="InputSuffix" /> with
///     <see cref="OutputSuffix" /> and <see cref="Accepts" /> with <see cref="Produces" />.
/// </summary>
/// <remarks>
///     <para>
///         <b>Direction convention.</b>
///         <see cref="Accepts" /> and <see cref="Produces" /> describe the rule in the
///         <i>inflection</i> direction — i.e. <see cref="Accepts" /> is the condition the input must
///         currently be tagged with (e.g. <c>"v5"</c>: the rule consumes a v5 dictionary form), and
///         <see cref="Produces" /> is the condition the output is tagged with (e.g. <c>"v5-te"</c>).
///         The generator emits the deinflection inverse with these swapped, so deinflection's
///         <see cref="IInflectionRuleBase.AcceptsConditions" /> equals the inflection rule's
///         <see cref="Produces" />, and vice versa.
///     </para>
///     <para>
///         <b>Empty values.</b>
///         Empty <see cref="Accepts" /> / <see cref="Produces" /> mean "inherit from the table-level
///         <see cref="InflectionTableAttribute.DefaultAccepts" /> /
///         <see cref="InflectionTableAttribute.DefaultProduces" />". After resolution, empty still
///         means "no constraint / no produced tag" (the deinflection engine treats an empty accepts
///         set as 'matches any current condition').
///     </para>
///     <para>
///         <b>CSV format.</b>
///         Multiple condition IDs are comma-separated, e.g. <c>"v5,v5k"</c>. Whitespace around items
///         is trimmed. Use this when one rule is genuinely class-polymorphic.
///     </para>
/// </remarks>
/// <param name="inputSuffix">
///     Suffix on the input form. For inflection: dictionary / base suffix this rule transforms.
///     For deinflection (auto-generated inverse): the inflected-form suffix matched against terms.
/// </param>
/// <param name="outputSuffix">Suffix that replaces <paramref name="inputSuffix" /> in inflection direction.</param>
/// <param name="form">
///     Fully-qualified path to a static <c>IInflectionForm</c> property (e.g. <c>"JapaneseForms.TeForm"</c>).
///     Resolved by the generator into a verbatim expression in the emitted source.
/// </param>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class SuffixRuleAttribute(string inputSuffix, string outputSuffix, string form) : Attribute
{
    public string InputSuffix { get; } = inputSuffix;
    public string OutputSuffix { get; } = outputSuffix;
    public string Form { get; } = form;

    /// <summary>
    ///     CSV of condition IDs the term must currently be tagged with (inflection direction).
    ///     Empty defers to <see cref="InflectionTableAttribute.DefaultAccepts" />.
    /// </summary>
    public string Accepts { get; init; } = "";

    /// <summary>
    ///     CSV of condition IDs the term will be tagged with after applying the rule (inflection direction).
    ///     Empty defers to <see cref="InflectionTableAttribute.DefaultProduces" />.
    /// </summary>
    public string Produces { get; init; } = "";
}
using System.Collections.Concurrent;

namespace Monowakaru.Languages.Grammar.Inflection;

/// <summary>
///     Default <see cref="IFormCondition" /> implementation. Conditions are interned by
///     <see cref="Id" /> via <see cref="Of" /> so that the same string yields the same instance —
///     callers get cheap reference equality on top of the value equality the record provides.
/// </summary>
/// <param name="Id">Stable identifier (e.g. Yomitan POS tag or hyphenated intermediate tag).</param>
/// <remarks>
///     The generator emits <see cref="Of" /> calls into rule tables, so all rule tables for the
///     same condition ID share one allocation. Equality between two <see cref="FormCondition" />s
///     uses the record-generated <see cref="object.Equals(object?)" /> (compares <see cref="Id" />)
///     so consumers can also build new instances directly without breaking comparisons.
/// </remarks>
public sealed record FormCondition(string Id) : IFormCondition
{
    private static readonly ConcurrentDictionary<string, FormCondition> Interned = new();

    /// <summary>
    ///     Returns a shared <see cref="FormCondition" /> for <paramref name="id" />, allocating once
    ///     per distinct ID for the lifetime of the process.
    /// </summary>
    public static FormCondition Of(string id)
    {
        return Interned.GetOrAdd(id, static k => new FormCondition(k));
    }
}
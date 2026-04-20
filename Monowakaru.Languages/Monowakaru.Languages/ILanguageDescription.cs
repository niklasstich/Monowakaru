namespace Monowakaru.Languages;

/// <summary>
///     Language description according to ISO 639.
/// </summary>
public interface ILanguageDescription
{
    /// <summary>The display name, the primary ISO language name.</summary>
    string DisplayName { get; }

    /// <summary>Alternative ISO language names.</summary>
    IEnumerable<string> AlternativeDisplayNames { get; }

    /// <summary>The language code defined in ISO 639-1.</summary>
    string Set1Name { get; }

    /// <summary>The terminological language code defined in ISO 639-2.</summary>
    string Set2TName { get; }

    /// <summary>The bibliographic language code defined in ISO 639-2.</summary>
    string Set2BName { get; }

    /// <summary>The language code defined in ISO 639-3.</summary>
    string Set3Name { get; }

    /// <summary>The name of the language, in the language.</summary>
    string NativeName { get; }

    /// <summary>
    ///     The writing systems common to the language.
    /// </summary>
    IEnumerable<IWritingSystem> WritingSystems { get; }
}
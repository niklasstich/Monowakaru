using System.Text.Unicode;

namespace Monowakaru.Languages;

/// <summary>
///     Description of a writing system.
/// </summary>
public interface IWritingSystem
{
    string DisplayName { get; }
    string NativeName { get; }
    IEnumerable<UnicodeRange> Ranges { get; }
}
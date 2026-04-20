namespace Monowakaru.Languages.Grammar;

public interface IWordClass
{
    /// <summary>
    ///     The english name of the word class.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The name of the word class in the native language.
    /// </summary>
    string? NativeName { get; }
}
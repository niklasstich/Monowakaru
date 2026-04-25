using Monowakaru.Languages.Grammar;

namespace Monowakaru.Languages.Japanese.Grammar;

public sealed record JapaneseWordClass(string Name, string? NativeName) : IWordClass;
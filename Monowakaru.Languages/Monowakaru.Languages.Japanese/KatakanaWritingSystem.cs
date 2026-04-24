using System.Text.Unicode;

namespace Monowakaru.Languages.Japanese;

public class KatakanaWritingSystem : IWritingSystem
{
    public string DisplayName => "Katakana";
    public string NativeName => "カタカナ";
    public IEnumerable<UnicodeRange> Ranges => [new(0x30A0, 0x60)];
}
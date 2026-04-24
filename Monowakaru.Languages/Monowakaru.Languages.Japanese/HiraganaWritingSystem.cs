using System.Text.Unicode;

namespace Monowakaru.Languages.Japanese;

public class HiraganaWritingSystem : IWritingSystem
{
    public string DisplayName => "Hiragana";
    public string NativeName => "ひらがな";
    public IEnumerable<UnicodeRange> Ranges => [new(0x3040, 0x60)];
}
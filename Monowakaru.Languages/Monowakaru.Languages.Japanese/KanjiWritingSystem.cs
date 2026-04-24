using System.Text.Unicode;

namespace Monowakaru.Languages.Japanese;

public class KanjiWritingSystem : IWritingSystem
{
    public string DisplayName => "Kanji";
    public string NativeName => "感じ";
    public IEnumerable<UnicodeRange> Ranges => [new(0x4E00, 0x51C0)];
}
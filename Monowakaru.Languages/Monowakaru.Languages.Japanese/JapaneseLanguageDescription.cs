namespace Monowakaru.Languages.Japanese;

public class JapaneseLanguageDescription : ILanguageDescription
{
    public string DisplayName => "Japanese";
    public IEnumerable<string> AlternativeDisplayNames => [];
    public string Set1Name => "ja";
    public string Set2TName => "jpn";
    public string Set2BName => "jpn";
    public string Set3Name => "jpn";
    public string NativeName => "日本語";

    public IEnumerable<IWritingSystem> WritingSystems =>
    [
        new HiraganaWritingSystem(), new KatakanaWritingSystem(), new KanjiWritingSystem()
    ];
}
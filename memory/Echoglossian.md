# Echoglossian Investigation Report

**Repo**: `/home/club/code/Echoglossian/`
**Date**: 2026-04-14

## What It Is

Echoglossian is a Dalamud plugin that **translates game text in realtime** as you play FFXIV. It captures text from native game UI addons, sends to translation APIs, and displays results either in-place (replacing ATK node text) or as ImGui overlay windows.

**Key distinction vs our goal**: Echoglossian does full-sentence machine translation. It does NOT do word-level dictionary lookup, morphological analysis, or Yomitan/JMdict integration. That is the gap Monowakaru fills.

---

## How It Hooks Game UI

### Lifecycle Events (`IAddonLifecycle`)

```
AddonLifecycle.RegisterListener(AddonEvent.PreRefresh, addonName, handler)
AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, addonName, handler)
```

- `PreRefresh` — capture original text *before* game processes addon
- `PostUpdate` — write translated text *after* game updates node state
- `PreDraw` — final pass before render
- `PreFinalize` / `PreHide` — reset state on close

**Key file**: `NativeUI/Helpers/AddonHandlerWiring.cs` — maps 20+ addon names to handlers.

### Reading ATK Nodes (unsafe)

```csharp
var addonPtr = GameGuiInterface.GetAddonByName("Talk");
var talkAddon = (AtkUnitBase*)addonPtr.Address;
var textNode = talkAddon->GetTextNodeById(3); // node 3 = body, node 2 = NPC name
```

For menus/skills/items, reads `AtkArrayDataHolder->StringArrayData` arrays directly off `AtkStage.Instance()`.

### Writing Back Translated Text

`NativeUI/Helpers/AtkTextNodeBufferWrapper.cs`:
- Allocates unmanaged UTF-8 buffer via `NativeMemory.Alloc()`
- Encodes C# string to UTF-8 and writes to node's text buffer
- Manages buffer lifetimes, frees on close

---

## Text Targets Covered

| Addon Name | What It Is |
|---|---|
| `Talk` | Main NPC dialogue |
| `_BattleTalk` | Combat NPC dialogue |
| `TalkSubtitle` | Cutscene subtitles |
| `_MiniTalk` | Floating dialogue bubbles |
| `JournalAccept` / `Journal` / `JournalResult` | Quest journal |
| `_WideText`, `_TextError`, `_TextClassChange` | Toast notifications |
| `TextGimmickHint` | Environmental hints |
| `CutSceneSelectString` | Dialogue choice options |
| `Character` | Character window text |
| `StringArrayData` (many) | Menus, skills, items, status effects |

---

## Translation Architecture

### Engines (15+)

`Translators/` contains: Google, DeepL, ChatGPT, Gemini, Ollama, LibreTranslate, LM Studio, Microsoft Azure, Yandex, Amazon, OpenRouter, DeepSeek.

All implement `ITranslator` with `Translate()` / `TranslateAsync()`.

`TranslationService.cs` selects engine based on `Config.ChosenTransEngine`.

### Async Queue

`Services/QueuedTranslationBroker.cs` — queues translation jobs via `Task.Run()` off the game loop to prevent UI stutter. Each translation target has a `SemaphoreSlim(1,1)` guard.

### Three-Tier Cache

1. **Session cache** — in-memory dict per addon type
2. **Persistent DB** — SQLite via EF Core, queried by original text hash
3. **Translation API** — only hit on cache miss

---

## Display Modes

### Native Replacement
Overwrites ATK text node in-place. Original text gone. Looks seamless.

### ImGui Overlay
`UIOverlays/TranslationOverlay/TranslationOverlayDrawer.cs` — separate transparent ImGui windows positioned near game addons.

Multiple overlay instances, one per UI element type. Some addon types (MiniTalk) create one overlay *per bubble instance* keyed by addon pointer.

### Hover Tooltip
`NativeUI/Helpers/HoverTooltipManager.cs`:
- Tracks rectangle regions (addon bounds)
- Draws DelvUI-style tooltip at cursor when mouse hovers registered region
- 80-char line wrap
- 30-second TTL for stale entries

---

## Database (EF Core SQLite)

`EFCoreSqlite/EchoglossianDbContext.cs`

Models (15+):
- `TalkMessage` — speaker, original, translated, language, engine, timestamp
- `QuestPlate` — quest text + objectives + summaries + quest ID
- `ToastMessage`, `BattleTalkMessage`, `TalkSubtitleMessage`
- `StringArrayDatas` — UI string arrays
- `ActionTooltip`, `ItemTooltip` — skill/item descriptions
- `LocationName`, `NpcNames`

Migrations in `EFCoreSqlite/Migrations/`. Recent one (`20260324193000_AddLookupIndexes`) added lookup indexes on original text columns for fast cache queries.

---

## Quest / Lumina Integration

`NativeUI/Helpers/QuestLuminaResolver.cs` — uses `IDataManager` to query Lumina game sheets.

Links quest text to quest IDs. Stores quest metadata in `QuestPlate`. Prefetches accepted quest translations via `AcceptedQuestPrefetchRuntime.cs`.

---

## Font Handling

Bundled fonts in `Font/`:
- `NotoSansCJKjp-Regular.otf` — Japanese
- `NotoSansCJKsc-Regular.otf` — Simplified Chinese
- `NotoSansCJKtc-Regular.otf` — Traditional Chinese
- `NotoSansCJKkr-Regular.otf` — Korean
- `NotoSansJP-VF-*.ttf` — variable font size variants

RTL text (Arabic, Hebrew) pre-rendered to bitmap via GDI+ (`ImageGeneration/TextImageRenderer.cs`) then uploaded as game textures.

---

## What Echoglossian Does NOT Do (Gap for Monowakaru)

- **No word-level lookup** — only full sentences
- **No Yomitan/JMdict format support**
- **No morphological analysis** (no tokenization, no furigana)
- **No kanji breakdown** (radicals, stroke counts, readings)
- **No spaced repetition / vocabulary tracking**
- **No hover-over-specific-word** interaction — hover triggers full-sentence tooltip, not per-word
- **No dictionary import** from external sources

---

## Key Files for Reference

| File | Why Relevant |
|---|---|
| `NativeUI/AddonHandlers/Talk/TalkHandler.cs` | Best example of PreRefresh + PostUpdate pattern for dialogue |
| `NativeUI/Helpers/AddonHandlerWiring.cs` | How to map addon names → handlers |
| `NativeUI/Helpers/HoverTooltipManager.cs` | Hover tooltip system — reusable pattern for word lookup |
| `NativeUI/Helpers/AtkTextNodeBufferWrapper.cs` | UTF-8 unmanaged buffer write-back |
| `NativeUI/Handlers/StringArrayDataHandler.cs` | StringArrayData hook (menus, items, skills) |
| `Translators/ITranslator.cs` | Translator interface |
| `EFCoreSqlite/Models/TalkMessage.cs` | DB model pattern |
| `UIOverlays/TranslationOverlay/TranslationOverlayDrawer.cs` | Overlay display logic |
| `GeneralHelpers/Sanitizer.cs` | Text sanitization before API calls |

---

## Architectural Takeaways for Monowakaru

1. **Use `IAddonLifecycle.PreRefresh`** to capture original Japanese text before game processes it.

2. **ATK node ID lookup** is the right way to extract specific text fields. Need to probe each addon to find correct node IDs (use `/egloaddonprobe` command from Echoglossian as reference).

3. **Hover tooltip pattern** (`HoverTooltipManager`) is close to what Yomitan does — register addon bounds as tooltip region, show lookup on hover. Extend to per-word granularity by measuring glyph positions.

4. **Per-word lookup needs tokenization first** — Echoglossian skips this entirely. Need a Japanese tokenizer (e.g. call local MeCab/Kuromoji endpoint, or use a Wasm port) before dictionary lookup.

5. **Yomitan dictionary format** is ZIP of JSON files. Need a reader/importer separate from Echoglossian's approach.

6. **ImGui overlay window** is the natural display surface for the popup dictionary card (like Yomitan's popup).

7. **EF Core SQLite** pattern from Echoglossian is reusable for caching dictionary lookups and user vocabulary lists.

8. **CJK fonts already figured out** — copy font bundling + `IUiBuilder.GetGameFontHandle` / ImGui font push pattern from Echoglossian's `PluginUI/Helpers/` font setup.

9. **Async off-thread** for any dictionary lookup that hits disk or network — same `Task.Run()` + `SemaphoreSlim` pattern.

10. **`IAddonLifecycle` is preferred over raw function hooks** for text interception in modern Dalamud — Echoglossian confirms this is the stable approach.
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using JetBrains.Annotations;
using Monowakaru.Services;

namespace Monowakaru.Windows;

[UsedImplicitly]
public class TextCaptureWindow : Window
{
    private readonly IKeyState _keyState;
    private readonly IPluginLog _log;
    private readonly TextCaptureService _textCaptureService;
    private IReadOnlyList<TextCapture> _lastCapture;
    private int _lastCount;
    private bool _wasApostropheDown;

    public TextCaptureWindow(TextCaptureService textCaptureService, IPluginLog log, IKeyState keyState)
        : base("Text Capture##monowakaru-debug")
    {
        _textCaptureService = textCaptureService;
        _log = log;
        _keyState = keyState;
        _lastCapture = [];
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        var isApostropheDown = _keyState[VirtualKey.OEM_7];
        if (!isApostropheDown || _wasApostropheDown) return;

        var mousePos = ImGui.GetMousePos();
        _log.Verbose("[TextCapture] Hotkey pressed at ({X}, {Y})", mousePos.X, mousePos.Y);
        _textCaptureService.CaptureNodesAt(mousePos);

        _wasApostropheDown = isApostropheDown;

        _lastCapture = _textCaptureService.CaptureNodesAt(mousePos);

        ImGui.Text($"{_lastCapture.Count} captures at ({mousePos.X}, {mousePos.Y})");
        ImGui.SameLine();
        ImGui.Separator();

        var tableFlags = ImGuiTableFlags.ScrollY
                         | ImGuiTableFlags.RowBg
                         | ImGuiTableFlags.BordersOuter
                         | ImGuiTableFlags.BordersInnerV
                         | ImGuiTableFlags.SizingStretchProp;

        // Leave room for separator + button row above
        var tableSize = new Vector2(0, ImGui.GetContentRegionAvail().Y);

        using var table = ImRaii.Table("captures", 4, tableFlags, tableSize);
        if (!table) return;

        ImGui.TableSetupScrollFreeze(0, 1);
        ImGui.TableSetupColumn("Addon", ImGuiTableColumnFlags.WidthFixed, 110);
        ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthStretch, 1);
        ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 55);
        ImGui.TableHeadersRow();

        for (var i = 0; i < _lastCapture.Count; i++)
        {
            var entry = _lastCapture[i];
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(entry.Addon);

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(entry.Text);

            ImGui.TableSetColumnIndex(2);
            if (ImGui.Button($"Copy##{i}"))
                CopyCaptureIntoClipboard(entry);
        }

        // Auto-scroll to bottom when new entries arrive
        if (_lastCapture.Count <= _lastCount) return;
        ImGui.SetScrollHereY(1.0f);
        _lastCount = _lastCapture.Count;
    }

    private static void CopyCaptureIntoClipboard(TextCapture entry)
    {
        ImGui.SetClipboardText(entry.Text);
    }
}
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
    private readonly IKeyState keyState;
    private readonly IPluginLog log;
    private readonly TextCaptureService textCaptureService;
    private int lastCount;
    private bool wasApostropheDown;

    public TextCaptureWindow(TextCaptureService textCaptureService, IPluginLog log, IKeyState keyState)
        : base("Text Capture##monowakaru-debug")
    {
        this.textCaptureService = textCaptureService;
        this.log = log;
        this.keyState = keyState;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
    }

    public override void Draw()
    {
        var isApostropheDown = keyState[VirtualKey.OEM_7];
        if (isApostropheDown && !wasApostropheDown)
        {
            var pos = ImGui.GetMousePos();
            log.Verbose("[TextCapture] Hotkey pressed at ({X}, {Y})", pos.X, pos.Y);
            textCaptureService.CaptureNodesAt(pos);
        }

        wasApostropheDown = isApostropheDown;

        var captures = textCaptureService.GetCaptures();

        ImGui.Text($"{captures.Length} unique text(s) captured");
        ImGui.SameLine();
        if (ImGui.Button("Clear"))
        {
            textCaptureService.Clear();
            lastCount = 0;
        }

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
        ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.WidthFixed, 65);
        ImGui.TableSetupColumn("Addon", ImGuiTableColumnFlags.WidthFixed, 110);
        ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthStretch, 1);
        ImGui.TableSetupColumn("Copy Text", ImGuiTableColumnFlags.WidthStretch, 1);
        ImGui.TableHeadersRow();

        foreach (var entry in captures)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.TextUnformatted(entry.CapturedAt.ToString("HH:mm:ss"));

            ImGui.TableSetColumnIndex(1);
            ImGui.TextUnformatted(entry.AddonName);

            ImGui.TableSetColumnIndex(2);
            ImGui.TextUnformatted(entry.Text);

            ImGui.TableSetColumnIndex(3);
            if (ImGui.Button($"Copy##{entry.CapturedAt.Ticks}"))
                CopyCaptureIntoClipboard(entry);
        }

        // Auto-scroll to bottom when new entries arrive
        if (captures.Length > lastCount)
        {
            ImGui.SetScrollHereY(1.0f);
            lastCount = captures.Length;
        }
    }

    private static void CopyCaptureIntoClipboard(CapturedText entry)
    {
        ImGui.SetClipboardText(entry.Text);
    }
}
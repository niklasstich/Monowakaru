using System.Collections.Generic;
using System.Numerics;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using JetBrains.Annotations;
using Monowakaru.Mediator;
using Monowakaru.Services;

namespace Monowakaru.Windows;

[UsedImplicitly]
public class TextCaptureWindow : Window, IMediatorSubscriber
{
    private IReadOnlyList<TextCapture> _lastCapture = [];
    private int _lastCount;

    public TextCaptureWindow(MediatorService mediatorService)
        : base("Text Capture##monowakaru-debug")
    {
        MediatorService = mediatorService;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        MediatorService.Subscribe<TextCaptureResultMessage>(this, OnCapture);
    }

    public MediatorService MediatorService { get; }

    private void OnCapture(TextCaptureResultMessage msg)
    {
        _lastCapture = msg.Captures;
        IsOpen = true;
    }

    public override void Draw()
    {
        ImGui.Text($"{_lastCapture.Count} captures");
        ImGui.Separator();

        var tableFlags = ImGuiTableFlags.ScrollY
                         | ImGuiTableFlags.RowBg
                         | ImGuiTableFlags.BordersOuter
                         | ImGuiTableFlags.BordersInnerV
                         | ImGuiTableFlags.SizingStretchProp;

        var tableSize = new Vector2(0, ImGui.GetContentRegionAvail().Y);

        using var table = ImRaii.Table("captures", 3, tableFlags, tableSize);
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
                ImGui.SetClipboardText(entry.Text);
        }

        if (_lastCapture.Count <= _lastCount) return;
        ImGui.SetScrollHereY(1.0f);
        _lastCount = _lastCapture.Count;
    }
}
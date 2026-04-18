using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Monowakaru.Mediator;

namespace Monowakaru.Services;

public class CaptureTextCommand(
    Configuration configuration,
    TextCaptureService textCaptureService,
    MediatorService mediatorService) : IHotkeyCommand
{
    public VirtualKey Key => configuration.CaptureHotkey;

    public void Execute()
    {
        var mousePos = ImGui.GetMousePos();
        var captures = textCaptureService.CaptureNodesAt(mousePos);
        mediatorService.Publish(new TextCaptureResultMessage(captures, mousePos));
    }
}
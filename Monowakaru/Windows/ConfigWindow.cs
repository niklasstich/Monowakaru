using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;

namespace Monowakaru.Windows;

public class ConfigWindow(Configuration config, IKeyState keyState) : Window("Monowakaru Config")
{
    private bool _listeningForHotkey;

    public override void Draw()
    {
        var configOption = config.ConfigOption;
        if (ImGui.Checkbox("Config Option", ref configOption))
            config.ConfigOption = configOption;

        ImGui.Spacing();
        ImGui.Text("Capture Hotkey");
        ImGui.SameLine();

        if (_listeningForHotkey)
        {
            ImGui.TextUnformatted("Press any key... (Escape to cancel)");
            foreach (var key in Enum.GetValues<VirtualKey>())
            {
                if (!keyState.IsVirtualKeyValid(key) || !keyState[key]) continue;
                if (key == VirtualKey.ESCAPE)
                {
                    _listeningForHotkey = false;
                }
                else
                {
                    config.CaptureHotkey = key;
                    _listeningForHotkey = false;
                }

                break;
            }
        }
        else
        {
            ImGui.TextUnformatted(config.CaptureHotkey.ToString());
            ImGui.SameLine();
            if (ImGui.Button("Change"))
                _listeningForHotkey = true;
        }
    }
}

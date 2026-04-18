using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Newtonsoft.Json;

namespace Monowakaru;

public class Configuration : IPluginConfiguration
{
    private bool configOption;
    private VirtualKey captureHotkey = VirtualKey.OEM_7;

    public int Version { get; set; }

    [JsonIgnore]
    public bool IsDirty { get; set; }

    public bool ConfigOption
    {
        get => configOption;
        set
        {
            configOption = value;
            IsDirty = true;
        }
    }

    public VirtualKey CaptureHotkey
    {
        get => captureHotkey;
        set
        {
            captureHotkey = value;
            IsDirty = true;
        }
    }
}
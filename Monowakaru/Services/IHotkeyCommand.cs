using Dalamud.Game.ClientState.Keys;

namespace Monowakaru.Services;

public interface IHotkeyCommand
{
    VirtualKey Key { get; }
    void Execute();
}
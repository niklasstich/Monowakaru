using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.Hosting;

namespace Monowakaru.Services;

public class HotkeyService(IEnumerable<IHotkeyCommand> commands, IKeyState keyState, IUiBuilder uiBuilder)
    : IHostedService
{
    private struct HotkeyState(IHotkeyCommand command)
    {
        public readonly IHotkeyCommand Command = command;
        public bool WasDown;
    }

    private readonly HotkeyState[] _states = commands.Select(c => new HotkeyState(c)).ToArray();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        uiBuilder.Draw += Poll;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        uiBuilder.Draw -= Poll;
        return Task.CompletedTask;
    }

    private void Poll()
    {
        for (var i = 0; i < _states.Length; i++)
        {
            var isDown = keyState[_states[i].Command.Key];
            if (isDown && !_states[i].WasDown)
                _states[i].Command.Execute();
            _states[i].WasDown = isDown;
        }
    }
}
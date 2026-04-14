namespace Monowakaru.Services;

using System.Threading;
using System.Threading.Tasks;

using DalaMock.Host.Mediator;
using Mediator;
using Windows;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.Hosting;

public class CommandService(ICommandManager commandManager, MediatorService mediatorService) : IHostedService
{
    private readonly MediatorService mediatorService = mediatorService;
    private readonly string[] commandName = { "/monowakaru", "/monowakarualias"};

    public ICommandManager CommandManager { get; } = commandManager;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < commandName.Length; i++)
        {
            CommandManager.AddHandler(
            commandName[i],
            new CommandInfo(OnCommand)
            {
                HelpMessage = "Shows the sample plugin main window.",
            });
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < commandName.Length; i++)
        {
            CommandManager.RemoveHandler(commandName[i]);
        }

        return Task.CompletedTask;
    }

    private void OnCommand(string command, string arguments)
    {
        mediatorService.Publish(new ToggleWindowMessage(typeof(MainWindow)));
    }
}

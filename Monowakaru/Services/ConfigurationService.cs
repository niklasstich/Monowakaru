namespace Monowakaru.Services;

using System;
using System.Threading;
using System.Threading.Tasks;

using DalaMock.Host.Hosting;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

public class ConfigurationService : HostingAwareService
{
    private readonly IDalamudPluginInterface pluginInterface;
    private readonly IPluginLog pluginLog;
    private readonly IFramework framework;
    private Configuration? configuration;

    public ConfigurationService(IDalamudPluginInterface pluginInterface, IPluginLog pluginLog, IFramework framework, HostedEvents hostedEvents)
        : base(hostedEvents)
    {
        this.pluginInterface = pluginInterface;
        this.pluginLog = pluginLog;
        this.framework = framework;
        var configPath = pluginInterface.GetPluginConfigDirectory();
    }

    /// <summary>
    /// Get the configuration of the plugin.
    /// </summary>
    /// <returns>The configuration either loaded from the file system or a new instance of the configuration.</returns>
    public Configuration GetConfiguration()
    {
        if (configuration == null)
        {
            try
            {
                configuration = pluginInterface.GetPluginConfig() as Configuration ??
                                     new Configuration();
            }
            catch (Exception e)
            {
                pluginLog.Error(e, "Failed to load configuration");
                configuration = new Configuration();
            }
        }

        return configuration;
    }

    /// <summary>
    /// Request a manual save of the configuration.
    /// </summary>
    public void Save()
    {
        GetConfiguration().IsDirty = false;
        pluginInterface.SavePluginConfig(GetConfiguration());
    }

    /// <summary>
    /// Automatically saves the plugin once it start's stopping and handles autosaving the configuration when the plugin has started or is stopping.
    /// </summary>
    /// <param name="eventType"></param>
    public override void OnPluginEvent(HostedEventType eventType)
    {
        if (eventType == HostedEventType.PluginStopping)
        {
            Save();
        }

        if (eventType == HostedEventType.PluginStarted)
        {
            framework.Update += HandleAutosave;
        }

        if (eventType == HostedEventType.PluginStopping)
        {
            framework.Update -= HandleAutosave;
        }
    }

    private void HandleAutosave(IFramework framework1)
    {
        if (configuration?.IsDirty ?? false)
        {
            pluginLog.Verbose("Configuration is dirty, saving.");
            Save();
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        framework.Update -= HandleAutosave;
        return Task.CompletedTask;
    }
}

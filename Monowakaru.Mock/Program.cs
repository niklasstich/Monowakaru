using DalaMock.Core.DI;

namespace Monowakaru.Mock;

internal static class Program
{
    private static void Main(string[] args)
    {
        var mockContainer = new MockContainer();
        var mockDalamudUi = mockContainer.GetMockUi();
        var pluginLoader = mockContainer.GetPluginLoader();
        var mockPlugin = pluginLoader.AddPlugin(typeof(MockTestPlugin));
        pluginLoader.StartPlugin(mockPlugin);
        mockDalamudUi.Run();
    }
}

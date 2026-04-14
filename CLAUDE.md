# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A [Dalamud](https://github.com/goatcorp/Dalamud) plugin for Final Fantasy XIV. The solution has two projects:

- **Monowakaru** — the actual plugin, built as a class library loaded by Dalamud at runtime.
- **Monowakaru.Mock** — a standalone executable that hosts the plugin outside the game using [DalaMock](https://github.com/Critical-Impact/DalaMock), allowing local UI development without launching FFXIV.

## Build commands

```bash
# Build the plugin
dotnet build Monowakaru/Monowakaru.csproj

# Build the mock runner
dotnet build Monowakaru.Mock/Monowakaru.Mock.csproj

# Run the mock (opens a window simulating the in-game UI)
dotnet run --project Monowakaru.Mock/Monowakaru.Mock.csproj
```

The `Dalamud.NET.Sdk` MSBuild SDK is used in both projects. It resolves Dalamud assemblies from `$(DalamudLibPath)` (set by the SDK, typically `~/.xlcore/dalamud/Dalamud/`).

## Architecture

### DI / hosting

`TestPlugin` (the entry point Dalamud loads) extends `HostedPlugin` from DalaMock.Host. All services and windows are registered in **Autofac** (`ConfigureContainer`) rather than `IServiceCollection`. Services that implement `IHostedService` are started/stopped automatically by the host.

The four services:
- **`WindowService`** — creates the `WindowSystem`, registers all `Window`-derived types (auto-discovered by assembly scan in `TestPlugin`), hooks `UiBuilder.Draw`, and handles open/close/toggle via mediator messages.
- **`InstallerWindowService`** — wires up `UiBuilder.OpenMainUi` / `OpenConfigUi` (the plugin installer buttons) to mediator toggle messages.
- **`ConfigurationService`** — loads/saves `Configuration` via `IDalamudPluginInterface`. Autosaves whenever `Configuration.IsDirty` is true (checked each framework tick).
- **`CommandService`** — registers chat commands `/monowakaru` and `/monowakarualias` that toggle `MainWindow`.

### Mediator

Inter-component communication goes through `MediatorService` (from DalaMock.Host). Messages are records in `Monowakaru/Mediator/Messages.cs` that extend `MessageBase`. To add a new message, add a record there and publish/subscribe via `MediatorService`.

### Windows

All classes whose name ends in `Window` are auto-registered with the Autofac container and added to the `WindowSystem`. To add a new window, create a class inheriting `Dalamud.Interface.Windowing.Window` with a name ending in `Window` — no other registration is needed.

`Configuration` is injected directly into `ConfigWindow` as a constructor parameter; changes to config properties set `IsDirty = true` to trigger autosave.

### Mock project

`MockTestPlugin` extends `TestPlugin` and overrides `ConfigureContainer` to swap in DalaMock implementations of `IWindowSystem`, `IFileDialogManager`, and `IFont`. `Program.cs` bootstraps `MockContainer`, adds the plugin, and runs the mock UI loop.

#### Built-in DalaMock windows

`MockContainer` automatically registers several utility windows that appear alongside the plugin's own windows when the mock runs:

| Window | Purpose |
|---|---|
| `MockWindows` | Buttons to fire `OpenMainUi` / `OpenConfigUi` events (same as the plugin installer buttons in-game) |
| `MockMockWindow` | Tabbed editor for all mock Dalamud services (see below) |
| `MockSettingsWindow` | Configure the game data path and plugin save path |
| `LocalPlayersWindow` / `LocalPlayerEditWindow` | Edit the simulated local player character |

The **MockMockWindow** tabs cover: `MockClientState` (login state, zone, local player), `MockGameGui` (hovered item/action), `MockCondition` (condition flags like `InCombat`), and others.

#### Limitations: native game UI

FFXIV's native addon windows (`AtkUnitBase` — dialog boxes, inventory, map, etc.) **cannot be rendered in DalaMock**. They depend on the game engine's C++ UI framework. The mock is only for the plugin's own ImGui windows.

To test plugin logic that reacts to game UI state, simulate the underlying state through the mock service editors instead — e.g. set condition flags in `MockCondition`, fire `TerritoryChanged` via `MockClientState`, or set `HoveredItem` via `MockGameGui`. Any code path that reads directly from native addon nodes (e.g. `GameGui.GetAddonByName(...)`) requires the live game.
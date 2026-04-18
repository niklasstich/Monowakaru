using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Microsoft.Extensions.Hosting;

namespace Monowakaru.Services;

using AtkValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

public record CapturedText(string AddonName, string Text, DateTime CapturedAt);

/// <summary>
///     Abstraction service for various different text extractors
/// </summary>
public class TextCaptureService : IHostedService, IDisposable
{
    // Addons that carry dialogue / quest / toast text worth capturing
    private static readonly string[] TrackedAddons =
    [
        // Dialogue
        "Talk", // Main NPC dialogue
        "_BattleTalk", // Combat NPC dialogue
        "TalkSubtitle", // Cutscene subtitles
        "_MiniTalk", // Floating dialogue bubbles
        "CutSceneSelectString", // Dialogue choices in cutscenes
        "SelectString", // NPC choice menus
        "SelectYesno", // Yes / No prompts
        "SelectOk", // Single-button confirmations

        // Quest
        "Journal", // Quest journal list
        "JournalDetail", // Quest journal detail view
        "JournalAccept", // Quest accept dialog
        "JournalResult", // Quest completion screen
        "ScenarioTree", // Main story scenario tree
        "RecommendList", // Recommended quests
        "_ToDoList", // Active objectives panel

        // Context menus
        "_MainCommand", // Main hotbar right-click menus (マイキャラクター etc.)
        "ContextMenu", // General right-click context menus

        // Tutorial / hints
        "OperationGuide", // HowTo tutorial popup windows

        // Toasts / notifications
        "_WideText", // Wide toasts (area transitions)
        "_TextError", // Error toasts
        "_AreaText", // Area/location name toasts
        "_TextClassChange", // Job change toasts
        "_TextGimmickHint" // Puzzle/gimmick hints
    ];

    private readonly IAddonLifecycle _addonLifecycle;
    private readonly List<CapturedText> _captures = [];
    private readonly Lock _gate = new();
    private readonly IPluginLog _log;

    private readonly HashSet<string> _seenKeys = [];
    private readonly TextNodeReader _textNodeReader;
    private bool _isDisposed;

    public TextCaptureService(IAddonLifecycle addonLifecycle, IPluginLog log, TextNodeReader textNodeReader)
    {
        _addonLifecycle = addonLifecycle;
        _log = log;
        _textNodeReader = textNodeReader;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var addon in TrackedAddons)
            _addonLifecycle.RegisterListener(AddonEvent.PreRefresh, addon, OnPreRefresh);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Unregister();
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing) Unregister();
        _isDisposed = true;
    }

    private void Unregister()
    {
        foreach (var addon in TrackedAddons)
            _addonLifecycle.UnregisterListener(AddonEvent.PreRefresh, addon, OnPreRefresh);
    }

    /// <summary>Returns a snapshot of all captured texts, safe to iterate on any thread.</summary>
    public CapturedText[] GetCaptures()
    {
        lock (_gate)
        {
            return [.. _captures];
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _seenKeys.Clear();
            _captures.Clear();
        }
    }

    /// <summary>
    ///     Walks the node tree of every tracked addon and captures text nodes whose screen bounds
    ///     contain <paramref name="screenPoint" />. Must be called from the game thread.
    /// </summary>
    public void CaptureNodesAt(Vector2 screenPoint)
    {
        foreach (var addonName in TrackedAddons)
        {
            var allNodes = _textNodeReader.GetTextNodes(addonName);
            if (allNodes.Count == 0) continue;

            _log.Verbose("[TCS] {addonName}: {total} total nodes, checking against {pos}",
                addonName, allNodes.Count, screenPoint);
            foreach (var node in allNodes)
                _log.Verbose("[TCS]   node '{text}' @ ({x},{y}) size ({w},{h})",
                    node.Text, node.ScreenPosition.X, node.ScreenPosition.Y, node.Size.X, node.Size.Y);
            var filteredNodes = allNodes.Where(node => node.Contains(screenPoint));
            foreach (var node in filteredNodes)
            {
                _log.Verbose("[TCS] {addonName}: {text} @ ({x},{y}), cursor contains screenPoint", addonName, node.Text,
                    node.ScreenPosition.X,
                    node.ScreenPosition.Y);
                AddCapture(addonName, node.Text);
            }
        }
    }

    private void AddCapture(string addonName, string text)
    {
        var key = $"{addonName}\0{text}";
        lock (_gate)
        {
            if (!_seenKeys.Add(key))
                return;
            _captures.Add(new CapturedText(addonName, text, DateTime.Now));
        }

        _log.Debug("[TextCapture] [{Addon}] {Text}", addonName, text);
    }

    private unsafe void OnPreRefresh(AddonEvent type, AddonArgs args)
    {
        if (args is not AddonRefreshArgs refreshArgs)
            return;

        var atkValues = (AtkValue*)refreshArgs.AtkValues;
        if (atkValues == null || refreshArgs.AtkValueCount == 0)
            return;

        var count = (int)refreshArgs.AtkValueCount;
        for (var i = 0; i < count; i++)
        {
            var text = ReadAtkString(atkValues[i]);
            if (!string.IsNullOrWhiteSpace(text))
                AddCapture(args.AddonName, text);
        }
    }

    private static unsafe string ReadAtkString(AtkValue atkValue)
    {
        // Check lower 4 bits (TypeMask) to identify string types (8 = String, 10 = String8)
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        // it is in fact a bitwise flag
#pragma warning disable S3265
        var baseType = atkValue.Type & AtkValueType.TypeMask;
#pragma warning restore S3265

        if ((baseType != AtkValueType.String && baseType != AtkValueType.String8) || !atkValue.String.HasValue)
            return string.Empty;

        // ReadSeStringNullTerminated strips SeString control codes; TextValue gives plain text
        return MemoryHelper.ReadSeStringNullTerminated((nint)atkValue.String.Value).TextValue;
    }
}
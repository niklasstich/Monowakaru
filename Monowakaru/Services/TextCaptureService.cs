using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin.Services;
using Serilog.Events;

namespace Monowakaru.Services;

/// <summary>
///     Queries visible text nodes across all tracked addons.
/// </summary>
public class TextCaptureService(IPluginLog log, TextNodeReader textNodeReader)
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

    /// <summary>
    ///     Walks the node tree of every tracked addon and returns text nodes whose screen bounds
    ///     contain <paramref name="screenPoint" />. Must be called from the game thread.
    /// </summary>
    public IReadOnlyList<TextCapture> CaptureNodesAt(Vector2 screenPoint)
    {
        var hits = TrackedAddons
            .SelectMany(CaptureNodes)
            .Where(node => node.Contains(screenPoint))
            .ToList();

        if (log.MinimumLogLevel != LogEventLevel.Verbose) return hits;

        foreach (var node in hits)
            log.Verbose("[TCS] {addonName}: {text} under cursor ({x},{y})", node.Addon, node.Text, screenPoint.X,
                screenPoint.Y);
        return hits;
    }

    /// <summary>
    ///     Walks the node tree of every tracked addon and returns all text nodes found. Must be called from the game thread.
    /// </summary>
    /// <returns>A dictionary from AddonName to a List of text nodes.</returns>
    public IReadOnlyDictionary<string, IReadOnlyList<TextCapture>> CaptureAllNodes()
    {
        return TrackedAddons.ToDictionary(addonName => addonName, CaptureNodes);
    }

    private IReadOnlyList<TextCapture> CaptureNodes(string addonName)
    {
        var allNodes = textNodeReader
            .GetTextNodes(addonName)
            .Select(n => new TextCapture(addonName, n.Text, n.ScreenPosition, n.Size))
            .ToList();
        if (allNodes.Count == 0) return [];

        log.Debug("[TCS] {addonName}: {total} total nodes",
            addonName, allNodes.Count);
        if (log.MinimumLogLevel != LogEventLevel.Verbose) return allNodes;
        foreach (var node in allNodes)
            log.Verbose("[TCS]   node '{text}' @ ({x},{y}) size ({w},{h})",
                node.Text, node.ScreenPosition.X, node.ScreenPosition.Y, node.Size.X, node.Size.Y);
        return allNodes;
    }
}

public record TextCapture(string Addon, string Text, Vector2 ScreenPosition, Vector2 Size)
{
    public bool Contains(Vector2 point)
    {
        return point.X >= ScreenPosition.X && point.X <= ScreenPosition.X + Size.X &&
               point.Y >= ScreenPosition.Y && point.Y <= ScreenPosition.Y + Size.Y;
    }
}
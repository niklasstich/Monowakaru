using System.Collections.Generic;
using System.Numerics;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Monowakaru.Services;

/// <summary>On-demand snapshot of a visible ATK text node.</summary>
/// <param name="Text">Plain text content, SeString codes stripped.</param>
/// <param name="ScreenPosition">Top-left corner in screen pixels.</param>
/// <param name="Size">Width × height in screen pixels (scale applied).</param>
public readonly record struct AddonTextNode(string Text, Vector2 ScreenPosition, Vector2 Size);

/// <summary>
///     Reads text nodes directly from the ATK node tree of a visible game addon.
///     Covers both dynamic text (set via AtkValues) and static text (button labels, ULD defaults).
///     Must be called from the game thread.
/// </summary>
public class TextNodeReader(IGameGui gameGui, IPluginLog log)
{
    /// <summary>Returns all visible text nodes in the named addon, or empty if not open.</summary>
    public unsafe IReadOnlyList<AddonTextNode> GetTextNodes(string addonName)
    {
        var addon = gameGui.GetAddonByName<AtkUnitBase>(addonName);
        log.Verbose("[TNR] {addon}: ptr={ptr} isVisible={vis}",
            addonName, (nint)addon, addon != null && addon->IsVisible);
        if (addon == null || !addon->IsVisible)
            return [];

        var results = new List<AddonTextNode>();
        CollectFromUldManager(&addon->UldManager, results);

        log.Verbose("[TNR] {addon}: nodeListCount={count} found {results} text nodes",
            addonName, addon->UldManager.NodeListCount, results.Count);
        return results;
    }

    private static unsafe void CollectFromUldManager(AtkUldManager* mgr, List<AddonTextNode> results)
    {
        foreach (var nodePtr in mgr->Nodes)
        {
            var node = nodePtr.Value;
            if (node == null) continue;

            HandleAtkResNode(results, node);
        }
    }

    private static unsafe void HandleAtkResNode(List<AddonTextNode> results, AtkResNode* node)
    {
        const int componentTypeId = 1000;
        // we can extract the text from text nodes directly
        if (node->Type == NodeType.Text)
        {
            var textNode = (AtkTextNode*)node;
            if (!textNode->IsVisible()) return;
            if (!textNode->NodeText.StringPtr.HasValue) return;

            var text = MemoryHelper
                .ReadSeStringNullTerminated((nint)textNode->NodeText.StringPtr.Value)
                .TextValue;
            if (string.IsNullOrWhiteSpace(text)) return;

            ushort drawW = 0, drawH = 0;
            textNode->GetTextDrawSize(&drawW, &drawH, textNode->NodeText.StringPtr);

            results.Add(new AddonTextNode(
                text,
                new Vector2(node->ScreenX, node->ScreenY),
                new Vector2(drawW * node->ScaleX, drawH * node->ScaleY)));
        }
        // if the node is a component, we need to recursively traverse it to search for nodes
        else if ((ushort)node->Type >= componentTypeId)
        {
            var compNode = (AtkComponentNode*)node;
            if (compNode->Component != null)
                CollectFromUldManager(&compNode->Component->UldManager, results);
        }
    }
}
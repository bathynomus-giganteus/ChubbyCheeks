using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Vfx;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class CardInspectApostlePreviewPatch
{
    private const string PreviewNodeName = "CultLeaderInspectApostlePreview";
    private const string PreviewGroupName = "CultLeaderInspectApostlePreviews";
    private const string PreviewProfileMetaKey = "CultLeaderPreviewProfileKey";
    private const string PreviewAliveMetaKey = "CultLeaderPreviewAlive";
    private static readonly Dictionary<string, Texture2D[]> PreviewFrameCache = [];
    private static readonly System.Reflection.FieldInfo? CardField =
        AccessTools.Field(typeof(NInspectCardScreen), "_card");

    [HarmonyPatch(typeof(NInspectCardScreen), "UpdateCardDisplay")]
    [HarmonyPostfix]
    private static void AfterUpdateCardDisplay(NInspectCardScreen __instance)
    {
        try
        {
            var card = CardField?.GetValue(__instance) as NCard;
            var model = card?.Model;
            if (model != null
                && IsCultLeaderCardType(model.GetType())
                && CultLeaderAnimationSettings.Allows(model))
            {
                if (ApostleSpinePrototype.IsPrototypeCard(model.GetType()))
                {
                    RemoveFramePreviewsOnly();
                    ApostleSpinePrototype.TryEnsurePreview(__instance, model.GetType());
                    return;
                }

                if (ApostleAnimationProfiles.TryGetPreviewProfile(model.GetType(), out var profile))
                {
                    ApostleSpinePrototype.RemoveAllPreviews();
                    EnsurePreview(__instance, profile);
                }
                else
                    RemovePreview(__instance);
            }
            else
                RemovePreview(__instance);
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CardInspectApostlePreviewPatch] Update failed: {ex}");
        }
    }

    [HarmonyPatch(typeof(NInspectCardScreen), nameof(NInspectCardScreen.Close))]
    [HarmonyPrefix]
    private static void BeforeClose(NInspectCardScreen __instance)
    {
        RemovePreview(__instance);
    }

    private static void EnsurePreview(NInspectCardScreen screen, PreviewProfile profile)
    {
        var existing = FindAndRemoveStalePreviews(screen, profile);
        if (existing != null && GodotObject.IsInstanceValid(existing))
        {
            existing.Size = profile.FrameSize;
            existing.PivotOffset = profile.FrameSize / 2f;
            PositionPreview(screen, existing, profile);
            existing.Visible = true;
            return;
        }

        var preview = new TextureRect
        {
            Name = PreviewNodeName,
            Size = profile.FrameSize,
            PivotOffset = profile.FrameSize / 2f,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            ZIndex = 100,
        };
        preview.SetMeta(PreviewProfileMetaKey, profile.Key);
        preview.SetMeta(PreviewAliveMetaKey, true);
        preview.AddToGroup(PreviewGroupName);

        PositionPreview(screen, preview, profile);
        screen.AddChild(preview);
        _ = AnimatePreviewAsync(preview, profile);
    }

    private static bool IsCultLeaderCardType(Type cardType) =>
        cardType.Namespace == "CultLeaderMod.CultLeaderModCode.Cards";

    private static void PositionPreview(NInspectCardScreen screen, TextureRect preview, PreviewProfile profile)
    {
        var viewportSize = screen.GetViewportRect().Size;
        if (viewportSize.X <= 0f || viewportSize.Y <= 0f)
            viewportSize = new Vector2(1920f, 1080f);

        var size = profile.FrameSize;
        preview.Position = new Vector2(
            Math.Clamp(viewportSize.X * 0.16f - size.X / 2f + profile.PositionOffset.X, 40f, Math.Max(40f, viewportSize.X - size.X - 40f)),
            Math.Clamp(viewportSize.Y * 0.38f - size.Y / 2f + profile.PositionOffset.Y, 60f, Math.Max(60f, viewportSize.Y - size.Y - 60f))
        );
    }

    private static void RemovePreview(NInspectCardScreen screen)
    {
        RemoveAllPreviews();
    }

    private static TextureRect? FindAndRemoveStalePreviews(NInspectCardScreen currentScreen, PreviewProfile currentProfile)
    {
        if (Engine.GetMainLoop() is not SceneTree tree)
            return null;

        TextureRect? keep = null;
        foreach (var preview in FindAllPreviewNodes(tree.Root))
        {
            if (!GodotObject.IsInstanceValid(preview))
                continue;

            var profileKey = preview.GetMeta(PreviewProfileMetaKey, string.Empty).AsString();
            var isAlive = preview.GetMeta(PreviewAliveMetaKey, true).AsBool();
            var isCurrent = preview.GetParent() == currentScreen && profileKey == currentProfile.Key && isAlive;

            if (isCurrent && keep == null)
            {
                keep = preview;
                continue;
            }

            MarkPreviewForRemoval(preview);
        }

        return keep;
    }

    private static void RemoveAllPreviews()
    {
        ApostleSpinePrototype.RemoveAllPreviews();
        RemoveFramePreviewsOnly();
    }

    private static void RemoveFramePreviewsOnly()
    {
        if (Engine.GetMainLoop() is not SceneTree tree)
            return;

        foreach (var preview in FindAllPreviewNodes(tree.Root))
            MarkPreviewForRemoval(preview);
    }

    private static IEnumerable<TextureRect> FindAllPreviewNodes(Node root)
    {
        var previews = new List<TextureRect>();

        FindNamedPreviewsRecursive(root, previews);

        if (Engine.GetMainLoop() is SceneTree tree)
        {
            foreach (var node in tree.GetNodesInGroup(PreviewGroupName))
            {
                if (node is TextureRect preview && !previews.Contains(preview))
                    previews.Add(preview);
            }
        }

        return previews;
    }

    private static void FindNamedPreviewsRecursive(Node node, List<TextureRect> previews)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is TextureRect preview
                && preview.Name == PreviewNodeName)
            {
                previews.Add(preview);
                continue;
            }

            FindNamedPreviewsRecursive(child, previews);
        }
    }

    private static void MarkPreviewForRemoval(TextureRect preview)
    {
        if (!GodotObject.IsInstanceValid(preview))
            return;

        preview.SetMeta(PreviewAliveMetaKey, false);
        preview.Visible = false;
        preview.Texture = null;
        preview.RemoveFromGroup(PreviewGroupName);
        preview.QueueFree();
    }

    private static async Task AnimatePreviewAsync(TextureRect preview, PreviewProfile profile)
    {
        try
        {
            if (Engine.GetMainLoop() is not SceneTree tree)
                return;

            var frames = GetPreviewFrames(profile);
            if (frames.Length == 0)
                return;

            var frame = 0;
            while (
                GodotObject.IsInstanceValid(preview)
                && preview.IsInsideTree()
                && preview.GetMeta(PreviewAliveMetaKey, true).AsBool()
            )
            {
                preview.Texture = frames[frame % frames.Length];
                frame++;
                await preview.ToSignal(tree.CreateTimer(profile.FrameSeconds), SceneTreeTimer.SignalName.Timeout);
            }
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CardInspectApostlePreviewPatch] Preview animation failed: {ex}");
        }
    }

    private static Texture2D[] GetPreviewFrames(PreviewProfile profile)
    {
        if (PreviewFrameCache.TryGetValue(profile.Key, out var cachedFrames))
            return cachedFrames;

        var frames = new List<Texture2D>(profile.FrameCount);
        for (var i = 0; i < profile.FrameCount; i++)
        {
            var texture = ExternalVfxTextureLoader.LoadFrame(
                profile.ExternalFrameDirectory,
                profile.FramePathFormat,
                i
            );
            if (texture != null)
                frames.Add(texture);
        }

        var loadedFrames = frames.ToArray();
        PreviewFrameCache[profile.Key] = loadedFrames;
        return loadedFrames;
    }
}

using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System.Threading;

namespace CultLeaderMod.CultLeaderModCode.Vfx;

public static class ApostleVfxPlayer
{
    private const int BaseVfxLayer = 80;
    private const int VfxLayerCycle = 1000;
    private const float FrameFadeOutSeconds = 0.5f;
    private static readonly Vector2 PlayerLowerCenterAnchor = new(0.30f, 0.42f);
    private static readonly Vector2 PlayerBesideJitterRange = new(90f, 55f);
    private static readonly Dictionary<string, Texture2D[]> BattleFrameCache = [];
    private static int _vfxSequence;

    public static void PlayMagicStrikeBesidePlayer()
    {
        PlayForCard(typeof(Cards.Apostle_Pure_01));
    }

    public static void PlayVelaGhostBesidePlayer()
    {
        PlayForCard(typeof(Cards.Apostle_Lively_12));
    }

    public static void PlayForCard(Cards.CultLeaderModCard card)
    {
        if (!CultLeaderAnimationSettings.Allows(card))
            return;

        PlayForCard(card.GetType());
    }

    public static void PlayForCard(Type cardType)
    {
        PlayForCard(cardType, null);
    }

    public static void PlayForCard(Type cardType, Creature? target)
    {
        if (ApostleSpinePrototype.IsPrototypeCard(cardType))
        {
            ApostleSpinePrototype.TryPlayBattle(cardType, target);
            return;
        }

        if (!ApostleAnimationProfiles.TryGetBattleProfile(cardType, out var profile))
            return;

        _ = PlayFrameVfxAsync(
            $"CultLeader{profile.Key}VfxLayer",
            $"CultLeader{profile.Key}Vfx",
            profile.FramePathFormat,
            profile.FrameCount,
            profile.FrameSeconds,
            profile.FrameSize,
            PlayerLowerCenterAnchor,
            profile.Scale,
            PlayerBesideJitterRange,
            profile.ExternalFrameDirectory
        );
    }

    private static async Task PlayFrameVfxAsync(
        string layerName,
        string spriteName,
        string framePathFormat,
        int frameCount,
        float frameSeconds,
        Vector2 frameSize,
        Vector2 viewportAnchor,
        Vector2 scale,
        Vector2 randomJitterRange,
        string? externalFrameDirectory = null
    )
    {
        try
        {
            if (Engine.GetMainLoop() is not SceneTree tree || tree.Root == null)
                return;

            var root = tree.Root;
            var viewportSize = root.GetVisibleRect().Size;
            var sequence = Interlocked.Increment(ref _vfxSequence);
            var layer = new CanvasLayer
            {
                Name = $"{layerName}_{sequence}",
                Layer = BaseVfxLayer + sequence % VfxLayerCycle,
            };

            var sprite = new TextureRect
            {
                Name = spriteName,
                Size = frameSize,
                PivotOffset = frameSize / 2f,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = Control.MouseFilterEnum.Ignore,
            };

            var jitter = new Vector2(
                (float)((Random.Shared.NextDouble() * 2d - 1d) * randomJitterRange.X),
                (float)((Random.Shared.NextDouble() * 2d - 1d) * randomJitterRange.Y)
            );
            var anchoredPosition = new Vector2(
                viewportSize.X * viewportAnchor.X + jitter.X,
                viewportSize.Y * viewportAnchor.Y + jitter.Y
            );

            sprite.Position = new Vector2(
                Math.Clamp(anchoredPosition.X, 120f, Math.Max(120f, viewportSize.X - frameSize.X - 80f)),
                Math.Clamp(anchoredPosition.Y, 80f, Math.Max(80f, viewportSize.Y - frameSize.Y - 80f))
            );
            sprite.Scale = scale;

            layer.AddChild(sprite);
            root.AddChild(layer);

            var frames = GetBattleFrames(
                spriteName,
                framePathFormat,
                frameCount,
                externalFrameDirectory
            );
            if (frames.Length == 0)
            {
                layer.QueueFree();
                return;
            }

            for (int frame = 0; frame < frameCount; frame++)
            {
                if (!GodotObject.IsInstanceValid(layer) || !GodotObject.IsInstanceValid(sprite))
                    return;

                sprite.Texture = frames[frame % frames.Length];
                sprite.Modulate = new Color(
                    sprite.Modulate.R,
                    sprite.Modulate.G,
                    sprite.Modulate.B,
                    GetFadeAlpha(frame, frameCount, frameSeconds)
                );

                await layer.ToSignal(tree.CreateTimer(frameSeconds), SceneTreeTimer.SignalName.Timeout);
            }

            if (GodotObject.IsInstanceValid(layer))
                layer.QueueFree();
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[ApostleVfxPlayer] Failed to play {spriteName} VFX: {ex}");
        }
    }

    private static Texture2D[] GetBattleFrames(
        string cacheKey,
        string framePathFormat,
        int frameCount,
        string? externalFrameDirectory
    )
    {
        if (BattleFrameCache.TryGetValue(cacheKey, out var cachedFrames))
            return cachedFrames;

        var frames = new List<Texture2D>(frameCount);
        for (var i = 0; i < frameCount; i++)
        {
            var texture = ExternalVfxTextureLoader.LoadFrame(
                externalFrameDirectory,
                framePathFormat,
                i
            );
            if (texture != null)
                frames.Add(texture);
        }

        var loadedFrames = frames.ToArray();
        BattleFrameCache[cacheKey] = loadedFrames;
        return loadedFrames;
    }

    private static float GetFadeAlpha(int frame, int frameCount, float frameSeconds)
    {
        var remainingSeconds = Math.Max(0f, (frameCount - frame) * frameSeconds);
        if (remainingSeconds >= FrameFadeOutSeconds)
            return 1f;

        return Math.Clamp(remainingSeconds / FrameFadeOutSeconds, 0f, 1f);
    }
}

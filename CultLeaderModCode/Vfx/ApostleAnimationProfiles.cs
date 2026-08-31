using Godot;
using System.Text.Json;

namespace CultLeaderMod.CultLeaderModCode.Vfx;

public static class ApostleAnimationProfiles
{
    public static readonly Vector2 DefaultBattleFrameSize = new(420f, 420f);
    public static readonly Vector2 DefaultBattleScale = new(1.10f, 1.10f);
    public static readonly Vector2 DefaultPreviewFrameSize = new(300f, 300f);
    public static readonly Vector2 DefaultPreviewPositionOffset = Vector2.Zero;
    public static readonly Vector2 MagicStrikeBattleScale = new(1.15f, 1.15f);
    public const float DefaultBattleFrameSeconds = 1f / 30f;
    public const float MagicStrikeFrameSeconds = 1f / 24f;
    public const float DefaultPreviewFrameSeconds = 1f / 30f;
    public const float MagicStrikePreviewFrameSeconds = 1f / 15f;

    private const string ManifestFileName = "external_vfx_manifest.json";

    public static readonly Dictionary<Type, BattleVfxProfile> BattleProfiles = [];
    public static readonly Dictionary<Type, PreviewProfile> PreviewProfiles = [];

    static ApostleAnimationProfiles()
    {
        LoadManifestProfiles();
    }

    public static bool TryGetBattleProfile(Type cardType, out BattleVfxProfile profile) =>
        TryGetProfile(cardType, BattleProfiles, out profile);

    public static bool TryGetPreviewProfile(Type cardType, out PreviewProfile profile) =>
        TryGetProfile(cardType, PreviewProfiles, out profile);

    private static bool TryGetProfile<TProfile>(
        Type cardType,
        Dictionary<Type, TProfile> profiles,
        out TProfile profile
    )
    {
        if (profiles.TryGetValue(cardType, out profile!))
            return true;

        foreach (var (registeredType, registeredProfile) in profiles)
        {
            if (registeredType.IsAssignableFrom(cardType))
            {
                profile = registeredProfile;
                return true;
            }
        }

        profile = default!;
        return false;
    }

    private static void LoadManifestProfiles()
    {
        var manifestPath = ResolveManifestPath();
        if (manifestPath == null)
        {
            Entry.Logger.Warn($"[ApostleAnimationProfiles] {ManifestFileName} was not found; apostle animations are disabled.");
            return;
        }

        try
        {
            var json = File.ReadAllText(manifestPath);
            using var document = JsonDocument.Parse(json);
            foreach (var entry in document.RootElement.EnumerateArray())
                RegisterManifestEntry(entry);

            Entry.Logger.Info(
                $"[ApostleAnimationProfiles] Loaded {BattleProfiles.Count} battle profiles and {PreviewProfiles.Count} preview profiles from {manifestPath}"
            );
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[ApostleAnimationProfiles] Failed to load {manifestPath}: {ex}");
        }
    }

    private static void RegisterManifestEntry(JsonElement entry)
    {
        var key = entry.GetProperty("key").GetString();
        if (string.IsNullOrWhiteSpace(key))
            return;

        var attackCount = GetInt(entry, "attack_count");
        var previewCount = GetInt(entry, "preview_count");
        var classNames = entry.TryGetProperty("classes", out var classesElement)
            ? classesElement.EnumerateArray()
                .Select(c => c.GetString())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList()
            : [];

        foreach (var className in classNames)
        {
            var cardType = ResolveCardType(className!);
            if (cardType == null)
            {
                Entry.Logger.Warn($"[ApostleAnimationProfiles] Card type not found for {className}; VFX profile {key} skipped.");
                continue;
            }

            if (attackCount > 0)
            {
                BattleProfiles[cardType] = Battle(
                    $"{key}_attack",
                    $"res://CultLeaderMod/images/vfx/_external_runtime/{key}_attack/frame_{{0:000}}.png",
                    attackCount,
                    BattleFrameSecondsFor(key),
                    BattleScaleFor(key),
                    externalFrameDirectory: $"external_vfx/{key}_attack"
                );
            }

            if (previewCount > 0)
            {
                PreviewProfiles[cardType] = Preview(
                    $"{key}_preview",
                    $"res://CultLeaderMod/images/vfx/_external_runtime/{key}_preview/frame_{{0:000}}.png",
                    previewCount,
                    PreviewFrameSecondsFor(key),
                    PreviewSizeFor(key),
                    PreviewPositionOffsetFor(key),
                    externalFrameDirectory: $"external_vfx/{key}_preview"
                );
            }
        }
    }

    private static string? ResolveManifestPath()
    {
        foreach (var directory in GetModDirectoryCandidates())
        {
            try
            {
                var path = Path.Combine(directory, ManifestFileName);
                if (File.Exists(path))
                    return path;
            }
            catch
            {
                // Keep trying the next candidate.
            }
        }

        return null;
    }

    private static IEnumerable<string> GetModDirectoryCandidates()
    {
        var assemblyDirectory = Path.GetDirectoryName(typeof(ApostleAnimationProfiles).Assembly.Location)
            ?? AppContext.BaseDirectory;
        var currentDirectory = System.Environment.CurrentDirectory;

        yield return assemblyDirectory;
        yield return Path.Combine(AppContext.BaseDirectory, "mods", "CultLeaderMod");
        yield return Path.Combine(currentDirectory, "mods", "CultLeaderMod");
        yield return currentDirectory;
    }

    private static Type? ResolveCardType(string className)
    {
        var assembly = typeof(ApostleAnimationProfiles).Assembly;
        return assembly.GetType($"CultLeaderMod.CultLeaderModCode.Cards.{className}")
            ?? assembly.GetTypes().FirstOrDefault(t => t.Name == className);
    }

    private static int GetInt(JsonElement entry, string propertyName)
    {
        return entry.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.Number
            ? property.GetInt32()
            : 0;
    }

    private static float BattleFrameSecondsFor(string key) =>
        key switch
        {
            "pure_01" => MagicStrikeFrameSeconds,
            "melancholy_26" => 1f / 40f,
            _ => DefaultBattleFrameSeconds,
        };

    private static float PreviewFrameSecondsFor(string key) =>
        key == "pure_01" ? MagicStrikePreviewFrameSeconds : DefaultPreviewFrameSeconds;

    private static Vector2 PreviewSizeFor(string key) =>
        key switch
        {
            "lively_13" => new Vector2(315f, 315f),
            "calm_23" or "melancholy_10" or "melancholy_25" or "melancholy_26" => new Vector2(280f, 280f),
            _ => DefaultPreviewFrameSize,
        };

    private static Vector2 PreviewPositionOffsetFor(string key) =>
        key switch
        {
            "calm_23" or "melancholy_10" or "melancholy_25" or "melancholy_26" => new Vector2(-28f, -24f),
            _ => DefaultPreviewPositionOffset,
        };

    private static Vector2 BattleScaleFor(string key) =>
        key switch
        {
            "pure_01" => MagicStrikeBattleScale,
            "melancholy_26" => new Vector2(0.95f, 0.95f),
            _ => DefaultBattleScale,
        };

    private static BattleVfxProfile Battle(
        string key,
        string framePathFormat,
        int frameCount,
        float frameSeconds = DefaultBattleFrameSeconds,
        Vector2? scale = null,
        string? externalFrameDirectory = null
    ) =>
        new(
            key,
            framePathFormat,
            frameCount,
            frameSeconds,
            DefaultBattleFrameSize,
            scale ?? DefaultBattleScale,
            externalFrameDirectory
        );

    private static PreviewProfile Preview(
        string key,
        string framePathFormat,
        int frameCount,
        float frameSeconds = DefaultPreviewFrameSeconds,
        Vector2? frameSize = null,
        Vector2? positionOffset = null,
        string? externalFrameDirectory = null
    ) =>
        new(
            key,
            framePathFormat,
            frameCount,
            frameSeconds,
            frameSize ?? DefaultPreviewFrameSize,
            positionOffset ?? DefaultPreviewPositionOffset,
            externalFrameDirectory
        );
}

public sealed record BattleVfxProfile(
    string Key,
    string FramePathFormat,
    int FrameCount,
    float FrameSeconds,
    Vector2 FrameSize,
    Vector2 Scale,
    string? ExternalFrameDirectory = null
);

public sealed record PreviewProfile(
    string Key,
    string FramePathFormat,
    int FrameCount,
    float FrameSeconds,
    Vector2 FrameSize,
    Vector2 PositionOffset,
    string? ExternalFrameDirectory = null
);

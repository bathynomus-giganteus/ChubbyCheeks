using Godot;

namespace CultLeaderMod.CultLeaderModCode.Vfx;

public static class ExternalVfxTextureLoader
{
    private static readonly Dictionary<string, Texture2D?> TextureCache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> LoggedExternalDirectories = new(StringComparer.OrdinalIgnoreCase);

    public static Texture2D? LoadFrame(string? externalFrameDirectory, string resourcePathFormat, int frame)
    {
        if (!string.IsNullOrWhiteSpace(externalFrameDirectory))
        {
            var externalPath = ResolveExternalFramePath(externalFrameDirectory, frame);
            if (externalPath != null)
            {
                var texture = LoadExternalTexture(externalPath);
                if (texture != null)
                    return texture;
            }
        }

        return GD.Load<Texture2D>(string.Format(resourcePathFormat, frame));
    }

    private static string? ResolveExternalFramePath(string externalFrameDirectory, int frame)
    {
        var frameNames = new[]
        {
            $"frame_{frame:000}.png",
            $"frame_{frame}.png",
        };

        foreach (var directory in GetExternalDirectoryCandidates(externalFrameDirectory))
        {
            try
            {
                foreach (var frameName in frameNames)
                {
                    var path = Path.Combine(directory, frameName);
                    if (File.Exists(path))
                    {
                        if (LoggedExternalDirectories.Add(directory))
                            Entry.Logger.Info($"[ExternalVfxTextureLoader] Loading external VFX frames from {directory}");

                        return path;
                    }
                }
            }
            catch (Exception ex)
            {
                Entry.Logger.Warn($"[ExternalVfxTextureLoader] Failed to probe external VFX path {directory}: {ex.Message}");
            }
        }

        return null;
    }

    private static IEnumerable<string> GetExternalDirectoryCandidates(string externalFrameDirectory)
    {
        var normalized = externalFrameDirectory
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);

        var assemblyDirectory = Path.GetDirectoryName(typeof(ExternalVfxTextureLoader).Assembly.Location)
            ?? AppContext.BaseDirectory;
        var currentDirectory = System.Environment.CurrentDirectory;

        yield return Path.Combine(assemblyDirectory, normalized);
        yield return Path.Combine(AppContext.BaseDirectory, "mods", "CultLeaderMod", normalized);
        yield return Path.Combine(currentDirectory, "mods", "CultLeaderMod", normalized);
        yield return Path.Combine(currentDirectory, normalized);
    }

    private static Texture2D? LoadExternalTexture(string externalPath)
    {
        if (TextureCache.TryGetValue(externalPath, out var cached))
            return cached;

        try
        {
            using var image = new Image();
            var error = image.Load(externalPath);
            if (error != Error.Ok)
            {
                Entry.Logger.Warn($"[ExternalVfxTextureLoader] Failed to load external image {externalPath}: {error}");
                TextureCache[externalPath] = null;
                return null;
            }

            var texture = ImageTexture.CreateFromImage(image);
            TextureCache[externalPath] = texture;
            return texture;
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[ExternalVfxTextureLoader] Failed to create external texture {externalPath}: {ex}");
            TextureCache[externalPath] = null;
            return null;
        }
    }
}

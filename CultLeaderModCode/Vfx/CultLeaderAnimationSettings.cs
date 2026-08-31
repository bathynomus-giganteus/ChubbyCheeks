using System.Text.Json;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Vfx;

public enum CardAnimationMode
{
    Off = 0,
    RareOnly = 1,
    Full = 2,
}

public static class CultLeaderAnimationSettings
{
    private const string SettingsFileName = "settings.json";
    private static readonly object Sync = new();
    private static bool _loaded;
    private static CardAnimationMode _mode = CardAnimationMode.Full;

    public static CardAnimationMode Mode
    {
        get
        {
            EnsureLoaded();
            return _mode;
        }
        set
        {
            EnsureLoaded();
            _mode = Normalize(value);
        }
    }

    public static bool Allows(CardModel card)
    {
        var mode = Mode;
        return mode switch
        {
            CardAnimationMode.Off => false,
            CardAnimationMode.RareOnly => card.Rarity is CardRarity.Rare or CardRarity.Ancient,
            _ => true,
        };
    }

    public static void Save()
    {
        lock (Sync)
        {
            try
            {
                Directory.CreateDirectory(GetSettingsDirectory());
                var json = JsonSerializer.Serialize(
                    new SettingsDto { CardAnimationMode = _mode.ToString() },
                    new JsonSerializerOptions { WriteIndented = true }
                );
                File.WriteAllText(GetSettingsPath(), json);
            }
            catch (Exception ex)
            {
                Entry.Logger.Warn($"[CultLeaderAnimationSettings] Failed to save settings: {ex}");
            }
        }
    }

    private static void EnsureLoaded()
    {
        if (_loaded)
            return;

        lock (Sync)
        {
            if (_loaded)
                return;

            _mode = LoadMode();
            _loaded = true;
        }
    }

    private static CardAnimationMode LoadMode()
    {
        try
        {
            var path = GetSettingsPath();
            if (!File.Exists(path))
                return CardAnimationMode.Full;

            var dto = JsonSerializer.Deserialize<SettingsDto>(File.ReadAllText(path));
            if (dto?.CardAnimationMode != null
                && Enum.TryParse<CardAnimationMode>(dto.CardAnimationMode, ignoreCase: true, out var parsed))
            {
                return Normalize(parsed);
            }
        }
        catch (Exception ex)
        {
            Entry.Logger.Warn($"[CultLeaderAnimationSettings] Failed to load settings: {ex}");
        }

        return CardAnimationMode.Full;
    }

    private static CardAnimationMode Normalize(CardAnimationMode value) =>
        Enum.IsDefined(value) ? value : CardAnimationMode.Full;

    private static string GetSettingsPath() => Path.Combine(GetSettingsDirectory(), SettingsFileName);

    private static string GetSettingsDirectory()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return string.IsNullOrWhiteSpace(appData)
            ? Path.Combine(Environment.CurrentDirectory, "CultLeaderMod")
            : Path.Combine(appData, "CultLeaderMod");
    }

    private sealed class SettingsDto
    {
        public string? CardAnimationMode { get; init; }
    }
}

using Godot;

namespace CultLeaderMod.CultLeaderModCode.Extensions;

public static class StringExtensions
{
    public static string ImagePath(this string path) => Path.Join(MainFile.ResPath, "images", path).Replace("\\", "/");
    public static string CardImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "card_portraits", path).Replace("\\", "/");
        if (ResourceLoader.Exists(path)) return path;
        return Path.Join(MainFile.ResPath, "images", "card_portraits", "card.png").Replace("\\", "/");
    }
    public static string BigCardImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "card_portraits", "big", path).Replace("\\", "/");
        if (ResourceLoader.Exists(path)) return path;
        return Path.Join(MainFile.ResPath, "images", "card_portraits", "big", "card.png").Replace("\\", "/");
    }
    public static string PowerImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "powers", path).Replace("\\", "/");
        if (ResourceLoader.Exists(path)) return path;
        return Path.Join(MainFile.ResPath, "images", "powers", "power.png").Replace("\\", "/");
    }
    public static string BigPowerImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "powers", "big", path).Replace("\\", "/");
        if (ResourceLoader.Exists(path)) return path;
        return Path.Join(MainFile.ResPath, "images", "powers", "big", "power.png").Replace("\\", "/");
    }
    public static string RelicImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "relics", path).Replace("\\", "/");
        if (ResourceLoader.Exists(path)) return path;
        return Path.Join(MainFile.ResPath, "images", "relics", "relic.png").Replace("\\", "/");
    }
    public static string BigRelicImagePath(this string path)
    {
        path = Path.Join(MainFile.ResPath, "images", "relics", "big", path).Replace("\\", "/");
        if (ResourceLoader.Exists(path)) return path;
        return Path.Join(MainFile.ResPath, "images", "relics", "big", "relic.png").Replace("\\", "/");
    }
    public static string CharacterUiPath(this string path) => Path.Join(MainFile.ResPath, "images", "charui", path).Replace("\\", "/");
}

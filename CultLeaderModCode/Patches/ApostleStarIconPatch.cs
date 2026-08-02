using CultLeaderMod.CultLeaderModCode.Cards;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using System.Reflection;
using System.Linq;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class ApostleStarIconPatch
{
    private static readonly MegaCrit.Sts2.Core.Logging.Logger Log =
        new("CultLeaderMod.StarIcon", MegaCrit.Sts2.Core.Logging.LogType.Generic);

    private static readonly FieldInfo? _starIconField =
        typeof(NCard).GetField("_starIcon", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo? _starLabelField =
        typeof(NCard).GetField("_starLabel", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo? _energyIconField =
        typeof(NCard).GetField("_energyIcon", BindingFlags.NonPublic | BindingFlags.Instance);

    static ApostleStarIconPatch()
    {
        Log.Info($"[StarIcon] Init: _starIconField={_starIconField != null}, _energyIconField={_energyIconField != null}");
    }

    private static MethodBase TargetMethod()
    {
        return typeof(NCard).GetMethod("UpdateStarCostVisuals", BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    private static void Postfix(NCard __instance)
    {
        var model = __instance.Model;
        if (model is not IApostleCard) return;
        if (model is not CultLeaderModCard ourCard) return;

        var iconPath = ourCard.StarIconPath;
        if (string.IsNullOrEmpty(iconPath)) return;

        var starIcon = _starIconField?.GetValue(__instance) as TextureRect;
        if (starIcon == null)
        {
            starIcon = __instance.GetNodeOrNull<TextureRect>("%StarIcon");
            if (starIcon == null) return;
        }

        var tex = ResourceLoader.Load<Texture2D>(iconPath, null, ResourceLoader.CacheMode.Reuse);
        if (tex == null) return;

        starIcon.Texture = tex;
        starIcon.Visible = true;

        // Position: below the energy icon, aligned to card's left edge
        var energyIcon = _energyIconField?.GetValue(__instance) as Control;
        if (energyIcon != null)
        {
            // Left edge same as energy icon's left (card left edge)
            // Top edge right below energy icon bottom, with small gap
            starIcon.Position = new Vector2(
                energyIcon.Position.X,
                energyIcon.Position.Y + energyIcon.Size.Y + 2f
            );
            Log.Info($"[StarIcon] energyIcon pos=({energyIcon.Position}) size=({energyIcon.Size}), starIcon pos=({starIcon.Position})");
        }

        // About double the previous size
        starIcon.Scale = new Vector2(1.4f, 1.4f);

        // Hide the star cost label
        var starLabel = _starLabelField?.GetValue(__instance) as Control;
        starLabel?.SetVisible(false);
    }
}

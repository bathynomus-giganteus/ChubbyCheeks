using CultLeaderMod.CultLeaderModCode.Cards;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class ApostleStarIconPatch
{
    private const float IconSize = 64f;
    private const float CardWidth = 300f;
    private const float RightMargin = 120f;

    private static MethodBase TargetMethod()
    {
        return typeof(NCard).GetMethod("UpdateStarCostVisuals",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    private static void Postfix(NCard __instance)
    {
        var model = __instance.Model;
        if (model is not IApostleCard) return;
        if (model is not CultLeaderModCard ourCard) return;

        var iconPath = ourCard.StarIconPath;
        if (string.IsNullOrEmpty(iconPath)) return;

        var tex = ResourceLoader.Load<Texture2D>(iconPath, null,
            ResourceLoader.CacheMode.Reuse);
        if (tex == null) return;

        var starIcon = __instance.GetNodeOrNull<TextureRect>("%StarIcon");
        if (starIcon == null) return;

        starIcon.Texture = tex;
        starIcon.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
        starIcon.Visible = true;

        var energyIcon = __instance.GetNodeOrNull<Control>("%EnergyIcon");
        float topY = energyIcon?.Position.Y ?? 6f;
        starIcon.SetSize(new Vector2(IconSize, IconSize));
        starIcon.SetPosition(new Vector2(
            CardWidth - IconSize - RightMargin,
            topY
        ));

        var starLabel = __instance.GetNodeOrNull<Control>("%StarLabel");
        starLabel?.SetVisible(false);
    }
}




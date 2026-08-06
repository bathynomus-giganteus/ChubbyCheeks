using HarmonyLib;
using CultLeaderMod.CultLeaderModCode.CardTags;
using MegaCrit.Sts2.Core.Nodes.Cards;
using STS2RitsuLib.Utils;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class CardFrameColorPatch
{
    [HarmonyPatch(typeof(NCard), "Reload")]
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        try
        {
            var model = __instance.Model;
            if (model == null) return;

            var tags = model.Tags;
            Godot.Material? mat = null;

            // Rainbow first: card with ALL 5 personality tags
            if (tags.Contains(CultLeaderCardTags.Pure) &&
                tags.Contains(CultLeaderCardTags.Calm) &&
                tags.Contains(CultLeaderCardTags.Frenzy) &&
                tags.Contains(CultLeaderCardTags.Lively) &&
                tags.Contains(CultLeaderCardTags.Melancholy))
                mat = CultLeaderFrameColors.Rainbow;
            else if (tags.Contains(CultLeaderCardTags.Pure))        mat = CultLeaderFrameColors.Pure;
            else if (tags.Contains(CultLeaderCardTags.Calm))         mat = CultLeaderFrameColors.Calm;
            else if (tags.Contains(CultLeaderCardTags.Frenzy))       mat = CultLeaderFrameColors.Frenzy;
            else if (tags.Contains(CultLeaderCardTags.Lively))       mat = CultLeaderFrameColors.Lively;
            else if (tags.Contains(CultLeaderCardTags.Melancholy))   mat = CultLeaderFrameColors.Melancholy;
            else return;

            var frame = __instance.GetNodeOrNull<Godot.TextureRect>("%Frame");
            if (frame != null && Godot.GodotObject.IsInstanceValid(mat))
            {
                frame.Material = mat;
            }
        }
        catch { }
    }
}

using CultLeaderMod.CultLeaderModCode.Vfx;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class ApostleAnimationPlayPatch
{
    [HarmonyPrefix]
    private static void BeforeCardPlay(CardModel __instance, Creature? target)
    {
        if (CultLeaderAnimationSettings.Allows(__instance))
            ApostleVfxPlayer.PlayForCard(__instance.GetType(), target);
    }
}

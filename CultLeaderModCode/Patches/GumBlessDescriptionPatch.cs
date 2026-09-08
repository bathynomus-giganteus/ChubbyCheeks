using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Patches;

// Resolve the description from this relic's owner, not a shared localization value.
[HarmonyPatch(typeof(RelicModel), "get_Description")]
public static class GumBlessDescriptionPatch
{
    [HarmonyPostfix]
    private static void Postfix(RelicModel __instance, ref LocString __result)
    {
        if (__instance is GumBlessRelic relic && relic.SelectedPersonalityDescription is { } description)
            __result = description;
        else if (__instance is HappinessOfYongchunRelic upgraded
            && GumBlessRelic.GetSelectionDescription(upgraded) is { } upgradedDescription)
            __result = upgradedDescription;
    }
}

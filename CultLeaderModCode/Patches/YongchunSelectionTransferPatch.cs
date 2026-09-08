using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Replace))]
public static class YongchunSelectionTransferPatch
{
    [HarmonyPrefix]
    private static void Prefix(RelicModel original, RelicModel replace)
    {
        if (original is not GumBlessRelic starter || replace is not HappinessOfYongchunRelic upgraded)
            return;

        // Replace removes the starter before the upgraded relic is obtained.
        upgraded.PersonalityMask = starter.PersonalityMask;
        if (upgraded.PersonalityMask == 0
            && GumBlessRelic.GetSelectedTags(starter.Owner) is { Count: 2 } selected)
            upgraded.PersonalityMask = GumBlessRelic.EncodeSelection(selected);
    }
}

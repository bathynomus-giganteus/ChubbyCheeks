using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

using Sts2CardModel = MegaCrit.Sts2.Core.Models.CardModel;
using Sts2PowerModel = MegaCrit.Sts2.Core.Models.PowerModel;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[HarmonyPatch]
public static class HappinessPowerPatch
{
    [HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.ModifyAmount))]
    [HarmonyPatch([
        typeof(PlayerChoiceContext),
        typeof(Sts2PowerModel),
        typeof(decimal),
        typeof(Creature),
        typeof(Sts2CardModel),
        typeof(bool),
    ])]
    [HarmonyPostfix]
    private static void ModifyAmountPostfix(
        Sts2PowerModel power,
        decimal offset,
        Creature applier,
        Sts2CardModel cardSource,
        ref Task<int> __result
    )
    {
        // Triggering is handled by HappinessPower.AfterPowerAmountChanged.
        if (power is HappinessPower && offset > 0m)
        {
            _ = applier;
            _ = cardSource;
        }
    }
}

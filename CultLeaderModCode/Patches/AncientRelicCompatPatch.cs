using System.Collections.Generic;
using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothTranscendencePatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        var starter = ModelDb.Card<TestAddApostleCards>();
        var ancient = ModelDb.Card<SaviorDescendsCard>();
        if (starter != null && ancient != null)
        {
            __result[starter.Id] = ancient;
        }
    }
}

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasYongchunPatch
{
    [HarmonyPostfix]
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is GumBlessRelic)
        {
            __result = ModelDb.Relic<HappinessOfYongchunRelic>();
        }
    }
}

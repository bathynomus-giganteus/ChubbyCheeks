using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch(typeof(CardFactory), "RollForRarity")]
public static class CardRarityBonusPatch
{
    [HarmonyPostfix]
    private static void Postfix(
        Player player,
        HashSet<CardRarity> allowedRarities,
        ref CardRarity __result)
    {
        var bonus = GumBlessRelic.GetHighRarityBonusPercent(player);
        if (__result != CardRarity.Common || bonus <= 0)
            return;

        var highRarities = new[] { CardRarity.Uncommon, CardRarity.Rare }
            .Where(allowedRarities.Contains)
            .ToArray();
        if (highRarities.Length == 0
            || player.PlayerRng.Rewards.NextInt(100) >= bonus)
            return;

        __result = highRarities[player.PlayerRng.Rewards.NextInt(highRarities.Length)];
    }
}

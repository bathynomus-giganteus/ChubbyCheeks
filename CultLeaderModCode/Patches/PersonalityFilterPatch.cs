using HarmonyLib;
using CultLeaderMod.CultLeaderModCode.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class PersonalityFilterPatch
{
    /// <summary>
    /// Patch CardFactory.GetDistinctForCombat to apply personality filtering.
    /// This covers: potions, Splash, Discovery, Grand Prize, etc.
    /// </summary>
    [HarmonyPatch(typeof(CardFactory), "GetDistinctForCombat")]
    [HarmonyPrefix]
    private static void Prefix(ref IEnumerable<CardModel> cards)
    {
        if (!GumBlessRelic.SelectionMade) return;

        var filtered = cards.Where(card =>
        {
            if (GumBlessRelic.IsUnselectedPersonalityCard(card))
            {
                return Random.Shared.NextDouble() >= 0.85;
            }
            return true;
        }).ToList();

        if (filtered.Count > 0)
        {
            cards = filtered;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using CultLeaderMod.CultLeaderModCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class PersonalityFilterPatch
{
    [HarmonyPatch(typeof(CardFactory), "GetDistinctForCombat")]
    [HarmonyPrefix]
    private static void Prefix(ref IEnumerable<CardModel> cards)
    {
        try
        {
            if (!GumBlessRelic.SelectionMade) return;

            Entry.Logger.Info($"[PersonalityFilter] GetDistinctForCombat called, SelectionMade=true");

            var filtered = cards.Where(card =>
            {
                try
                {
                    if (GumBlessRelic.IsUnselectedPersonalityCard(card))
                    {
                        return Random.Shared.NextDouble() >= 0.85;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Entry.Logger.Error($"[PersonalityFilter] Error filtering card {card?.GetType().Name}: {ex}");
                    return true;
                }
            }).ToList();

            if (filtered.Count > 0)
            {
                cards = filtered;
                Entry.Logger.Info($"[PersonalityFilter] Filtered cards count: {filtered.Count}");
            }
            else
            {
                Entry.Logger.Warn("[PersonalityFilter] All cards filtered out, keeping original");
            }
        }
        catch (Exception ex)
        {
            Entry.Logger.Error($"[PersonalityFilter] Prefix error: {ex}");
        }
    }

    [HarmonyPatch(typeof(Hook), "ModifyMerchantCardPool")]
    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<CardModel> __result)
    {
        try
        {
            if (!GumBlessRelic.SelectionMade) return;

            var list = __result?.ToList() ?? new List<CardModel>();
            __result = GumBlessRelic.FilterUnselectedCards(list);
        }
        catch (Exception ex)
        {
            Entry.Logger.Error($"[PersonalityFilter] Merchant card pool filter error: {ex}");
        }
    }
}
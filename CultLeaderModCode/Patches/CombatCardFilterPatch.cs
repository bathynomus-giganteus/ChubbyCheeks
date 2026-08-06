using System.Reflection;
using HarmonyLib;
using CultLeaderMod.CultLeaderModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace CultLeaderMod.CultLeaderModCode.Patches;

/// <summary>
/// Patches CardPileCmd.Add (batch) and AddGeneratedCardsToCombat to filter out
/// unselected personality cards during in-combat card generation.
/// </summary>
public static class CombatCardFilterPatch
{
    [HarmonyPatch(typeof(CardPileCmd), "AddGeneratedCardsToCombat")]
    [HarmonyPatch(new[] { typeof(IEnumerable<CardModel>), typeof(PileType), typeof(Creature), typeof(CardPilePosition) })]
    [HarmonyPrefix]
    private static bool Prefix_AddGeneratedCardsToCombat(
        IEnumerable<CardModel> cards,
        PileType destinationPile,
        Creature target,
        CardPilePosition position,
        ref Task<IReadOnlyList<CardModel>> __result)
    {
        if (!GumBlessRelic.SelectionMade || GumBlessRelic.UnselectedTags == null)
            return true;

        var list = cards.ToList();
        var filtered = GumBlessRelic.FilterUnselectedCards(list);

        if (ReferenceEquals(filtered, list))
            return true;

        Entry.Logger.Info($"[CombatCardFilter] AddGeneratedCardsToCombat: filtered {list.Count - filtered.Count} cards");

        var original = AccessTools.Method(typeof(CardPileCmd), "AddGeneratedCardsToCombat",
            new[] { typeof(IEnumerable<CardModel>), typeof(PileType), typeof(Creature), typeof(CardPilePosition) });
        __result = (Task<IReadOnlyList<CardModel>>)original.Invoke(null, new object[] { filtered, destinationPile, target, position })!;
        return false;
    }

    [HarmonyPatch(typeof(CardPileCmd), "Add")]
    [HarmonyPatch(new[] { typeof(IEnumerable<CardModel>), typeof(PileType), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool) })]
    [HarmonyPrefix]
    private static bool Prefix_AddBatch(
        IEnumerable<CardModel> cards,
        PileType pileType,
        CardPilePosition position,
        AbstractModel? anchor,
        bool skipEvents,
        ref Task<IReadOnlyList<CardModel>> __result)
    {
        if (!GumBlessRelic.SelectionMade || GumBlessRelic.UnselectedTags == null)
            return true;

        var list = cards.ToList();
        var filtered = GumBlessRelic.FilterUnselectedCards(list);

        if (ReferenceEquals(filtered, list))
            return true;

        Entry.Logger.Info($"[CombatCardFilter] Add(batch): filtered {list.Count - filtered.Count} cards");

        var original = AccessTools.Method(typeof(CardPileCmd), "Add",
            new[] { typeof(IEnumerable<CardModel>), typeof(PileType), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool) });
        __result = (Task<IReadOnlyList<CardModel>>)original.Invoke(null, new object[] { filtered, pileType, position, anchor, skipEvents })!;
        return false;
    }
}

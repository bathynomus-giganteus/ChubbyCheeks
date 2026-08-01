using CultLeaderMod.CultLeaderModCode.Cards;
using CultLeaderMod.CultLeaderModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using System.Reflection;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class ApostleRewardWeightPatch
{
    private static MethodBase TargetMethod() =>
        typeof(CardFactory)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(method =>
            {
                if (method.Name != "CreateForReward")
                    return false;

                ParameterInfo[] parameters = method.GetParameters();
                return parameters.Length == 3 &&
                       parameters[0].ParameterType == typeof(Player) &&
                       parameters[1].ParameterType == typeof(IEnumerable<CardModel>) &&
                       parameters[2].ParameterType == typeof(CardCreationOptions);
            });

    private static void Postfix(
        Player player,
        IEnumerable<CardModel> blacklist,
        CardCreationOptions options,
        ref CardModel __result)
    {
        CultLeaderStartingRelic? relic = player.GetRelic<CultLeaderStartingRelic>();
        if (relic is null || __result is null)
            return;

        var resultRarity = __result.Rarity;
        HashSet<ModelId> excluded = blacklist.Select(card => card.Id).ToHashSet();
        List<CardModel> sameRarityCandidates = options
            .GetPossibleCards(player)
            .Where(card => card.Rarity == resultRarity && !excluded.Contains(card.Id))
            .DistinctBy(card => card.Id)
            .ToList();

        List<CardModel> weighted = relic.ApplyPersonalityWeights(sameRarityCandidates).ToList();
        if (weighted.Count == 0)
            return;

        var rng = options.RngOverride ?? player.PlayerRng.Rewards;
        CardModel canonical = weighted[rng.NextInt(weighted.Count)];
        __result = player.RunState.CreateCard(canonical, player);
    }
}

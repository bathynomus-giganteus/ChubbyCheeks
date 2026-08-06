using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

using Sts2PowerModel = MegaCrit.Sts2.Core.Models.PowerModel;
using Sts2CardModel = MegaCrit.Sts2.Core.Models.CardModel;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[HarmonyPatch]
public static class PowerInterceptPatch
{
    private static readonly Dictionary<Type, Type> BaseToUpgraded = new()
    {
        { typeof(RegenPower), typeof(LifeEssencePower) },
        { typeof(PlatingPower), typeof(SolidIcePower) },
        { typeof(VigorPower), typeof(FervorPower) },
        { typeof(BitterPainPower), typeof(BitterPainBurstPower) },
        { typeof(ArtifactPower), typeof(HappinessPower) },
    };

    private static readonly HashSet<Type> AllMonitoredTypes = new()
    {
        typeof(RegenPower), typeof(PlatingPower), typeof(VigorPower),
        typeof(BitterPainPower), typeof(ArtifactPower),
        typeof(LifeEssencePower), typeof(SolidIcePower), typeof(FervorPower),
        typeof(BitterPainBurstPower), typeof(HappinessPower),
    };

    private static bool IsApostleCard(Sts2CardModel? card)
    {
        if (card == null) return false;
        var tags = Traverse.Create(card).Property<object>("CanonicalTags").Value;
        if (tags is System.Collections.Generic.HashSet<MegaCrit.Sts2.Core.Entities.Cards.CardTag> tagSet)
        {
            return tagSet.Any(t => t.ToString()?.Contains("Apostle") == true);
        }
        return false;
    }

    [HarmonyPatch(typeof(PowerCmd), "Apply")]
    [HarmonyPatch(
        [typeof(PlayerChoiceContext), typeof(Sts2PowerModel), typeof(Creature),
         typeof(decimal), typeof(Creature), typeof(Sts2CardModel), typeof(bool)])]
    [HarmonyPrefix]
    private static void ApplyPrefix(
        PlayerChoiceContext choiceContext,
        Sts2PowerModel power,
        Creature target,
        ref decimal amount,
        Creature applier,
        Sts2CardModel cardSource,
        bool silent)
    {
        if (target == null || amount <= 0) return;
        var powerType = power.GetType();
        var authority = target.Powers?.OfType<CultLeaderAuthorityPower>().FirstOrDefault();
        bool isApostle = IsApostleCard(cardSource);

        if (authority != null && isApostle && AllMonitoredTypes.Contains(powerType))
        {
            amount *= (1 + authority.Amount);
        }
    }

    [HarmonyPatch(typeof(PowerCmd), "Apply")]
    [HarmonyPatch(
        [typeof(PlayerChoiceContext), typeof(Sts2PowerModel), typeof(Creature),
         typeof(decimal), typeof(Creature), typeof(Sts2CardModel), typeof(bool)])]
    [HarmonyPostfix]
    private static async void ApplyPostfix(
        PlayerChoiceContext choiceContext,
        Sts2PowerModel power,
        Creature target,
        decimal amount,
        Creature applier,
        Sts2CardModel cardSource,
        bool silent)
    {
        if (target == null || amount <= 0) return;
        var powerType = power.GetType();

        var elderForm = target.Powers?.OfType<ElderFormPower>().FirstOrDefault();
        if (elderForm != null && BaseToUpgraded.TryGetValue(powerType, out var upgradedType))
        {
            var existingPower = target.Powers?.FirstOrDefault(p => p.GetType() == powerType && p != power);
            if (existingPower != null)
            {
                await PowerCmd.ModifyAmount(choiceContext, existingPower, -amount, applier, null);
            }
            var applyMethod = typeof(PowerCmd).GetMethods()
                .First(m => m.Name == "Apply" && m.IsGenericMethodDefinition && m.GetParameters().Length == 6);
            var genericApply = applyMethod.MakeGenericMethod(upgradedType);
            await (Task)genericApply.Invoke(null, [choiceContext, target, amount, applier, (Sts2CardModel?)cardSource, silent]);
        }

        var authority = target.Powers?.OfType<CultLeaderAuthorityPower>().FirstOrDefault();
        if (authority != null && authority.Amount >= 5)
        {
            var hasElderForm = target.Powers?.OfType<ElderFormPower>().FirstOrDefault();
            if (hasElderForm == null)
            {
                await PowerCmd.ModifyAmount(choiceContext, authority, -5m, applier, null);
                await PowerCmd.Apply<ElderFormPower>(choiceContext, target, 1m, applier, null);
            }
        }
    }
}

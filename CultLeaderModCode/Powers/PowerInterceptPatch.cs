using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using Sts2CardModel = MegaCrit.Sts2.Core.Models.CardModel;
using Sts2PowerModel = MegaCrit.Sts2.Core.Models.PowerModel;

namespace CultLeaderMod.CultLeaderModCode.Powers;

[HarmonyPatch]
public static class PowerInterceptPatch
{
    [HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Apply))]
    [HarmonyPatch([
        typeof(PlayerChoiceContext),
        typeof(Sts2PowerModel),
        typeof(Creature),
        typeof(decimal),
        typeof(Creature),
        typeof(Sts2CardModel),
        typeof(bool),
    ])]
    [HarmonyPostfix]
    private static void AuthorityApplyPostfix(
        PlayerChoiceContext choiceContext,
        Sts2PowerModel power,
        Creature target,
        Creature applier,
        Sts2CardModel cardSource,
        ref Task __result
    )
    {
        if (power is CultLeaderAuthorityPower)
            __result = AwaitApplyAndTryEnterElderForm(
                __result,
                choiceContext,
                power,
                target,
                applier,
                cardSource
            );
    }

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
    private static void AuthorityModifyAmountPostfix(
        PlayerChoiceContext choiceContext,
        Sts2PowerModel power,
        Creature applier,
        Sts2CardModel cardSource,
        ref Task<int> __result
    )
    {
        if (power is CultLeaderAuthorityPower)
            __result = AwaitModifyAndTryEnterElderForm(
                __result,
                choiceContext,
                power,
                power.Owner,
                applier,
                cardSource
            );
        else if (power is LifeEssencePower life && life.Owner != null)
            __result = AwaitLifeEssenceModifyHpSync(__result, life, life.Owner);
    }

    private static async Task<int> AwaitLifeEssenceModifyHpSync(
        Task<int> original, LifeEssencePower life, Creature target)
    {
        var result = await original;
        int targetHp = life.Amount * 5;
        int delta = targetHp - life.GrantedHp;
        if (delta != 0)
        {
            if (delta > 0)
                await CreatureCmd.GainMaxHp(target, delta);
            else
                await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), target, -delta, false);
            life.GrantedHp = targetHp;
        }
        life.TrackedAmount = life.Amount;
        return result;
    }

    private static async Task AwaitApplyAndTryEnterElderForm(
        Task original,
        PlayerChoiceContext choiceContext,
        Sts2PowerModel power,
        Creature target,
        Creature? applier,
        Sts2CardModel? cardSource
    )
    {
        await original;
        await TryEnterElderForm(choiceContext, power, target, applier, cardSource);
    }

    /// <summary>
    /// LifeEssencePower 的 HP 变更 + TempMaxHpPower 同步。
    /// 每次 PowerCmd.Apply 都触发（包括首次和后续层数增加）。
    /// </summary>
    [HarmonyPatch(typeof(PowerCmd), nameof(PowerCmd.Apply))]
    [HarmonyPatch([
        typeof(PlayerChoiceContext),
        typeof(Sts2PowerModel),
        typeof(Creature),
        typeof(decimal),
        typeof(Creature),
        typeof(Sts2CardModel),
        typeof(bool),
    ])]
    [HarmonyPostfix]
    private static void LifeEssenceHpSyncPostfix(
        Sts2PowerModel power,
        Creature target,
        Creature applier,
        Sts2CardModel cardSource,
        ref Task __result)
    {
        if (power is LifeEssencePower life && target != null)
            __result = AwaitLifeEssenceHpSync(__result, life, target, applier, cardSource);
    }

    private static async Task AwaitLifeEssenceHpSync(
        Task original, LifeEssencePower life, Creature target, Creature applier, Sts2CardModel cardSource)
    {
        await original;
        int targetHp = life.Amount * 5;
        int delta = targetHp - life.GrantedHp;
        if (delta != 0)
        {
            if (delta > 0)
                await CreatureCmd.GainMaxHp(target, delta);
            else
                await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), target, -delta, false);
            life.GrantedHp = targetHp;
        }
        life.TrackedAmount = life.Amount;

        // 同步 TempMaxHpPower 视觉标记
        try
        {
            int targetTempHp = life.Amount * 5;
            var tempHp = target.Powers?.OfType<TempMaxHpPower>().FirstOrDefault();
            if (tempHp != null)
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), tempHp, targetTempHp - tempHp.Amount, applier, cardSource, silent: true);
            else if (targetTempHp > 0)
                await PowerCmd.Apply<TempMaxHpPower>(new ThrowingPlayerChoiceContext(), target, targetTempHp, applier, cardSource);
        }
        catch { }
    }

    private static async Task<int> AwaitModifyAndTryEnterElderForm(
        Task<int> original,
        PlayerChoiceContext choiceContext,
        Sts2PowerModel power,
        Creature target,
        Creature? applier,
        Sts2CardModel? cardSource
    )
    {
        var result = await original;
        await TryEnterElderForm(choiceContext, power, target, applier, cardSource);
        return result;
    }

    private static async Task TryEnterElderForm(
        PlayerChoiceContext choiceContext,
        Sts2PowerModel power,
        Creature target,
        Creature? applier,
        Sts2CardModel? cardSource
    )
    {
        if (ApostlePowerRules.IsConverting || power is not CultLeaderAuthorityPower authority)
            return;

        if (
            target == null
            || target != authority.Owner
            || authority.Amount < 5
            || target.Powers?.OfType<ElderFormPower>().Any() == true
        )
            return;

        Entry.Logger.Info(
            $"[Authority] {authority.Amount} stacks reached; consuming 5 and entering Elder Form."
        );

        await PowerCmd.ModifyAmount(
            choiceContext,
            authority,
            -5m,
            applier ?? target,
            cardSource,
            silent: true
        );
        await PowerCmd.Apply<ElderFormPower>(
            choiceContext,
            target,
            1m,
            applier ?? target,
            cardSource
        );
    }
}

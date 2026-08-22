using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CultLeaderMod.CultLeaderModCode.Cards;

internal static class ApostleCardEffectHelpers
{
    public static int PureStacks(Creature owner)
    {
        return (int)(
            (owner.GetPower<HealingPower>()?.Amount ?? 0m)
            + (owner.GetPower<LifeEssencePower>()?.Amount ?? 0m)
        );
    }

    public static int FrenzyStacks(Creature owner)
    {
        return (int)(
            (owner.GetPower<VigorPower>()?.Amount ?? 0m)
            + (owner.GetPower<FervorPower>()?.Amount ?? 0m)
        );
    }
    public static int GetFrenzyResourceAmount(Creature owner)
    {
        return ApostlePowerRules.HasElderForm(owner)
            ? (int)(owner.GetPower<FervorPower>()?.Amount ?? 0m)
            : (int)(owner.GetPower<VigorPower>()?.Amount ?? 0m);
    }

    public static List<Creature> AliveEnemies(Creature owner)
    {
        return owner.CombatState
                ?.GetCreaturesOnSide(CombatSide.Enemy)
                .Where(enemy => !enemy.IsDead)
                .ToList()
            ?? [];
    }

    public static Creature? RandomEnemy(Creature owner)
    {
        var enemies = AliveEnemies(owner);
        return enemies.Count == 0 ? null : enemies[Random.Shared.Next(enemies.Count)];
    }

    public static Task Attack(
        PlayerChoiceContext choiceContext,
        CardModel card,
        CardPlay cardPlay,
        Creature target,
        decimal damage
    )
    {
        return DamageCmd
            .Attack(damage)
            .FromCard(card, cardPlay)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public static Task<AttackCommand> AttackAndGetResult(
        PlayerChoiceContext choiceContext,
        CardModel card,
        CardPlay cardPlay,
        Creature target,
        decimal damage
    )
    {
        return DamageCmd
            .Attack(damage)
            .FromCard(card, cardPlay)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public static Task AttackAll(
        PlayerChoiceContext choiceContext,
        CardModel card,
        CardPlay cardPlay,
        Creature owner,
        decimal damage
    )
    {
        var combatState = owner.CombatState;
        if (combatState == null)
            return Task.CompletedTask;

        return DamageCmd
            .Attack(damage)
            .FromCard(card, cardPlay)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);
    }

    public static async Task ApplyTemporaryStrengthLoss(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature source,
        CardModel cardSource
    )
    {
        if (amount <= 0)
            return;

        await PowerCmd.Apply<TempStrengthLossPower>(choiceContext, target, amount, source, cardSource);
    }

    public static async Task TriggerPureStacks(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int times,
        CardModel cardSource
    )
    {
        for (int i = 0; i < times; i++)
        {
            var lifeEssence = owner.GetPower<LifeEssencePower>();
            if (lifeEssence != null && lifeEssence.Amount > 0)
            {
                await lifeEssence.TriggerActive(choiceContext, owner, cardSource);
                continue;
            }

            var healing = owner.GetPower<HealingPower>();
            if (healing != null && healing.Amount > 0)
            {
                await healing.TriggerActive(choiceContext, owner, cardSource);
            }
        }
    }

    public static async Task RemovePureStacks(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int times,
        CardModel cardSource
    )
    {
        for (int i = 0; i < times; i++)
        {
            var lifeEssence = owner.GetPower<LifeEssencePower>();
            if (lifeEssence != null && lifeEssence.Amount > 0)
            {
                await PowerCmd.ModifyAmount(choiceContext, lifeEssence, -1m, owner, cardSource, silent: true);
                continue;
            }

            var healing = owner.GetPower<HealingPower>();
            if (healing != null && healing.Amount > 0)
            {
                await PowerCmd.ModifyAmount(choiceContext, healing, -1m, owner, cardSource, silent: true);
            }
        }
    }

    public static async Task RemovePureStacksBulk(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int amount,
        CardModel cardSource
    )
    {
        if (amount <= 0)
            return;

        var lifeEssence = owner.GetPower<LifeEssencePower>();
        decimal lifeAmount = lifeEssence?.Amount ?? 0m;
        decimal takeLife = Math.Min(lifeAmount, amount);
        if (lifeEssence != null && takeLife > 0m)
            await PowerCmd.ModifyAmount(choiceContext, lifeEssence, -takeLife, owner, cardSource, silent: true);

        decimal remaining = amount - takeLife;
        if (remaining <= 0m)
            return;

        var healing = owner.GetPower<HealingPower>();
        decimal healingAmount = healing?.Amount ?? 0m;
        decimal takeHealing = Math.Min(healingAmount, remaining);
        if (healing != null && takeHealing > 0m)
            await PowerCmd.ModifyAmount(choiceContext, healing, -takeHealing, owner, cardSource, silent: true);
    }


    public static int CalmStacks(Creature owner)
    {
        return (int)(
            (owner.GetPower<PlatingPower>()?.Amount ?? 0m)
            + (owner.GetPower<SolidIcePower>()?.Amount ?? 0m)
        );
    }

    public static async Task TriggerCalmStacks(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int times,
        CardModel cardSource
    )
    {
        for (int i = 0; i < times; i++)
        {
            var solidIce = owner.GetPower<SolidIcePower>();
            if (solidIce != null && solidIce.Amount > 0m)
            {
                await solidIce.TriggerActive(choiceContext, owner, cardSource);
                continue;
            }

            var plating = owner.GetPower<PlatingPower>();
            if (plating != null && plating.Amount > 0m)
            {
                decimal block = plating.Amount;
                await PowerCmd.ModifyAmount(choiceContext, plating, -1m, owner, cardSource, silent: true);
                await CreatureCmd.GainBlock(owner, block, ValueProp.Move, null, true);
            }
        }
    }

    public static async Task RemoveCalmStacks(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int amount,
        CardModel cardSource
    )
    {
        if (amount <= 0)
            return;

        var solidIce = owner.GetPower<SolidIcePower>();
        decimal solidAmount = solidIce?.Amount ?? 0m;
        decimal takeSolid = Math.Min(solidAmount, amount);
        if (solidIce != null && takeSolid > 0m)
            await PowerCmd.ModifyAmount(choiceContext, solidIce, -takeSolid, owner, cardSource, silent: true);

        decimal remaining = amount - takeSolid;
        if (remaining <= 0m)
            return;

        var plating = owner.GetPower<PlatingPower>();
        decimal platingAmount = plating?.Amount ?? 0m;
        decimal takePlating = Math.Min(platingAmount, remaining);
        if (plating != null && takePlating > 0m)
            await PowerCmd.ModifyAmount(choiceContext, plating, -takePlating, owner, cardSource, silent: true);
    }

    public static int CountCombatCards(Player player, Func<CardModel, bool> predicate)
    {
        PileType[] piles = [PileType.Draw, PileType.Hand, PileType.Discard, PileType.Exhaust];
        return piles.Sum(pile => pile.GetPile(player).Cards.Count(predicate));
    }

    /// <summary>
    /// 统计当前卡组（抽牌堆 + 手牌 + 弃牌堆，不含消耗堆）中满足条件的卡牌数量。
    /// </summary>
    public static int CountDeckCards(Player player, Func<CardModel, bool> predicate)
    {
        PileType[] piles = [PileType.Draw, PileType.Hand, PileType.Discard];
        return piles.Sum(pile => pile.GetPile(player).Cards.Count(predicate));
    }

    public static async Task<bool> TryTriggerCalmStack(
        PlayerChoiceContext choiceContext,
        Creature owner,
        CardModel cardSource
    )
    {
        var solidIce = owner.GetPower<SolidIcePower>();
        if (solidIce != null && solidIce.Amount > 0m)
        {
            await solidIce.TriggerActive(choiceContext, owner, cardSource);
            return true;
        }

        var plating = owner.GetPower<PlatingPower>();
        if (plating != null && plating.Amount > 0m)
        {
            decimal block = plating.Amount;
            await PowerCmd.ModifyAmount(choiceContext, plating, -1m, owner, cardSource, silent: true);
            await CreatureCmd.GainBlock(owner, block, ValueProp.Move, null, true);
            return true;
        }

        return false;
    }

    public static int LivelyStacks(Creature owner)
    {
        return (int)(
            (owner.GetPower<RetainPower>()?.Amount ?? 0m)
            + (owner.GetPower<HappinessPower>()?.Amount ?? 0m)
        );
    }

    public static async Task RemoveLivelyStacks(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int amount,
        CardModel cardSource
    )
    {
        if (amount <= 0)
            return;

        var happiness = owner.GetPower<HappinessPower>();
        decimal happinessAmount = happiness?.Amount ?? 0m;
        decimal takeHappiness = Math.Min(happinessAmount, amount);
        if (happiness != null && takeHappiness > 0m)
            await PowerCmd.ModifyAmount(choiceContext, happiness, -takeHappiness, owner, cardSource, silent: true);

        decimal remaining = amount - takeHappiness;
        if (remaining <= 0m)
            return;

        var retain = owner.GetPower<RetainPower>();
        decimal retainAmount = retain?.Amount ?? 0m;
        decimal takeRetain = Math.Min(retainAmount, remaining);
        if (retain != null && takeRetain > 0m)
            await PowerCmd.ModifyAmount(choiceContext, retain, -takeRetain, owner, cardSource, silent: true);
    }

    public static int MelancholyStacks(Creature owner)
    {
        return (int)(
            (owner.GetPower<BitterPainPower>()?.Amount ?? 0m)
            + (owner.GetPower<BitterPainBurstPower>()?.Amount ?? 0m)
        );
    }

    public static async Task RemoveMelancholyStacks(
        PlayerChoiceContext choiceContext,
        Creature owner,
        int amount,
        CardModel cardSource
    )
    {
        if (amount <= 0)
            return;

        if (ApostlePowerRules.HasElderForm(owner))
        {
            var burst = owner.GetPower<BitterPainBurstPower>();
            decimal burstAmount = burst?.Amount ?? 0m;
            decimal takeBurst = Math.Min(burstAmount, amount);
            if (burst != null && takeBurst > 0m)
                await PowerCmd.ModifyAmount(choiceContext, burst, -takeBurst, owner, cardSource, silent: true);

            decimal remaining = amount - takeBurst;
            if (remaining <= 0m)
                return;

            var bitter = owner.GetPower<BitterPainPower>();
            decimal bitterAmount = bitter?.Amount ?? 0m;
            decimal takeBitter = Math.Min(bitterAmount, remaining);
            if (bitter != null && takeBitter > 0m)
                await PowerCmd.ModifyAmount(choiceContext, bitter, -takeBitter, owner, cardSource, silent: true);
            return;
        }

        var baseBitter = owner.GetPower<BitterPainPower>();
        decimal baseBitterAmount = baseBitter?.Amount ?? 0m;
        decimal takeBase = Math.Min(baseBitterAmount, amount);
        if (baseBitter != null && takeBase > 0m)
            await PowerCmd.ModifyAmount(choiceContext, baseBitter, -takeBase, owner, cardSource, silent: true);

        decimal remainingBase = amount - takeBase;
        if (remainingBase <= 0m)
            return;

        var baseBurst = owner.GetPower<BitterPainBurstPower>();
        decimal baseBurstAmount = baseBurst?.Amount ?? 0m;
        decimal takeBaseBurst = Math.Min(baseBurstAmount, remainingBase);
        if (baseBurst != null && takeBaseBurst > 0m)
            await PowerCmd.ModifyAmount(choiceContext, baseBurst, -takeBaseBurst, owner, cardSource, silent: true);
    }

    public static int CountDebuffStacks(Creature target)
    {
        return target.Powers
            .Where(power => power.Type == PowerType.Debuff)
            .Sum(power => Math.Max(0, (int)power.Amount));
    }

    public static int CountDebuffTypes(Creature target)
    {
        return target.Powers
            .Count(power => power.Type == PowerType.Debuff && power.Amount > 0m);
    }

    public static bool HasDebuff(Creature target)
    {
        return target.Powers.Any(power => power.Type == PowerType.Debuff && power.Amount > 0m);
    }

    public static async Task<int> RemoveRandomDebuffStacks(
        PlayerChoiceContext choiceContext,
        Creature target,
        int amount)
    {
        int removed = 0;
        for (int i = 0; i < amount; i++)
        {
            var debuffs = target.Powers
                .Where(power => power.Type == PowerType.Debuff && power.Amount > 0m)
                .ToList();

            if (debuffs.Count == 0)
                break;

            var power = debuffs[Random.Shared.Next(debuffs.Count)];
            if (power.Amount <= 1m)
                await PowerCmd.Remove(power);
            else
                await PowerCmd.ModifyAmount(choiceContext, power, -1m, null, null, silent: true);

            removed++;
        }

        return removed;
    }
}

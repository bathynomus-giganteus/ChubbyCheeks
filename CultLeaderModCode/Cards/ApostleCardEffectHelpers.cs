using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CultLeaderMod.CultLeaderModCode.Cards;

internal static class ApostleCardEffectHelpers
{
    public static int PureStacks(Creature owner)
    {
        return (int)(
            (owner.GetPower<RegenPower>()?.Amount ?? 0m)
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

        await PowerCmd.Apply<StrengthPower>(choiceContext, target, -amount, source, cardSource);
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

            var regen = owner.GetPower<RegenPower>();
            if (regen != null && regen.Amount > 0)
            {
                await CreatureCmd.Heal(owner, 5m, true);
                await PowerCmd.ModifyAmount(choiceContext, regen, -1m, owner, cardSource, silent: true);
            }
        }
    }

    public static int CountCombatCards(Player player, Func<CardModel, bool> predicate)
    {
        PileType[] piles = [PileType.Draw, PileType.Hand, PileType.Discard, PileType.Exhaust];
        return piles.Sum(pile => pile.GetPile(player).Cards.Count(predicate));
    }
}

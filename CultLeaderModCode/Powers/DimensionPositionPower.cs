using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 次元定位：下一张攻击牌结算后，恢复该攻击牌消耗、触发或移除的核心增益层数。
/// </summary>
[RegisterPower]
public class DimensionPositionPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_06.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/狂热_06.png";

    private Dictionary<Type, decimal>? snapshotBeforeAttack;
    private readonly Dictionary<Type, decimal> lostDuringAttack = [];

    private static readonly Type[] RestorablePowerTypes =
    [
        typeof(HealingPower),
        typeof(LifeEssencePower),
        typeof(PlatingPower),
        typeof(SolidIcePower),
        typeof(VigorPower),
        typeof(FervorPower),
        typeof(RetainPower),
        typeof(HappinessPower),
        typeof(BitterPainPower),
        typeof(BitterPainBurstPower),
        typeof(StrengthPower),
        typeof(DexterityPower),
        typeof(ArtifactPower),
        typeof(BufferPower),
    ];

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay
    )
    {
        if (
            base.Amount > 0m
            && snapshotBeforeAttack == null
            && dealer == base.Owner
            && cardSource?.Type == CardType.Attack
            && cardPlay != null
        )
            Snapshot();

        return base.ModifyDamageAdditive(target, amount, props, dealer, cardSource, cardPlay);
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        await base.AfterAttack(choiceContext, command);

        if (base.Amount <= 0m || command.Attacker != base.Owner || command.CardPlay?.Card.Type != CardType.Attack)
            return;

        if (snapshotBeforeAttack != null)
            await RestoreLostStacks(choiceContext, command.CardPlay.Card);
        else if (lostDuringAttack.Count > 0)
            await RestoreRecordedStacks(choiceContext, command.CardPlay.Card);

        snapshotBeforeAttack = null;
        lostDuringAttack.Clear();
        await PowerCmd.ModifyAmount(choiceContext, this, -1m, base.Owner, command.CardPlay.Card, silent: true);
    }

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);

        if (
            base.Amount <= 0m
            || amount >= 0m
            || power.Owner != base.Owner
            || cardSource?.Type != CardType.Attack
            || !RestorablePowerTypes.Contains(power.GetType())
        )
            return;

        var type = power.GetType();
        lostDuringAttack.TryGetValue(type, out decimal oldLoss);
        lostDuringAttack[type] = oldLoss + (-amount);
    }

    private void Snapshot()
    {
        snapshotBeforeAttack = RestorablePowerTypes
            .Select(type => (type, amount: GetAmount(type)))
            .Where(item => item.amount > 0m)
            .ToDictionary(item => item.type, item => item.amount);
    }

    private async Task RestoreLostStacks(PlayerChoiceContext choiceContext, CardModel cardSource)
    {
        foreach (var (type, oldAmount) in snapshotBeforeAttack!)
        {
            var currentAmount = GetAmount(type);
            var missing = oldAmount - currentAmount;
            lostDuringAttack.TryGetValue(type, out decimal recordedLoss);
            var restore = Math.Max(missing, recordedLoss);
            if (restore <= 0m)
                continue;

            await ApplyStacks(choiceContext, type, restore, cardSource);
        }

        await RestoreRecordedStacks(choiceContext, cardSource);
    }

    private async Task RestoreRecordedStacks(PlayerChoiceContext choiceContext, CardModel cardSource)
    {
        foreach (var (type, amount) in lostDuringAttack)
        {
            if (snapshotBeforeAttack != null && snapshotBeforeAttack.ContainsKey(type))
                continue;

            if (amount > 0m)
                await ApplyStacks(choiceContext, type, amount, cardSource);
        }
    }

    private decimal GetAmount(Type powerType)
    {
        return base.Owner.Powers?.FirstOrDefault(power => power.GetType() == powerType)?.Amount ?? 0m;
    }

    private Task ApplyStacks(
        PlayerChoiceContext choiceContext,
        Type powerType,
        decimal amount,
        CardModel cardSource
    )
    {
        if (powerType == typeof(HealingPower))
            return ApostleCardPlayHelpers.ApplyPurePower(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(LifeEssencePower))
            return PowerCmd.Apply<LifeEssencePower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(PlatingPower))
            return ApostleCardPlayHelpers.ApplyCalmPower(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(SolidIcePower))
            return PowerCmd.Apply<SolidIcePower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(VigorPower))
            return ApostleCardPlayHelpers.ApplyFrenzyPower(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(FervorPower))
            return PowerCmd.Apply<FervorPower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(RetainPower))
            return ApostleCardPlayHelpers.ApplyLivelyPower(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(HappinessPower))
            return PowerCmd.Apply<HappinessPower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(BitterPainPower))
            return ApostleCardPlayHelpers.ApplyMelancholyPower(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(BitterPainBurstPower))
            return PowerCmd.Apply<BitterPainBurstPower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(StrengthPower))
            return PowerCmd.Apply<StrengthPower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(DexterityPower))
            return PowerCmd.Apply<DexterityPower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(ArtifactPower))
            return PowerCmd.Apply<ArtifactPower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);
        if (powerType == typeof(BufferPower))
            return PowerCmd.Apply<BufferPower>(choiceContext, base.Owner, amount, base.Owner, cardSource, silent: true);

        return Task.CompletedTask;
    }
}

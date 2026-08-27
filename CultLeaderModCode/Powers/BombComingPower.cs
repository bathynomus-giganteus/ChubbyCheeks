using CultLeaderMod.CultLeaderModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 炸弹来啦：倒计时期间每回合开始获得保留，倒计时归零后对所有敌人造成伤害。
/// </summary>
[RegisterPower]
public class BombComingPower : ModPowerTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Damage", 15m), new DynamicVar("RetainAmt", 2m)];

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/活泼_24.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/活泼_24.png";

    public void Configure(decimal damage, decimal retainAmt)
    {
        DynamicVars["Damage"].BaseValue = damage;
        DynamicVars["RetainAmt"].BaseValue = retainAmt;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await base.AfterPlayerTurnStart(choiceContext, player);

        if (player.Creature != base.Owner || base.Amount <= 0m)
            return;

        Flash();
        if (base.Amount > 1m)
        {
            await ApostleCardPlayHelpers.ApplyLivelyPower(
                choiceContext,
                base.Owner,
                DynamicVars["RetainAmt"].BaseValue,
                base.Owner,
                null
            );
            await PowerCmd.Decrement(this);
            return;
        }

        foreach (var enemy in ApostleCardEffectHelpers.AliveEnemies(base.Owner))
        {
            await CreatureCmd.Damage(
                choiceContext,
                enemy,
                DynamicVars["Damage"].BaseValue,
                ValueProp.Unpowered,
                base.Owner
            );
        }
        await PowerCmd.Remove(this);
    }
}

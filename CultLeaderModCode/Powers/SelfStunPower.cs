using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 眩晕 — 跳过下一回合（不抽牌、不回复能量、不能出牌）。
/// </summary>
[RegisterPower]
public class SelfStunPower : ModPowerTemplate
{
    private bool _stunActive;

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override string CustomIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_埃尔芬.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/badges/portraits/纯粹_埃尔芬.png";

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (_stunActive && card.Owner.Creature == base.Owner)
            return false;
        return true;
    }

    public override bool ShouldDraw(Player player, bool fromHandDraw)
    {
        if (_stunActive && player == base.Owner.Player && !fromHandDraw)
            return false;
        return true;
    }

    public override bool ShouldPlayerResetEnergy(Player player)
    {
        if (_stunActive && player == base.Owner.Player)
            return false;
        return true;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        if (!participants.Contains(base.Owner))
            return;

        if (!_stunActive)
            _stunActive = true;
        else
            await PowerCmd.Remove(this);
    }
}
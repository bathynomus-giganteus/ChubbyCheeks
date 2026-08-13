using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Powers;

/// <summary>
/// 保留 — 每有1层，回合结束时选择至多1张手牌保留，随后减少1层。
/// </summary>
[RegisterPower]
public class RetainPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomIconPath => "res://CultLeaderMod/images/powers/tain.png";
    public override string CustomBigIconPath => "res://CultLeaderMod/images/powers/big/tain.png";

    public override async Task BeforeFlush(PlayerChoiceContext choiceContext, Player player)
    {
        await base.BeforeFlush(choiceContext, player);
        if (player != base.Owner.Player || base.Amount <= 0)
            return;

        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            new CardSelectorPrefs(base.SelectionScreenPrompt, 0, base.Amount),
            null,
            this
        );

        foreach (var card in cards)
            CardCmd.ApplySingleTurnRetain(card);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        if (participants.Contains(base.Owner) && base.Amount > 0)
            await PowerCmd.Decrement(this);
    }
}
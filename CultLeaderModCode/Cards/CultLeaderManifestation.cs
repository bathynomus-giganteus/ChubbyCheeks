using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Powers;
using CultLeaderMod.CultLeaderModCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public sealed class CultLeaderManifestation() :
    CultLeaderModCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override string CustomPortraitPath => "cult_leader_manifestation.png".BigCardImagePath();
    public override string PortraitPath => "cult_leader_manifestation.png".CardImagePath();
    public override string BetaPortraitPath => PortraitPath;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<CultLeaderAuthorityPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Gain 5 authority
        await PowerCmd.Apply<CultLeaderAuthorityPower>(
            choiceContext, Owner.Creature, 5m, Owner.Creature, this);

        // Then gain 1 more (triggers AfterApplied → Elder Form)
        await PowerCmd.Apply<CultLeaderAuthorityPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
}
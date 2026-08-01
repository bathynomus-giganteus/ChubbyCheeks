using BaseLib.Utils;
using CultLeaderMod.CultLeaderModCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace CultLeaderMod.CultLeaderModCode.Cards;

public sealed class CultLeaderDefend() :
    CultLeaderModCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override bool GainsBlock => true;
    public override bool IsBasicStrikeOrDefend => true;
    public override string CustomPortraitPath => "cult_leader_defend.png".BigCardImagePath();
    public override string PortraitPath => "cult_leader_defend.png".CardImagePath();
    public override string BetaPortraitPath => PortraitPath;

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Pure_07 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("RegenAmt", 3m),
            new BlockVar(3m, ValueProp.Move),
            new DynamicVar("DrawAmt", 1m),
        ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/我来保护你.png");

    public Apostle_Pure_07()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await ApostleCardPlayHelpers.ApplyPurePower(
            choiceContext,
            owner,
            DynamicVars["RegenAmt"].BaseValue,
            owner,
            this
        );
        await CreatureCmd.GainBlock(owner, DynamicVars.Block, cardPlay);
        await CardPileCmd.Draw(choiceContext, DynamicVars["DrawAmt"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RegenAmt"].UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["DrawAmt"].UpgradeValueBy(1m);
    }
}


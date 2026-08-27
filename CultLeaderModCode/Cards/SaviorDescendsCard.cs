using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class SaviorDescendsCard : ModCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/savior_descends.png");

    public SaviorDescendsCard()
        : base(2, CardType.Skill, CardRarity.Ancient, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CultLeaderAuthorityPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["Amount"].BaseValue,
            base.Owner.Creature,
            this
        );

        await ApostleRecruitmentHelper.OfferRandomApostleToHandForFreeThisTurn(choiceContext, base.Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Amount"].UpgradeValueBy(1m);
    }
}

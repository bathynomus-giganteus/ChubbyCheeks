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
public class HundredDaysBlessingCard : ModCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("DrawAmt", 2m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/hundred_days_blessing.png");

    public HundredDaysBlessingCard()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;

        await CardPileCmd.Draw(choiceContext, DynamicVars["DrawAmt"].BaseValue, base.Owner);
        await PowerCmd.Apply<CultLeaderAuthorityPower>(
            choiceContext,
            owner,
            1m,
            owner,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DrawAmt"].UpgradeValueBy(1m);
    }
}
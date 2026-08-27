using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_25 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainAmt", 4m), new DynamicVar("HealAmt", 4m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_25.png");

    public Apostle_Lively_25()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;

        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            owner,
            DynamicVars["RetainAmt"].BaseValue,
            owner,
            this
        );

        await CreatureCmd.Heal(owner, DynamicVars["HealAmt"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RetainAmt"].UpgradeValueBy(2m);
        DynamicVars["HealAmt"].UpgradeValueBy(2m);
    }
}

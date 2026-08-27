using CultLeaderMod.CultLeaderModCode.CardTags;
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
public class Apostle_Frenzy_19 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Threshold", 5m), new DynamicVar("VigorGain", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/向前迈进的决心.png");

    public Apostle_Frenzy_19()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await FrenzySpendTrackerPower.EnsureTracker(choiceContext, owner, owner, this);
        var power = await PowerCmd.Apply<ForwardResolvePower>(
            choiceContext,
            owner,
            DynamicVars["Threshold"].BaseValue,
            owner,
            this
        );
        power?.ConfigureVigorGain(DynamicVars["VigorGain"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Threshold"].UpgradeValueBy(-1m);
        DynamicVars["VigorGain"].UpgradeValueBy(1m);
    }
}

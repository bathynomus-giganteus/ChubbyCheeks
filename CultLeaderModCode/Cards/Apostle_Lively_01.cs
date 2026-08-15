using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using CultLeaderMod.CultLeaderModCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_01 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainAmt", 2m), new DynamicVar("Duration", 7m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_01.png");

    public Apostle_Lively_01()
        : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await PowerCmd.Apply<FrogRainPower>(
            choiceContext,
            owner,
            DynamicVars["Duration"].BaseValue,
            owner,
            this
        );
        owner.GetPower<FrogRainPower>()?.ConfigureRetainPerTurn(DynamicVars["RetainAmt"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RetainAmt"].UpgradeValueBy(1m);
    }
}


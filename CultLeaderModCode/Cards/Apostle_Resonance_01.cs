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
public class Apostle_Resonance_01 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CultLeaderCardTags.Apostle,
        CultLeaderCardTags.Pure,
        CultLeaderCardTags.Calm,
        CultLeaderCardTags.Frenzy,
        CultLeaderCardTags.Lively,
        CultLeaderCardTags.Melancholy,
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("HolyAmt", 2m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/resonance/apostle_resonance_01.png");

    public Apostle_Resonance_01()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await PowerCmd.Apply<LittleWishBecomesLightPower>(choiceContext, owner, 1m, owner, this);
        await PowerCmd.Apply<HolyPower>(
            choiceContext,
            owner,
            DynamicVars["HolyAmt"].BaseValue,
            owner,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HolyAmt"].UpgradeValueBy(2m);
    }
}

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
public class Apostle_Frenzy_01 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("BaseHp", 5m), new DynamicVar("HpPerFrenzy", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/小小塞巴斯蒂安.png");

    public Apostle_Frenzy_01()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var resource = ApostleCardEffectHelpers.GetFrenzyResourceAmount(owner);
        var hpPool = DynamicVars["BaseHp"].IntValue + resource * DynamicVars["HpPerFrenzy"].IntValue;
        if (hpPool <= 0)
            return;

        await PowerCmd.Apply<SebastianPower>(
            choiceContext,
            owner,
            hpPool,
            owner,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HpPerFrenzy"].UpgradeValueBy(2m);
    }
}

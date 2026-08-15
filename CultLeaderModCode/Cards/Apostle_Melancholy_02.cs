using System.Linq;
using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Melancholy_02 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("MaxPain", 5m), new DynamicVar("Multiplier", 1m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/magic_bullet_load.png");

    public Apostle_Melancholy_02()
        : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int pain = ApostleCardEffectHelpers.MelancholyStacks(owner);
        int consume = Math.Min(pain, DynamicVars["MaxPain"].IntValue);
        await ApostleCardEffectHelpers.RemoveMelancholyStacks(choiceContext, owner, consume, this);

        int bullets = consume * DynamicVars["Multiplier"].IntValue;
        if (bullets > 0)
            await PowerCmd.Apply<MagicBulletPower>(choiceContext, owner, bullets, owner, this);

        if (base.CardScope == null)
            return;

        var shooter = base.CardScope.CreateCard<Apostle_Melancholy_02_1>(base.Owner);
        await CardPileCmd.Add(shooter, PileType.Draw, CardPilePosition.Top, this, false);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        DynamicVars["Multiplier"].UpgradeValueBy(1m);
    }

}

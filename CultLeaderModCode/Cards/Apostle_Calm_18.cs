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
public class Apostle_Calm_18 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RemoveAmt", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/蜂蜜炸弹.png");

    public Apostle_Calm_18()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var target = cardPlay.Target;

        int before = ApostleCardEffectHelpers.CalmStacks(owner);
        await ApostleCardEffectHelpers.RemoveCalmStacks(choiceContext, owner, DynamicVars["RemoveAmt"].IntValue, this);
        int after = ApostleCardEffectHelpers.CalmStacks(owner);
        int removed = before - after;

        if (removed > 0 && target != null)
            await PowerCmd.Apply<TempStrengthLossPower>(choiceContext, target, removed * 2m, owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RemoveAmt"].UpgradeValueBy(1m);
    }
}

using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_15 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RemoveAmt", 3m), new DynamicVar("HealPerStack", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/里科塔_全套餐.png");

    public Apostle_Calm_15()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int available = ApostleCardEffectHelpers.CalmStacks(owner);
        int remove = Math.Min(DynamicVars["RemoveAmt"].IntValue, available);
        await ApostleCardEffectHelpers.RemoveCalmStacks(choiceContext, owner, remove, this);
        if (remove > 0)
            await CreatureCmd.Heal(owner, remove * DynamicVars["HealPerStack"].BaseValue, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HealPerStack"].UpgradeValueBy(1m);
    }
}

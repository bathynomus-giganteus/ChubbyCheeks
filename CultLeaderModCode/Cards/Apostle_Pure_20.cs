using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
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
public class Apostle_Pure_20 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new BlockVar(8m, ValueProp.Move), new DynamicVar("TriggerAmt", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/帮帮我朋友们.png");

    public Apostle_Pure_20()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var stacks = ApostleCardEffectHelpers.PureStacks(owner);
        int triggerAmt = (int)Math.Min(stacks, DynamicVars["TriggerAmt"].BaseValue);
        if (triggerAmt > 0)
        {
            await ApostleCardEffectHelpers.TriggerPureStacks(choiceContext, owner, triggerAmt, this);
            DynamicVars.Damage.BaseValue += triggerAmt;
        }
        await CreatureCmd.GainBlock(owner, DynamicVars.Block, cardPlay);
        await ApostleCardEffectHelpers.AttackAll(choiceContext, this, cardPlay, owner, DynamicVars.Damage.BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["TriggerAmt"].UpgradeValueBy(2m);
    }
}

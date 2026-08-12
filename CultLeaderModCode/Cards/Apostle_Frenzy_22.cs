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
public class Apostle_Frenzy_22 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(8m, ValueProp.Move), new DynamicVar("DrawThreshold", 5m), new DynamicVar("EnergyThreshold", 20m), new EnergyVar(2)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/机器人矩阵.png");

    public Apostle_Frenzy_22()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var stacks = ApostleCardEffectHelpers.FrenzyStacks(owner);
        await CreatureCmd.GainBlock(owner, DynamicVars.Block, cardPlay);
        if (stacks >= DynamicVars["DrawThreshold"].BaseValue)
            await CardPileCmd.Draw(choiceContext, 1m, base.Owner);
        if (stacks >= DynamicVars["EnergyThreshold"].BaseValue)
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["DrawThreshold"].UpgradeValueBy(-1m);
        DynamicVars["EnergyThreshold"].UpgradeValueBy(-4m);
    }
}

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
public class Apostle_Melancholy_09 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("MaxPain", 3m), new DynamicVar("BlockPerPain", 5m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/魔力喷发.png");

    public Apostle_Melancholy_09()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int pain = ApostleCardEffectHelpers.MelancholyStacks(owner);
        int consume = Math.Min(pain, DynamicVars["MaxPain"].IntValue);
        await ApostleCardEffectHelpers.RemoveMelancholyStacks(choiceContext, owner, consume, this);

        decimal block = consume * DynamicVars["BlockPerPain"].BaseValue;
        if (block > 0m)
            await CreatureCmd.GainBlock(owner, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockPerPain"].UpgradeValueBy(2m);
    }

}

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
public class Apostle_Melancholy_03 : ModCardTemplate
{

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Melancholy];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("PainAmt", 6m), new DynamicVar("Threshold", 20m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/melancholy/土豆番薯.png");

    public Apostle_Melancholy_03()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        await ApostleCardPlayHelpers.ApplyMelancholyPower(
            choiceContext,
            owner,
            DynamicVars["PainAmt"].BaseValue,
            owner,
            this
        );

        int stacks = ApostleCardEffectHelpers.MelancholyStacks(owner);
        int threshold = DynamicVars["Threshold"].IntValue;
        if (stacks < threshold)
            return;

        await ApostleCardEffectHelpers.RemoveMelancholyStacks(choiceContext, owner, threshold, this);
        foreach (var enemy in ApostleCardEffectHelpers.AliveEnemies(owner))
        {
            await CreatureCmd.Stun(enemy);
        }
        await CardCmd.Exhaust(choiceContext, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PainAmt"].UpgradeValueBy(2m);
        DynamicVars["Threshold"].UpgradeValueBy(-4m);
    }

}

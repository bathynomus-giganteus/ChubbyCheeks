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
public class Apostle_Lively_22 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("RetainCost", 9m), new DynamicVar("Repeats", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_22.png");

    public Apostle_Lively_22()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int required = DynamicVars["RetainCost"].IntValue;
        int available = ApostleCardEffectHelpers.LivelyStacks(owner);
        if (available < required)
            return;

        await ApostleCardEffectHelpers.RemoveLivelyStacks(choiceContext, owner, required, this);

        for (int i = 0; i < DynamicVars["Repeats"].IntValue; i++)
        {
            var target = ApostleCardEffectHelpers.RandomEnemy(owner);
            if (target == null)
                break;

            await PowerCmd.Apply<BeePower>(
                choiceContext,
                target,
                1m,
                owner,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RetainCost"].UpgradeValueBy(-3m);
    }
}

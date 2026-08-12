using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Pure_19 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/基础黑客攻击.png");

    public Apostle_Pure_19()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        var stacks = ApostleCardEffectHelpers.PureStacks(base.Owner.Creature);
        if (stacks <= 0)
            return;

        await ApostleCardEffectHelpers.Attack(choiceContext, this, cardPlay, target, stacks);
        await ApostleCardEffectHelpers.ApplyTemporaryStrengthLoss(
            choiceContext,
            target,
            stacks,
            base.Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        // TODO: upgrade should reduce this card's cost to 0 once a safe cost mutator is confirmed.
    }
}

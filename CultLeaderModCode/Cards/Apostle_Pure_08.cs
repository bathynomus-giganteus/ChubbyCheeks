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
public class Apostle_Pure_08 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Pure];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("WeakAmt", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/pure/调皮的笑容.png");

    public Apostle_Pure_08()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        await PowerCmd.Apply<WeakPower>(choiceContext, target, DynamicVars["WeakAmt"].BaseValue, base.Owner.Creature, this);
        var debuffCount = target.Powers.Count(power => power.Type == MegaCrit.Sts2.Core.Entities.Powers.PowerType.Debuff);
        if (debuffCount > 0)
            await ApostleCardPlayHelpers.ApplyPurePower(choiceContext, base.Owner.Creature, debuffCount, base.Owner.Creature, this);
        // The source design raises this card's cost by 1 for the combat after use.
        // We intentionally leave that transient cost mutation out until the STS2 card-cost
        // API is nailed down, because unsafe mutations have previously broken card flow.
    }

    protected override void OnUpgrade()
    {
        // Upgraded version currently removes the temporary cost increase behavior above.
    }
}

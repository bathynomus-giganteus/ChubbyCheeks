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
        var debuffTypes = ApostleCardEffectHelpers.CountDebuffTypes(target);
        if (debuffTypes > 0)
            await ApostleCardPlayHelpers.ApplyPurePower(choiceContext, base.Owner.Creature, debuffTypes * 3, base.Owner.Creature, this);
        if (!IsUpgraded)
            base.EnergyCost.AddThisCombat(1);
    }

    protected override void OnUpgrade()
    {
        // Upgraded version currently removes the temporary cost increase behavior above.
    }
}

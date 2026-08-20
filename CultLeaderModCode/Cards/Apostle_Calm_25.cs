using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_25 : ModCardTemplate
{
    protected override bool HasEnergyCostX => true;

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("BonusHits", 0m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/XG_激光.png");

    public Apostle_Calm_25()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        int hits = (int)base.ResolveEnergyXValue() + DynamicVars["BonusHits"].IntValue;
        decimal hitDamage = ApostleCardEffectHelpers.CalmStacks(owner);
        if (hits <= 0)
            return;

        var combatState = owner.CombatState;
        if (combatState == null)
            return;

        await DamageCmd
            .Attack(hitDamage)
            .WithHitCount(hits)
            .FromCard(this, cardPlay)
            .TargetingRandomOpponents(combatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusHits"].UpgradeValueBy(1m);
    }
}

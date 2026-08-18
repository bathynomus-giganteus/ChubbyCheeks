using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_06 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move)];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_06.png");

    public Apostle_Lively_06()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner;
        var ownerCreature = owner.Creature;
        var hand = PileType.Hand.GetPile(owner);
        var discarded = hand.Cards.Where(c => c != this).ToList();

        if (discarded.Count == 0)
            return;

        await CardCmd.Discard(choiceContext, discarded);

        await ApostleCardPlayHelpers.ApplyLivelyPower(
            choiceContext,
            ownerCreature,
            discarded.Count,
            ownerCreature,
            this
        );

        var combatState = ownerCreature.CombatState;
        if (combatState == null)
            return;

        await DamageCmd
            .Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(discarded.Count)
            .FromCard(this, cardPlay)
            .TargetingRandomOpponents(combatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}

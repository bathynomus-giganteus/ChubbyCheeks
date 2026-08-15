using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_05 : ModCardTemplate
{
    private const int TransformThreshold = 100;

    private int _damageTaken;

    [SavedProperty]
    public int DamageTaken
    {
        get => _damageTaken;
        set
        {
            AssertMutable();
            _damageTaken = value;
        }
    }

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("Progress", 0m)];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_05.png");

    public Apostle_Lively_05()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override void AfterCreated()
    {
        base.AfterCreated();
        DynamicVars["Progress"].BaseValue = DamageTaken;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);

        if (target != base.Owner.Creature)
            return;

        // Counting only starts once this copy has actually been added to the deck.
        if (!base.FloorAddedToDeck.HasValue)
            return;

        // Only monster attacks on the player count; events and self-damage do not.
        if (dealer == null || dealer == base.Owner.Creature || !props.IsPoweredAttack())
            return;

        if (DamageTaken >= TransformThreshold)
            return;

        DamageTaken += 1;
        DynamicVars["Progress"].BaseValue = DamageTaken;
        RefreshProgressVisual();

        if (DamageTaken >= TransformThreshold)
            await TransformToMeltedButter(choiceContext);
    }

    private void RefreshProgressVisual()
    {
        if (base.Owner == null)
            return;

        var hand = PileType.Hand.GetPile(base.Owner);
        if (hand == null || !hand.Cards.Contains(this))
            return;

        NCard.FindOnTable(this)?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
    }

    private async Task TransformToMeltedButter(PlayerChoiceContext choiceContext)
    {
        var scope = base.CardScope;
        if (scope == null)
            return;

        var replacement = scope.CreateCard<Apostle_Lively_05_1>(base.Owner);
        if (replacement == null)
            return;

        if (IsUpgraded)
            CardCmd.Upgrade(replacement, CardPreviewStyle.None);

        await CardCmd.Transform(this, replacement, CardPreviewStyle.None);
    }
}

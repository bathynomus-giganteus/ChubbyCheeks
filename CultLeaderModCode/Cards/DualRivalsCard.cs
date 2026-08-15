using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class DualRivalsCard : ModCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(20m, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/dual_rivals.png");

    public DualRivalsCard()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        var owner = base.Owner.Creature;
        await ApostleCardEffectHelpers.Attack(
            choiceContext,
            this,
            cardPlay,
            cardPlay.Target,
            DynamicVars.Damage.BaseValue
        );

        await PowerCmd.Apply<CultLeaderAuthorityPower>(
            choiceContext,
            owner,
            1m,
            owner,
            this
        );
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
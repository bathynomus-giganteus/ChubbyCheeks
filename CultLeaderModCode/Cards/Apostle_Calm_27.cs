using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Calm_27 : ModCardTemplate
{
    protected override bool HasEnergyCostX => true;

    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Calm];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(0m, ValueProp.Move),
        new DynamicVar("PlatingAmt", 3m),
        new DynamicVar("BonusRepeats", 0m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/calm/calm_27.png");

    public Apostle_Calm_27()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = base.Owner.Creature;
        var repeats = (int)ResolveEnergyXValue() + DynamicVars["BonusRepeats"].IntValue;
        for (var i = 0; i < repeats; i++)
        {
            await ApostleCardPlayHelpers.ApplyCalmPower(
                choiceContext,
                owner,
                DynamicVars["PlatingAmt"].BaseValue,
                owner,
                this);

            var damage = DynamicVars.Damage.BaseValue
                + ApostleCardEffectHelpers.CalmStacks(owner)
                + (ApostleCardEffectHelpers.LivelyStacks(owner) + ApostleCardEffectHelpers.PureStacks(owner)) / 3;
            await ApostleCardEffectHelpers.AttackAll(choiceContext, this, cardPlay, owner, damage);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusRepeats"].UpgradeValueBy(1m);
    }
}

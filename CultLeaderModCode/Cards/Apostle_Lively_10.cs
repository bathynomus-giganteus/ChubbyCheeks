using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace CultLeaderMod.CultLeaderModCode.Cards;

[RegisterCard(typeof(CultLeaderModCardPool))]
public class Apostle_Lively_10 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Lively];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Multiplier", 5m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override CardAssetProfile AssetProfile =>
        new(
            PortraitPath: "res://CultLeaderMod/images/card_portraits/lively/lively_10.png"
        );

    public Apostle_Lively_10()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        var owner = base.Owner.Creature;
        decimal retainStacks =
            (owner.GetPower<RetainPower>()?.Amount ?? 0m)
            + (owner.GetPower<HappinessPower>()?.Amount ?? 0m);
        decimal damage = retainStacks * DynamicVars["Multiplier"].BaseValue;

        var power = await PowerCmd.Apply<FateClockPower>(
            choiceContext,
            cardPlay.Target,
            3m,
            owner,
            this
        );
        if (power != null)
        {
            power.SetDamage(damage);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Multiplier"].UpgradeValueBy(3m);
    }
}


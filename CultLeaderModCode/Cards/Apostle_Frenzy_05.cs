using CultLeaderMod.CultLeaderModCode.CardTags;
using CultLeaderMod.CultLeaderModCode.Character;
using CultLeaderMod.CultLeaderModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class Apostle_Frenzy_05 : ModCardTemplate
{
    protected override HashSet<CardTag> CanonicalTags =>
        [CultLeaderCardTags.Apostle, CultLeaderCardTags.Frenzy];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move), new DynamicVar("GoldAmt", 10m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile =>
        new(PortraitPath: "res://CultLeaderMod/images/card_portraits/frenzy/Money_Gun.png");

    public Apostle_Frenzy_05()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;

        var attack = await ApostleCardEffectHelpers.AttackAndGetResult(
            choiceContext,
            this,
            cardPlay,
            target,
            DynamicVars.Damage.BaseValue
        );

        if (attack.Results
            .SelectMany((List<DamageResult> resultGroup) => resultGroup)
            .Any(result => result.WasTargetKilled))
        {
            await PlayerCmd.GainGold(DynamicVars["GoldAmt"].BaseValue, base.Owner, true);
        }

    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["GoldAmt"].UpgradeValueBy(5m);
    }
}
